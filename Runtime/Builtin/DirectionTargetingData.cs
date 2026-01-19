using System;
using UnityEngine;

namespace Cyclic.FlexTargeting.Builtin
{
    public enum TargetingOriginSource
    {
        Transform,
        Manual
    }

    /// <summary>
    /// The most common flex targeting solution. Targets are collected within a certain range and then sorted by which
    /// target is most closely aligned with a certain direction. Any targets that are outside a cone that is defined
    /// by the targeting direction and a half angle value will be discarded.
    /// </summary>
    [Serializable]
    public class DirectionTargetingData : IFlexData<IFlexTarget>
    {
        [Header("References")]
        [SerializeField] private TargetingOriginSource _targetingOriginSource = TargetingOriginSource.Transform;
        [SerializeField] private Transform _targetingOrigin = null;

        [Header("Settings")]
        [SerializeField, Min(0.0f)] private float _maxRange = 10.0f;
        [SerializeField, Min(0.0f)] private float _halfAngle = 15.0f;
        [Tooltip("Defines a radius around the origin point from which LOS checks will begin. Use a non-zero value " +
                 "if there is a chance that LOS checks will be blocked by the targeter itself.")]
        [SerializeField, Min(0.0f)] private float _losBufferRadius = 0.0f;
        [SerializeField] private LayerMask _losLayerMask = Physics.DefaultRaycastLayers;

        public float MaxRange { get => _maxRange; set => _maxRange = value; }
        public float LosBufferRadius { get => _losBufferRadius; set => _losBufferRadius = value; }
        public LayerMask LosLayerMask { get => _losLayerMask; set => _losLayerMask = value; }

        public TargetingOriginSource TargetingOriginSource { get => _targetingOriginSource; set => _targetingOriginSource = value; }
        public Transform TargetingOrigin { get => _targetingOrigin; set => _targetingOrigin = value; }
        public float HalfAngle { get => _halfAngle; set => _halfAngle = value; }
        public Vector3 ManualOriginPos { get; set; } = Vector3.zero;
        public Vector3 ManualOriginDir { get; set; } = Vector3.zero;

        public Vector3 TargetingOriginPosition
        {
            get
            {
                return _targetingOriginSource switch
                {
                    TargetingOriginSource.Transform => _targetingOrigin.position,
                    TargetingOriginSource.Manual => ManualOriginPos,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public Vector3 TargetingOriginDir
        {
            get
            {
                return _targetingOriginSource switch
                {
                    TargetingOriginSource.Transform => _targetingOrigin.forward,
                    TargetingOriginSource.Manual => ManualOriginDir,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public bool ScoreTarget(IFlexTarget flexTarget, out float score)
        {
            /*
             * The angle between the targeting origin direction and the vector from origin to target is used as the
             * score. Since score is sorted by smallest, the target with the smallest angle difference is considered
             * as the best target. Only targets that are within the given half angle are accepted as valid targets, the
             * rest are filtered out
             */
            Vector3 oPos = TargetingOriginPosition;
            Vector3 oForward = TargetingOriginDir;
            Vector3 targetPos = flexTarget.TargetPosition;

            score = Vector3.Angle(targetPos - oPos, oForward);
            return score <= _halfAngle;
        }
    }
}
