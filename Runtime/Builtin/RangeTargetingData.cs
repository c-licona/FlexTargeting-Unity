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
        [SerializeField] private Transform _targetingOrigin;

        [Header("Settings")]
        [SerializeField, Min(0.0f)] private float _maxRange = 10.0f;
        [SerializeField, Min(0.0f)] private float _losOriginOffset = 0.0f;
        [SerializeField] private LayerMask _losLayerMask = Physics.DefaultRaycastLayers;

        public Vector3 TargetingOriginPosition => _targetingOrigin.position;
        public float MaxRange { get => _maxRange; set => _maxRange = value; }
        public float LosOriginOffset { get => _losOriginOffset; set => _losOriginOffset = value; }
        public LayerMask LosLayerMask { get => _losLayerMask; set => _losLayerMask = value; }

        public bool ScoreTarget(IFlexTarget flexTarget, out float score)
        {
            /*
             * The distance between the target and the origin is used as the targeting score. Only targets that are
             * within the max range are accepted as valid targets, the rest are filtered out
             */
            score = Vector3.Distance(TargetingOriginPosition, flexTarget.TargetPosition);
            return score <= _maxRange; // this should always be true since this is only being called on in range targets
        }
    }
}
