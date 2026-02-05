// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

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
        /// <remarks>
        /// The most common return value here is "true" when the target is registered OnEnable, or returning the
        /// "enabled" state of the MonoBehavior if registering OnAwake. Of course for custom cases this should return
        /// whatever you need it to.
        /// </remarks>
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
        /// <para>
        /// Distance value used to offset the line-of-sight (LOS) ray on the TARGET side. This is helpful for making
        /// sure the LOS ray does not intersect with the targets geometry. During LOS checks, the raycast to this
        /// target will stop early by this radius distance. If you imagine a sphere with this radius around the target,
        /// the raycast will stop at the surface if this imaginary sphere. Make sure to set this value so that the
        /// imaginary sphere sufficiently envelops most of the target.
        /// </para>
        /// <para>
        /// If the targeter is so close that it is within this radius, the LOS check automatically passes.
        /// </para>
        /// </summary>
        float LosBufferRadius { get; }
    }
}
