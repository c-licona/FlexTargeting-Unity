using System;
using UnityEngine;

namespace Cyclic.FlexTargeting.Builtin
{
    /// <summary>
    /// A targeting solution that gets a group of in range targets, and ranks targets by direction, but with increasing
    /// bands of distance. So the closest band of targets is sorted higher than farther bands of targets, but within
    /// each band targets are sorted by direction
    /// </summary>
    [Serializable]
    public class ComboTargetingData : IFlexData<IFlexTarget>
    {
        [Header("References")]
        [SerializeField] private TargeterOriginSource _targeterOriginSource = TargeterOriginSource.Transform;
        [SerializeField] private Transform _targeterOrigin = null;

        [Header("Settings")]
        [SerializeField, Min(0.0f)] private float _maxRange = 10.0f;
        [SerializeField, Min(0.0f)] private float _halfAngle = 15.0f;
        [SerializeField, Min(Single.Epsilon)] private float _rangeBandWidth = 10.0f;
        [Tooltip("Defines a radius around the origin point from which LOS checks will begin. Use a non-zero value " +
                 "if there is a chance that LOS checks will be blocked by the targeter itself.")]
        [SerializeField, Min(0.0f)] private float _losBufferRadius = 0.0f;
        [SerializeField] private LayerMask _losLayerMask = Physics.AllLayers;

        public float MaxRange => _maxRange;
        public float LosBufferRadius { get => _losBufferRadius; set => _losBufferRadius = value; }
        public LayerMask LosLayerMask => _losLayerMask;

        public TargeterOriginSource TargeterOriginSource { get => _targeterOriginSource; set => _targeterOriginSource = value; }
        public Transform TargeterOrigin { get => _targeterOrigin; set => _targeterOrigin = value; }
        public float HalfAngle { get => _halfAngle; set => _halfAngle = value; }
        public float RangeBandWidth { get => _rangeBandWidth; set => _rangeBandWidth = value; }
        public Vector3 ManualOriginPos { get; set; } = Vector3.zero;
        public Vector3 ManualOriginDir { get; set; } = Vector3.zero;

        public Vector3 TargeterPosition => _targeterOriginSource switch
        {
            TargeterOriginSource.Transform => _targeterOrigin.position,
            TargeterOriginSource.Manual => ManualOriginPos,
            _ => throw new ArgumentOutOfRangeException()
        };

        public Vector3 TargeterDirection => _targeterOriginSource switch
        {
            TargeterOriginSource.Transform => _targeterOrigin.forward,
            TargeterOriginSource.Manual => ManualOriginDir,
            _ => throw new ArgumentOutOfRangeException()
        };

        public bool ScoreTarget(IFlexTarget target, out float score)
        {
            /*
             * Targets from the origin to max range are separated into discrete bands based on their distance, and each
             * band bumps up the score for each target in that band by the same amount. The angle between the origin
             * forward direction and the vector from origin to target is used as the primary tiebreaker score for
             * targets within each band. The result is that targets that are within closer bands and have the smallest
             * angle difference are the highest priority.
             *
             * ---Example---
             * MaxRange: 50m, HalfAngle: 45deg, RangeBandWidth: 10m
             * 0m (T1-5deg, T2-20deg, T3-10deg) -> 10m (T4-50deg, T5-3deg) -> 20m (T6-12deg) -> 30m
             *
             * In this example, T1 would be the best target, followed by T3 and then T2. Then T5 would come next.
             * Notice that T5 has a better angle score than T1, but since it's an entire band away from the first
             * group, its score ends up being lower than all the targets in the first group. T4 WOULD be the next
             * best target, but it is outside the half angle range so it gets culled out as an invalid target. The
             * last valid target is T6.
             */

            Vector3 oPos = TargeterPosition;
            Vector3 oForward = TargeterDirection;
            Vector3 targetPos = target.TargetPosition;

            float distance = Vector3.Distance(oPos, targetPos);
            int band = Mathf.FloorToInt(distance / RangeBandWidth);
            float angle = Vector3.Angle(targetPos - oPos, oForward);
            score = angle + (band * 180.0f);

            return angle <= _halfAngle;
        }
    }
}
