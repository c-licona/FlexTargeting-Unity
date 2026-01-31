// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using System;
using UnityEngine;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// <para>
    /// Implementations of this interface provide the data necessary to properly filter and sort a range of targets
    /// provided to the <see cref="FlexTargetingCore"/> methods. This interface declares the least amount of data
    /// required to execute the core targeting methods.
    /// </para>
    /// <para>
    /// The <see cref="ScoreTarget"/> method is the heart of this interface. It determines how each target is scored
    /// against each other in order to determine the "best" target.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The 2 most common implementations for this interface are DirectionTargetingData and
    /// RangeTargetingData which are located in the Builtin folder for this package.
    /// </remarks>
    public interface IFlexData<in TTarget> where TTarget : IFlexTarget
    {
        /// <summary>
        /// Get the origin of the targeter that is performing the targeting. This is typically used for determining
        /// whether a target is in range, and serving as the line-of-sight origin.
        /// </summary>
        Vector3 TargeterPosition { get; }

        /// <summary>
        /// The max range at which to consider targets for targeting. Any targets that are farther away than this range
        /// will be culled from the targeting process.
        /// </summary>
        float MaxRange { get; }

        /// <summary>
        /// <para>
        /// Distance value used to offset the line-of-sight (LOS) ray on the TARGETER side. This is helpful for making
        /// sure the LOS ray does not intersect with the targeters own geometry. See <see cref="LosLayerMask"/> if LOS
        /// is not needed.
        /// </para>
        /// <para>
        /// If the target is so close that it is within this radius, the LOS check automatically passes.
        /// </para>
        /// <remarks>
        /// Since it isn't very common to change this value, the default interface implementation sets this value to 0
        /// </remarks>
        /// </summary>
        float LosBufferRadius => 0.0f;

        /// <summary>
        /// A common aspect to Flex Targeting is using line-of-sight checks to ensure a target is valid. A ray is
        /// formed between the <see cref="TargeterPosition"/> and the flex target
        /// <see cref="IFlexTarget.TargetPosition"/> to evaluate LOS. This layer mask determines which layers can block
        /// the LOS ray. Use <see cref="FlexTargetingExtras.NoLayers"/> if LOS checks are not needed.
        /// </summary>
        LayerMask LosLayerMask { get; }

        /// <summary>
        /// The query trigger interaction setting to use for the line-of-sight (LOS) check. The most common setting to
        /// use (in my experience) is to ignore triggers, but this setting can be changed to suit your needs.
        /// </summary>
        QueryTriggerInteraction LosQueryTriggerInteraction => QueryTriggerInteraction.Ignore;

        /// <summary>
        /// The comparison method to use to determine how scores are sorted. Use
        /// <see cref="FlexTargetingExtras.SortBySmallestScore"/> in order to consider the target with the smallest score
        /// as the best target (this is the most common sorting method). Use
        /// <see cref="FlexTargetingExtras.SortByLargestScore"/> in order to consider the target with the largest score
        /// as the best target.
        /// </summary>
        Comparison<FlexTargetListItem> ScoreComparer => FlexTargetingExtras.SortBySmallestScore;

        /// <summary>
        /// When searching through the list of all potential targets, this filter decides whether the target should be
        /// considered for continuing through the process of finding the best target. The filter process typically gets
        /// progressively more and more complex, with this filter acting as the first line of defense. Because of that,
        /// this filter is usually the least computationally complex.
        /// </summary>
        /// <remarks>
        /// The default and most common implementation of this method is to check 2 things: First, check if the target
        /// is anywhere near the targeter. Second, check if the target itself is even valid.
        /// </remarks>
        /// <param name="target">The target to evaluate</param>
        /// <returns>Returns true if the target should continue through the determination process. Returns false if
        /// the target should be skipped.</returns>
        bool DoesPassFilter(TTarget target)
        {
            return Vector3.Distance(TargeterPosition, target.TargetPosition) <= MaxRange && target.IsTargetValid;
        }

        /// <summary>
        /// This is the heart of the <see cref="FlexTargetingCore"/> methods. Implementations of this method evaluate
        /// the provided <paramref name="target"/> and assign a score to that particular target. The score value
        /// is then used within the core methods to sort the targets by the best score. See <see cref="ScoreComparer"/>
        /// to change whether the targets have their scores sorted by smallest or largest score.
        /// </summary>
        /// <param name="target">The current target being scored by a <see cref="FlexTargetingCore"/> method.</param>
        /// <param name="score">The score to assign to the current <see cref="target"/></param>
        /// <returns>The return bool value is used to further filter out certain targets. If False is returned, the
        /// target will be culled out. If True is returned, the target will be considered as a potential target with
        /// the output score being used in the sorting process.</returns>
        bool ScoreTarget(TTarget target, out float score);
    }
}
