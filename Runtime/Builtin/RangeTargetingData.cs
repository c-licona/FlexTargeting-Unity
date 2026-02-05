// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

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
        [Tooltip(FlexTargetingTooltips.TargeterOriginSource)]
        public TargeterOriginSource TargeterOriginSource = TargeterOriginSource.Transform;

        [Tooltip(FlexTargetingTooltips.TargeterOrigin)]
        public Transform TargeterOrigin = null;


        [Header("Settings")]
        [Tooltip(FlexTargetingTooltips.MaxRange)]
        [SerializeField, Min(0.0f)] protected float _maxRange = 10.0f;

        [Tooltip(FlexTargetingTooltips.TargeterLosBufferRadius)]
        [SerializeField, Min(0.0f)] protected float _losBufferRadius = 0.0f;

        [Tooltip(FlexTargetingTooltips.LosLayerMask)]
        [SerializeField] protected LayerMask _losLayerMask = Physics.DefaultRaycastLayers;

        [Tooltip(FlexTargetingTooltips.LosQueryTriggerInteraction)]
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
