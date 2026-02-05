namespace Cyclic.FlexTargeting
{
    public static class FlexTargetingTooltips
    {
        public const string TargeterOriginSource = "Determine where targeter origin values are sourced from. Either from a referenced transform or manually through code.";
        public const string TargeterOrigin = "The transform to source targeter origin values from.";
        public const string MaxRange = "The maximum range at which targets will be considered.";
        public const string LosRaySize = "The line-of-sight check can use either a raycast or a spherecast. If this value is 0, a raycast will be used. Otherwise, this value will represent the radius of the sphere that is cast.";
        public const string TargeterLosBufferRadius = "Defines a radius around the origin point from which line-of-sight checks will begin. Use a non-zero value if there is a chance that LOS checks will be blocked by the targeter itself.";
        public const string HalfAngle = "Targets will be considered if the angle between [targeter direction] and [vector from origin to target] is less than or equal to this angle (degree) value. At 180 degrees, all targets in range will be considered.";
        public const string LosLayerMask = "The layers that will block the line-of-sight raycasts. Set this to 'Nothing' to ignore LOS checks.";
        public const string LosQueryTriggerInteraction = "Determine whether the line-of-sight raycasts hit triggers.";
        public const string RangeBandWidth = "The size of each band that divides the space between the targeter and max range.";
        public const string TargetLosBufferRadius = "Defines a radius around the target point at which line-of-sight checks will stop. Use a non-zero value if there is a chance that the LOS checks will be blocked by the target itself.";
        public const string TargetPoint = "The center point of the target. The vector between the targeter and this point will be used for distance checks and the line-of-sight checks.";
    }
}
