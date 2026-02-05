// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

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
    public class ComboTargetingData : IFlexData<IFlexTarget>, IFlexDataGizmo
    {
        [Header("References")]
        [Tooltip(FlexTargetingTooltips.TargeterOriginSource)]
        public TargeterOriginSource TargeterOriginSource = TargeterOriginSource.Transform;

        [Tooltip(FlexTargetingTooltips.TargeterOrigin)]
        public Transform TargeterOrigin = null;


        [Header("Settings")]
        [Tooltip(FlexTargetingTooltips.MaxRange)]
        [SerializeField, Min(0.0f)] protected float _maxRange = 10.0f;

        [Tooltip(FlexTargetingTooltips.HalfAngle)]
        [SerializeField, Range(0.0f, 180.0f)] protected float _halfAngle = 15.0f;

        [Tooltip(FlexTargetingTooltips.RangeBandWidth)]
        [SerializeField, Min(Single.Epsilon)] protected float _rangeBandWidth = 2.0f;

        [Tooltip(FlexTargetingTooltips.LosRaySize)]
        [SerializeField, Min(0.0f)] protected float _losRaySize = 0.0f;

        [Tooltip(FlexTargetingTooltips.TargeterLosBufferRadius)]
        [SerializeField, Min(0.0f)] protected float _losBufferRadius = 0.0f;

        [Tooltip(FlexTargetingTooltips.LosLayerMask)]
        [SerializeField] protected LayerMask _losLayerMask = Physics.DefaultRaycastLayers;

        [Tooltip(FlexTargetingTooltips.LosQueryTriggerInteraction)]
        [SerializeField] protected QueryTriggerInteraction _losQueryTriggerInteraction = QueryTriggerInteraction.Ignore;


        public virtual float MaxRange { get => _maxRange; set => _maxRange = value; }
        public virtual float HalfAngle { get => _halfAngle; set => _halfAngle = value; }
        public virtual float RangeBandWidth { get => _rangeBandWidth; set => _rangeBandWidth = value; }
        public virtual float LosRaySize { get => _losRaySize; set => _losRaySize = value; }
        public virtual float LosBufferRadius { get => _losBufferRadius; set => _losBufferRadius = value; }
        public virtual LayerMask LosLayerMask { get => _losLayerMask; set => _losLayerMask = value; }
        public virtual QueryTriggerInteraction LosQueryTriggerInteraction { get => _losQueryTriggerInteraction; set => _losQueryTriggerInteraction = value; }
        public virtual Vector3 ManualOriginPos { get; set; } = Vector3.zero;
        public virtual Vector3 ManualOriginDir { get; set; } = Vector3.zero;
        public virtual Vector3 TargeterPosition => TargeterOriginSource switch
        {
            TargeterOriginSource.Transform => TargeterOrigin.position,
            TargeterOriginSource.Manual => ManualOriginPos,
            _ => throw new ArgumentOutOfRangeException()
        };
        public virtual Vector3 TargeterDirection => TargeterOriginSource switch
        {
            TargeterOriginSource.Transform => TargeterOrigin.forward,
            TargeterOriginSource.Manual => ManualOriginDir,
            _ => throw new ArgumentOutOfRangeException()
        };

        public virtual bool ScoreTarget(IFlexTarget target, out float score)
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

            return angle <= HalfAngle;
        }

        public void DrawGizmos(Transform targeterTransform)
        {
#if UNITY_EDITOR
            Transform transform = TargeterOriginSource switch
            {
                TargeterOriginSource.Transform => TargeterOrigin != null ? TargeterOrigin : targeterTransform,
                TargeterOriginSource.Manual => targeterTransform,
                _ => throw new ArgumentOutOfRangeException()
            };

            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            Gizmos.DrawWireSphere(origin, _maxRange);
            if (!Mathf.Approximately(_losBufferRadius, 0.0f)) Gizmos.DrawWireSphere(origin, _losBufferRadius);
            Gizmos.DrawLine(origin, origin + direction * _maxRange);
            if (_halfAngle > Mathf.Epsilon && _halfAngle < 180.0f)
            {
                Gizmos.DrawLine(origin, origin + (Quaternion.AngleAxis(_halfAngle, transform.up) * direction) * _maxRange);
                Gizmos.DrawLine(origin, origin + (Quaternion.AngleAxis(-_halfAngle, transform.up) * direction) * _maxRange);
            }

            if (_rangeBandWidth > 0.01f) // avoid division by 0 and showing discs that are too small to be useful
            {
                int numBands = Mathf.FloorToInt(_maxRange / _rangeBandWidth);
                numBands = Mathf.Clamp(numBands, 0, 20); // limit max gizmo discs to avoid tanking the editor
                for (int i = 1; i < numBands + 1; i++)
                {
                    UnityEditor.Handles.DrawWireDisc(origin, transform.up, _rangeBandWidth * i);
                }
            }
#endif
        }
    }
}
