// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using UnityEngine;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// A generic MonoBehavior version of a <see cref="IFlexTarget"/> that can automatically register/unregister
    /// itself to the <see cref="FlexTargetRepository"/> on awake/destroy, enable/disable or manually.
    /// </summary>
    /// <typeparam name="TSelf">This type should be the derived type itself so that it can be properly stored
    /// in the <see cref="FlexTargetRepository"/></typeparam>
    /// <example>
    /// An example of setting <typeparamref name="TSelf"/> correctly in a derived class:
    /// <code>class AimAssistTarget : FlexTargetComponent{AimAssistTarget}</code>
    /// </example>
    public abstract class FlexTargetComponent<TSelf> : MonoBehaviour, IFlexTarget where TSelf : FlexTargetComponent<TSelf>
    {
        /// <summary>
        /// The <see cref="FlexTargetComponent{TSelf}"/> takes care of some common boilerplate for
        /// <see cref="IFlexTarget"/>s such as automatically registering this target to the
        /// <see cref="FlexTargetRepository"/> on awake/destroy or enable/disable. Alternatively, this can be set to
        /// manual to skip automatic registration.
        /// </summary>
        protected enum RegisterWhen
        {
            [Tooltip("This target gets registered to the repository on Awake, and unregistered on Destroy.")]
            OnAwake,
            [Tooltip("This target gets registered to the repository on Enable, and unregistered on Disable.")]
            OnEnable,
            [Tooltip("This target must be registered and unregistered from the repository manually.")]
            Manual
        }

        /// <summary>
        /// Determine when this target component registers itself with the <see cref="FlexTargetRepository"/>
        /// </summary>
        protected virtual RegisterWhen RegisterTargetWhen => RegisterWhen.OnAwake;

        protected virtual void Awake()
        {
            if (RegisterTargetWhen == RegisterWhen.OnAwake)
            {
                FlexTargetRepository.AddTarget(this as TSelf);
            }
        }

        protected virtual void OnDestroy()
        {
            if (RegisterTargetWhen == RegisterWhen.OnAwake)
            {
                FlexTargetRepository.RemoveTarget(this as TSelf);
            }
        }

        protected virtual void OnEnable()
        {
            if (RegisterTargetWhen == RegisterWhen.OnEnable)
            {
                FlexTargetRepository.AddTarget(this as TSelf);
            }
        }

        protected virtual void OnDisable()
        {
            if (RegisterTargetWhen == RegisterWhen.OnEnable)
            {
                FlexTargetRepository.RemoveTarget(this as TSelf);
            }
        }

        // the default implementation of RegisterTargetWhen is to register on awake. So the most common default
        // implementation of this method is to simply return if this target is currently enabled.
        public virtual bool IsTargetValid => enabled;
        public abstract Vector3 TargetPosition { get; }
        public abstract float LosBufferRadius { get; }

        #if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(TargetPosition, LosBufferRadius);
        }
        #endif
    }
}
