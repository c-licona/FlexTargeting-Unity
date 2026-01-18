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
        [SerializeField] private Transform _targetingOrigin;

        [Header("Settings")]
        [SerializeField, Min(0.0f)] private float _maxRange = 10.0f;
        [SerializeField, Min(0.0f)] private float _halfAngle = 15.0f;
        [SerializeField, Min(Single.Epsilon)] private float _rangeBandWidth = 10.0f;
        [SerializeField] private LayerMask _losLayerMask = Physics.AllLayers;

        public Vector3 TargetingOriginPosition => _targetingOrigin.position;
        public float MaxRange => _maxRange;
        public LayerMask LosLayerMask => _losLayerMask;

        /// <summary>
        /// Targets from the origin to max range are separated into discrete bands based on their distance, and each
        /// band bumps up the score for each target in that band by the same amount. The angle between the origin
        /// forward direction and the vector from origin to target is used as the primary tie breaker score for
        /// targets within each band.
        /// The result is that targets that are within closer bands and have the smallest angle difference are the
        /// highest priority.
        /// </summary>
        /// <example>
        /// MaxRange: 50m, HalfAngle: 45deg, RangeBandWidth: 10m
        /// )   T1-5deg, T2-20deg, T3-10deg   10m)   T4-50deg, T5-3deg   20m)   T6-12deg   30m)
        ///
        /// In this example, T1 would be the best target, followed by T3 and then T2. Then T5 would come next. Notice
        /// that T5 has a better angle score than T1, but since it's an entire band away from the first group, its
        /// score ends up being lower than all of the targets in the first group. T4 would be the next best target but
        /// it is outside of the half angle range so it gets culled out as an invalid target. The last target is T6.
        /// </example>
        public bool ScoreTarget(IFlexTarget flexTarget, out float score)
        {
            Vector3 oPos = TargetingOriginPosition;
            Vector3 oForward = _targetingOrigin.forward;
            Vector3 targetPos = flexTarget.TargetPosition;

            float distance = Vector3.Distance(oPos, targetPos);
            int band = Mathf.FloorToInt(distance / _rangeBandWidth);
            float angle = Vector3.Angle(targetPos - oPos, oForward);
            score = angle + (band * 180.0f);

            return angle <= _halfAngle;
        }
    }
}
