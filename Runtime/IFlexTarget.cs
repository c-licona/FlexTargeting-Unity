using UnityEngine;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// An interface defining the least amount of information a Flex Target needs to define in order for a target to
    /// be evaluated in the core targeting methods.
    /// </summary>
    public interface IFlexTarget
    {
        /// <summary>
        /// Determines whether this target should be considered as a potential target in any
        /// <see cref="FlexTargetingCore"/> methods.
        /// </summary>
        /// <value>
        ///     If true is returned, this target will continue through the determination process of
        ///     <see cref="FlexTargetingCore"/> methods. If false is returned, this target will be filtered out and not
        ///     be considered as a potential target.
        /// </value>
        bool IsTargetValid { get; }

        /// <summary>
        /// The position of this target, to be used for distance checks and line-of-sight checks
        /// </summary>
        Vector3 TargetPosition { get; }

        /// <summary>
        /// The line-of-sight(LOS) radius. During LOS checks, the raycast to this target will stop early by this radius
        /// distance. If you imagine a sphere with this radius around the target, the raycast will stop at the surface
        /// if this imaginary sphere. This is useful so that the LOS check doesn't hit the target itself.
        /// </summary>
        float LosRadius { get; }
    }
}
