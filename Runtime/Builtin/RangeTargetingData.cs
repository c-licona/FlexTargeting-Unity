using System;
using UnityEngine;

namespace Cyclic.FlexTargeting.Builtin
{
    /// <summary>
    /// A simpler targeting solution that scores targets based on how close they are, with the closest target being the
    /// best target.
    /// </summary>
    [Serializable]
    public class RangeTargetingData : IFlexData<IFlexTarget>
    {
        [Header("References")]
        [SerializeField] private TargeterOriginSource _targeterOriginSource = TargeterOriginSource.Transform;
        [SerializeField] private Transform _targeterOrigin = null;

        [Header("Settings")]
        [SerializeField, Min(0.0f)] private float _maxRange = 10.0f;
        [Tooltip("Defines a radius around the origin point from which LOS checks will begin. Use a non-zero value " +
                 "if there is a chance that LOS checks will be blocked by the targeter itself.")]
        [SerializeField, Min(0.0f)] private float _losBufferRadius = 0.0f;
        [SerializeField] private LayerMask _losLayerMask = Physics.DefaultRaycastLayers;

        public float MaxRange { get => _maxRange; set => _maxRange = value; }
        public float LosBufferRadius { get => _losBufferRadius; set => _losBufferRadius = value; }
        public LayerMask LosLayerMask { get => _losLayerMask; set => _losLayerMask = value; }

        public TargeterOriginSource TargeterOriginSource { get => _targeterOriginSource; set => _targeterOriginSource = value; }
        public Transform TargeterOrigin { get => _targeterOrigin; set => _targeterOrigin = value; }
        public Vector3 ManualOriginPos { get; set; } = Vector3.zero;

        public Vector3 TargeterPosition => _targeterOriginSource switch
        {
            TargeterOriginSource.Transform => _targeterOrigin.position,
            TargeterOriginSource.Manual => ManualOriginPos,
            _ => throw new ArgumentOutOfRangeException()
        };

        public bool ScoreTarget(IFlexTarget target, out float score)
        {
            /*
             * The distance between the target and the origin is used as the targeting score. Only targets that are
             * within the max range are accepted as valid targets, the rest are filtered out
             */
            score = Vector3.Distance(TargeterPosition, target.TargetPosition);
            return score <= _maxRange; // this should always be true since this is only being called on in range targets
        }
    }
}
