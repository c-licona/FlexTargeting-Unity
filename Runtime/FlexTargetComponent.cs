using UnityEngine;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// A generic MonoBehavior version of a <see cref="IFlexTarget"/> that will automatically register/unregister
    /// itself to the <see cref="FlexTargetRepository"/> on enable/disable.
    /// </summary>
    /// <typeparam name="TSelf">This type should be the derived type itself so that it can be properly stored
    /// in the <see cref="FlexTargetRepository"/></typeparam>
    /// <example>
    /// An example of setting <typeparamref name="TSelf"/> correctly in a derived class:
    /// <code>class AimAssistTarget : FlexTargetComponent{AimAssistTarget}</code>
    /// </example>
    public abstract class FlexTargetComponent<TSelf> : MonoBehaviour, IFlexTarget where TSelf : FlexTargetComponent<TSelf>
    {
        protected enum RegisterWhen
        {
            [Tooltip("This target gets registered to the repository on Awake, and unregistered on Destroy.")]
            OnAwake,
            [Tooltip("This target gets registered to the repository on Enable, and unregistered on Disable.")]
            OnEnable,
            [Tooltip("This target must be registered and unregistered from the repository manually.")]
            Manual
        }

        [Header("Flex Target Settings")]
        [Tooltip("Set if and when this target automatically registers itself with the target repository.")]
        [SerializeField] protected RegisterWhen _registerWhen = RegisterWhen.OnAwake;

        protected virtual void Awake()
        {
            if (_registerWhen == RegisterWhen.OnAwake)
            {
                FlexTargetRepository.AddTarget(this as TSelf);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_registerWhen == RegisterWhen.OnAwake)
            {
                FlexTargetRepository.RemoveTarget(this as TSelf);
            }
        }

        protected virtual void OnEnable()
        {
            if (_registerWhen == RegisterWhen.OnEnable)
            {
                FlexTargetRepository.AddTarget(this as TSelf);
            }
        }

        protected virtual void OnDisable()
        {
            if (_registerWhen == RegisterWhen.OnEnable)
            {
                FlexTargetRepository.RemoveTarget(this as TSelf);
            }
        }

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
