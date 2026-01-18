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
    /// <code>class InteractionTarget : FlexTargetComponent{InteractionTarget}</code>
    /// </example>
    public abstract class FlexTargetComponent<TSelf> : MonoBehaviour, IFlexTarget where TSelf : FlexTargetComponent<TSelf>
    {
        protected virtual void Awake()
        {
            FlexTargetRepository.Instance.OfType<TSelf>().AddTarget(this as TSelf);
        }

        protected virtual void OnDestroy()
        {
            // Use TryGetInstance which will not instantiate a new instance if it is null. This will avoid issues
            // when the application is quitting
            if (FlexTargetRepository.Instance.TryOfType<TSelf>(out var outInstance))
            {
                outInstance.RemoveTarget(this as TSelf);
            }
        }

        public virtual bool IsTargetValid => enabled;
        public abstract Vector3 TargetPosition { get; }
        public abstract float LosRadius { get; }

        #if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(TargetPosition, LosRadius);
        }
        #endif
    }
}
