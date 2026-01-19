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
    /// against each other in order to determine the "final" or "best" target.
    /// </para>
    /// </summary>
    /// <example>
    /// The 2 most common examples of implementations for this interface are DirectionTargetingData and
    /// RangeTargetingData which are located in the Builtin folder for this package.
    /// </example>
    public interface IFlexData<in TTarget> where TTarget : IFlexTarget
    {
        /// <summary>
        /// Get the origin of the targeter that is performing the targeting. Only targets that are close enough to this
        /// position value (as determined by <see cref="MaxRange"/>) will be considered in the targeting process.
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
        /// The comparison method to use to determine how scores are sorted. Use
        /// <see cref="FlexTargetingExtras.SortBySmallestScore"/> in order to consider the target with the smallest score
        /// as the best target (this is the most common sorting method). Use
        /// <see cref="FlexTargetingExtras.SortByLargestScore"/> in order to consider the target with the largest score
        /// as the best target.
        /// </summary>
        Comparison<FlexTargetListItem> ScoreComparer => FlexTargetingExtras.SortBySmallestScore;

        /// <summary>
        /// This is the heart of the <see cref="FlexTargetingCore"/> methods. Implementations of this method evaluate
        /// the provided <paramref name="flexTarget"/> and assign a score to that particular target. The score value
        /// is then used within the core methods to sort the targets by the best score. See <see cref="ScoreComparer"/>
        /// to change whether the targets have their scores sorted by smallest or largest score.
        /// </summary>
        /// <param name="flexTarget">The current target being scored by a <see cref="FlexTargetingCore"/> method.</param>
        /// <param name="score">The score to assign to the current <see cref="flexTarget"/></param>
        /// <returns>The return bool value is used to filter out certain targets. If False is returned, the target will
        /// be culled out. If True is returned, the target will be considered as a potential target with the output
        /// score being used in the sorting process.</returns>
        bool ScoreTarget(TTarget flexTarget, out float score);
    }
}
