using System;
using UnityEngine;

namespace Cyclic.FlexTargeting.Builtin
{
    /// <summary>
    /// A simpler targeting solution that scores targets based on how close they are, with the closest target being the
    /// best target.
    /// </summary>
    [Serializable]
    public class RangeTargetingData : IFlexData<IFlexTarget>, IFlexDataGizmo
    {
        [Header("References")]
        [Tooltip("Determine where targeter origin values are sourced from.")]
        public TargeterOriginSource TargeterOriginSource = TargeterOriginSource.Transform;

        [Tooltip("The transform to source targeter origin values from.")]
        public Transform TargeterOrigin = null;


        [Header("Settings")]
        [Tooltip("The maximum range at which targets will be considered.")]
        [SerializeField, Min(0.0f)] protected float _maxRange = 10.0f;

        [Tooltip("Defines a radius around the origin point from which LOS checks will begin. Use a non-zero value " +
                 "if there is a chance that LOS checks will be blocked by the targeter itself.")]
        [SerializeField, Min(0.0f)] protected float _losBufferRadius = 0.0f;

        [Tooltip("The layers that will block the LOS raycasts. Set to None to ignore LOS checks.")]
        [SerializeField] protected LayerMask _losLayerMask = Physics.DefaultRaycastLayers;

        [Tooltip("Determine whether the LOS raycasts hit triggers.")]
        [SerializeField] protected QueryTriggerInteraction _losQueryTriggerInteraction = QueryTriggerInteraction.Ignore;

        public virtual float MaxRange { get => _maxRange; set => _maxRange = value; }
        public virtual float LosBufferRadius { get => _losBufferRadius; set => _losBufferRadius = value; }
        public virtual LayerMask LosLayerMask { get => _losLayerMask; set => _losLayerMask = value; }
        public virtual QueryTriggerInteraction LosQueryTriggerInteraction { get => _losQueryTriggerInteraction; set => _losQueryTriggerInteraction = value; }
        public virtual Vector3 ManualOriginPos { get; set; } = Vector3.zero;
        public virtual Vector3 TargeterPosition => TargeterOriginSource switch
        {
            TargeterOriginSource.Transform => TargeterOrigin.position,
            TargeterOriginSource.Manual => ManualOriginPos,
            _ => throw new ArgumentOutOfRangeException()
        };

        public virtual bool ScoreTarget(IFlexTarget target, out float score)
        {
            /*
             * The distance between the target and the origin is used as the targeting score. Only targets that are
             * within the max range are accepted as valid targets, the rest are filtered out
             */
            score = Vector3.Distance(TargeterPosition, target.TargetPosition);
            return score <= MaxRange; // this should always be true since this is only being called on in range targets
        }

        public virtual void DrawGizmos(Transform targeterTransform)
        {
#if UNITY_EDITOR
            Vector3 origin = TargeterOriginSource switch
            {
                TargeterOriginSource.Transform => TargeterOrigin != null ? TargeterOrigin.position : targeterTransform.position,
                TargeterOriginSource.Manual => targeterTransform.position,
                _ => throw new ArgumentOutOfRangeException()
            };

            Gizmos.DrawWireSphere(origin, _maxRange);
            if (!Mathf.Approximately(_losBufferRadius, 0.0f)) Gizmos.DrawWireSphere(origin, _losBufferRadius);
#endif
        }
    }
}
