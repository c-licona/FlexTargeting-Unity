// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using System;
using UnityEngine;

namespace Cyclic.FlexTargeting.Builtin
{
    /// <summary>
    /// The most common flex targeting solution. Targets are collected within a certain range and then sorted by which
    /// target is most closely aligned with a certain direction. Any targets that are outside a cone that is defined
    /// by the targeting direction and a half angle value will be discarded.
    /// </summary>
    [Serializable]
    public class DirectionTargetingData : IFlexData<IFlexTarget>, IFlexDataGizmo
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
             * The angle between the targeting origin direction and the vector from origin to target is used as the
             * score. Since score is sorted by smallest, the target with the smallest angle difference is considered
             * as the best target. Only targets that are within the given half angle are accepted as valid targets, the
             * rest are filtered out
             */
            Vector3 oPos = TargeterPosition;
            Vector3 oForward = TargeterDirection;
            Vector3 targetPos = target.TargetPosition;

            score = Vector3.Angle(targetPos - oPos, oForward);
            return score <= HalfAngle;
        }

        public virtual void DrawGizmos(Transform targeterTransform)
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
#endif
        }
    }
}
