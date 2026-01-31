// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cyclic.FlexTargeting
{
    public struct FlexTargetListItem
    {
        public IFlexTarget FlexTarget;
        public float Score;
    }

    /// <summary>
    /// A delegate that evaluates the given <paramref name="target"/> and returns whether the target should or should
    /// not be considered as a potential target in the DetermineBestTarget methods.
    /// </summary>
    /// <typeparam name="TTarget">The type of flex target to consider. This type is contravariant, allowing for more
    /// flexibility in the delegates that can be used.</typeparam>
    public delegate bool TargetFilter<in TTarget>(TTarget target) where TTarget : IFlexTarget;

    /// <summary>
    /// <para>
    /// A delegate that evaluates the given <paramref name="target"/> and returns whether the target should or should
    /// not be considered as a potential target in the DetermineBestTarget methods.
    /// </para>
    /// <para>
    /// A context reference is passed in from which additional data can be accessed when evaluating the target. The
    /// most common usage of this delegate is to pass in <c>this</c> as the context to not allocate heap memory
    /// because of an implicit capture of <c>this</c>.
    /// See: https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#zero-allocations-with-delegates and
    /// https://www.jacksondunstan.com/articles/3765
    /// </para>
    /// </summary>
    /// <typeparam name="TContext">The context from which this delegate is being called. Commonly "this" is passed in
    /// as the context.</typeparam>
    /// <typeparam name="TTarget">The type of flex target to consider. This type is contravariant, allowing for more
    /// flexibility in the delegates that can be used.</typeparam>
    public delegate bool TargetFilter<in TContext, in TTarget>(TContext context, TTarget target) where TTarget : IFlexTarget;

    /// <summary>
    /// Contains some useful extras
    /// </summary>
    public static class FlexTargetingExtras
    {
        /// <summary>
        /// Layer mask constant to select no layers.
        /// </summary>
        public const int NoLayers = 0;

        public static readonly Comparison<FlexTargetListItem> SortBySmallestScore = (x, y) => x.Score.CompareTo(y.Score);
        public static readonly Comparison<FlexTargetListItem> SortByLargestScore = (x, y) => y.Score.CompareTo(x.Score);
    }

    /// <summary>
    /// Contains the core methods of the flex targeting package: DetermineBestTarget and DetermineBestTargets
    /// </summary>
    public static class FlexTargetingCore
    {
        private static readonly List<FlexTargetListItem> s_potentialTargets = new();
        private static readonly RaycastHit[] s_losResults = new RaycastHit[1];
        private static readonly TargetFilter<IFlexTarget> s_nullFilter = _ => true;

        #region DetermineBestTarget

        /// <inheritdoc cref="DetermineBestTarget{TTarget}(IFlexData{TTarget}, out TTarget, IReadOnlyList{TTarget}, TargetFilter{TTarget})"/>
        public static bool DetermineBestTarget<TTarget>([NotNull] IFlexData<TTarget> data,
            out TTarget bestTarget)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTarget(data,
                out bestTarget,
                inputTargets: FlexTargetRepository.GetTargets<TTarget>(),
                s_nullFilter);
        }

        /// <inheritdoc cref="DetermineBestTarget{TTarget}(IFlexData{TTarget}, out TTarget, IReadOnlyList{TTarget}, TargetFilter{TTarget})"/>
        public static bool DetermineBestTarget<TTarget>([NotNull] IFlexData<TTarget> data,
            out TTarget bestTarget,
            IReadOnlyList<TTarget> inputTargets)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTarget(data,
                out bestTarget,
                inputTargets,
                s_nullFilter);
        }

        /// <inheritdoc cref="DetermineBestTarget{TTarget}(IFlexData{TTarget}, out TTarget, IReadOnlyList{TTarget}, TargetFilter{TTarget})"/>
        public static bool DetermineBestTarget<TTarget>([NotNull] IFlexData<TTarget> data,
            out TTarget bestTarget,
            [NotNull] TargetFilter<TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTarget(data,
                out bestTarget,
                inputTargets: FlexTargetRepository.GetTargets<TTarget>(),
                targetFilter);
        }

        /// <summary>
        /// <para>
        /// Filters and scores the targets in the <paramref name="inputTargets"/> list and outputs the best target to
        /// <paramref name="bestTarget"/>. The data and scoring method that is used to determine the best target is
        /// contained within the provided <paramref name="data"/>. An optional <paramref name="targetFilter"/> can be
        /// provided to further filter out valid targets.
        /// </para>
        /// <remarks>
        /// When using a <paramref name="targetFilter"/> it is important to be aware of what does and doesn't allocate
        /// memory on the heap when working with delegates. Use the overload of this method that includes a context
        /// object to properly use an instance or static method without allocating memory. See this link for more info:
        /// https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#zero-allocations-with-delegates
        /// </remarks>
        /// </summary>
        /// <param name="data">A structure with the data and scoring method to use for sorting and ultimately
        ///     determining the best target to return.</param>
        /// <param name="bestTarget">The best target that scores the best among the rest of the potential targets.
        ///     Will be null when no valid target was found.</param>
        /// <param name="inputTargets">An collection of targets that will be filtered, scored, and
        ///     sorted to determine which is the best target to return.</param>
        /// <param name="targetFilter">A delegate that can further process each input target to determine whether it
        ///     should be considered for the final determination.</param>
        /// <typeparam name="TTarget">The type of flex target to be considered.</typeparam>
        /// <returns>Returns true if a valid final target was returned into <paramref name="bestTarget"/>. Returns
        /// false if no valid target was found, in which case <paramref name="bestTarget"/> will be null.</returns>
        public static bool DetermineBestTarget<TTarget>([NotNull] IFlexData<TTarget> data,
            out TTarget bestTarget,
            IReadOnlyList<TTarget> inputTargets,
            [NotNull] TargetFilter<TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            Assert.IsNotNull(data);
            Assert.IsNotNull(targetFilter);
            s_potentialTargets.Clear();

            // filter valid targets and score them
            for (int i = 0; i < inputTargets.Count; i++)
            {
                var target = inputTargets[i];

                if (data.DoesPassFilter(target) &&
                    targetFilter.Invoke(target) &&
                    data.ScoreTarget(target, out float score))
                {
                    s_potentialTargets.Add(new FlexTargetListItem{ FlexTarget = target, Score = score });
                }
            }

            s_potentialTargets.Sort(data.ScoreComparer);

            // return the first target that isn't LOS blocked
            foreach (var targetItem in s_potentialTargets)
            {
                IFlexTarget target = targetItem.FlexTarget;

                if (!IsTargetLosBlocked(target, data.TargeterPosition, data.LosBufferRadius, data.LosLayerMask,
                        data.LosQueryTriggerInteraction))
                {
                    bestTarget = target as TTarget;
                    return true;
                }
            }

            bestTarget = null;
            return false;
        }

        #endregion

        #region DetermineBestTarget with context

        /// <inheritdoc cref="DetermineBestTarget{TContext, TTarget}(TContext, IFlexData{TTarget}, out TTarget, IReadOnlyList{TTarget}, TargetFilter{TContext, TTarget})"/>
        public static bool DetermineBestTarget<TContext, TTarget>(TContext context,
            [NotNull] IFlexData<TTarget> data,
            out TTarget bestTarget,
            [NotNull] TargetFilter<TContext, TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTarget(context,
                data,
                out bestTarget,
                inputTargets: FlexTargetRepository.GetTargets<TTarget>(),
                targetFilter);
        }

        /// <summary>
        /// <para>
        /// Filters and scores the targets in the <paramref name="inputTargets"/> list and outputs the best target to
        /// <paramref name="bestTarget"/>. The data and scoring method that is used to determine the best target is
        /// contained within the provided <paramref name="data"/>. An optional <paramref name="targetFilter"/> can be
        /// provided to further filter out valid targets.
        /// </para>
        /// <remarks>
        /// When using a <paramref name="targetFilter"/> it is important to be aware of what does and doesn't allocate
        /// memory on the heap when working with delegates. Use this method which includes a context object to properly
        /// use an instance or static method without allocating memory. See this link for more info:
        /// https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#zero-allocations-with-delegates
        /// </remarks>
        /// <example>
        /// <code>
        /// // an example in which the context is not passed in. this results in a delegate allocation!
        /// FlexTargetingCore.DetermineBestTarget(data, out bestTarget, inputTargets, (target) => SomeMethod(target));
        /// // an example in which the context IS passed in. this results in no allocation.
        /// FlexTargetingCore.DetermineBestTarget(context: this, data, out bestTarget, inputTargets, (context, target) => context.SomeMethod(target));
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="context">The context object to pass along into the <paramref name="targetFilter"/> in
        ///     order to avoid allocating heap memory with delegates. Typically this parameter is simply the calling
        ///     object: <c>this</c></param>
        /// <param name="data">A structure with the data and scoring method to use for sorting and ultimately
        ///     determining the best target to return.</param>
        /// <param name="bestTarget">The best target that scores the best among the rest of the potential targets.
        ///     Will be null when no valid target was found.</param>
        /// <param name="inputTargets">An enumerable collection of targets that will be filtered, scored, and
        ///     sorted to determine which is the best target to return.</param>
        /// <param name="targetFilter">A delegate that can further process each input target to determine whether it
        ///     should be considered for the final determination.</param>
        /// <typeparam name="TContext">The type of the <paramref name="context"/> object passed in which will be
        /// passed along into the <paramref name="targetFilter"/></typeparam>
        /// <typeparam name="TTarget">The type of flex target to be considered.</typeparam>
        /// <returns>Returns true if a valid final target was returned into <paramref name="bestTarget"/>. Returns
        /// false if no valid target was found, in which case <paramref name="bestTarget"/> will be null.</returns>
        public static bool DetermineBestTarget<TContext, TTarget>(TContext context,
            [NotNull] IFlexData<TTarget> data,
            out TTarget bestTarget,
            IReadOnlyList<TTarget> inputTargets,
            [NotNull] TargetFilter<TContext, TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            Assert.IsNotNull(data);
            Assert.IsNotNull(targetFilter);
            s_potentialTargets.Clear();

            // filter valid targets and score them
            for (int i = 0; i < inputTargets.Count; i++)
            {
                var target = inputTargets[i];

                if (data.DoesPassFilter(target) &&
                    targetFilter.Invoke(context, target) &&
                    data.ScoreTarget(target, out float score))
                {
                    s_potentialTargets.Add(new FlexTargetListItem{ FlexTarget = target, Score = score });
                }
            }

            s_potentialTargets.Sort(data.ScoreComparer);

            // return the first target that isn't LOS blocked
            foreach (var targetItem in s_potentialTargets)
            {
                IFlexTarget target = targetItem.FlexTarget;

                if (!IsTargetLosBlocked(target, data.TargeterPosition, data.LosBufferRadius, data.LosLayerMask,
                        data.LosQueryTriggerInteraction))
                {
                    bestTarget = target as TTarget;
                    return true;
                }
            }

            bestTarget = null;
            return false;
        }

        #endregion

        #region DetermineBestTargets

        /// <inheritdoc cref="DetermineBestTargets{TTarget}(IFlexData{TTarget}, ICollection{TTarget}, IReadOnlyList{TTarget}, TargetFilter{TTarget})" />
        public static int DetermineBestTargets<TTarget>([NotNull] IFlexData<TTarget> data,
            ICollection<TTarget> bestTargets)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTargets(data,
                bestTargets,
                inputTargets: FlexTargetRepository.GetTargets<TTarget>(),
                s_nullFilter);
        }

        /// <inheritdoc cref="DetermineBestTargets{TTarget}(IFlexData{TTarget}, ICollection{TTarget}, IReadOnlyList{TTarget}, TargetFilter{TTarget})" />
        public static int DetermineBestTargets<TTarget>([NotNull] IFlexData<TTarget> data,
            ICollection<TTarget> bestTargets,
            IReadOnlyList<TTarget> inputTargets)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTargets(data,
                bestTargets,
                inputTargets,
                s_nullFilter);
        }

        /// <inheritdoc cref="DetermineBestTargets{TTarget}(IFlexData{TTarget}, ICollection{TTarget}, IReadOnlyList{TTarget}, TargetFilter{TTarget})" />
        public static int DetermineBestTargets<TTarget>([NotNull] IFlexData<TTarget> data,
            ICollection<TTarget> bestTargets,
            [NotNull] TargetFilter<TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTargets(data,
                bestTargets,
                inputTargets: FlexTargetRepository.GetTargets<TTarget>(),
                targetFilter);
        }

        /// <summary>
        /// <para>
        /// Filters and scores the targets in the <paramref name="inputTargets"/> list and outputs the passing targets
        /// to the <paramref name="bestTargets"/> collection. The data and scoring method that is used to determine the
        /// best targets is contained within the provided <paramref name="data"/> structure. Every target that passes
        /// all conditions will be returned in the order of best scoring to worst. An optional
        /// <paramref name="targetFilter"/> can be provided to further filter out valid targets.
        /// </para>
        /// <remarks>
        /// When using a <paramref name="targetFilter"/> it is important to be aware of what does and doesn't allocate
        /// memory on the heap when working with delegates. Use the overload of this method that includes a context
        /// object to properly use an instance or static method without allocating memory. See this link for more info:
        /// https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#zero-allocations-with-delegates
        /// </remarks>
        /// </summary>
        /// <param name="data">A structure with the data and scoring method to use for sorting and ultimately
        ///     determining the best targets to return</param>
        /// <param name="bestTargets">The collection that will contain the targets that passed all conditions. These
        ///     targets will be added to the collection in the order of best scoring to worst.</param>
        /// <param name="inputTargets">A list of targets that will be filtered, scored, and sorted to determine which
        ///     are the best targets to return.</param>
        /// <param name="targetFilter">A delegate that can further process each input target to determine whether it
        ///     should be considered as a valid target to return.</param>
        /// <typeparam name="TTarget">The type of flex target to be considered.</typeparam>
        /// <returns>Returns the number of targets that were added to the <paramref name="bestTargets"/> collection.
        /// </returns>
        public static int DetermineBestTargets<TTarget>([NotNull] IFlexData<TTarget> data,
            ICollection<TTarget> bestTargets,
            IReadOnlyList<TTarget> inputTargets,
            [NotNull] TargetFilter<TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            Assert.IsNotNull(data);
            Assert.IsNotNull(targetFilter);
            bestTargets.Clear();
            s_potentialTargets.Clear();

            // filter valid targets and score them
            for (int i = 0; i < inputTargets.Count; i++)
            {
                var target = inputTargets[i];

                if (data.DoesPassFilter(target) &&
                    targetFilter.Invoke(target) &&
                    data.ScoreTarget(target, out float score) &&
                    !IsTargetLosBlocked(target, data.TargeterPosition, data.LosBufferRadius, data.LosLayerMask,
                        data.LosQueryTriggerInteraction))
                {
                    s_potentialTargets.Add(new FlexTargetListItem{ FlexTarget = target, Score = score });
                }
            }

            s_potentialTargets.Sort(data.ScoreComparer);

            // send the filtered targets to the final targets list
            int numTargetsAdded = 0;
            foreach (var targetItem in s_potentialTargets)
            {
                bestTargets.Add(targetItem.FlexTarget as TTarget);
                numTargetsAdded++;
            }

            return numTargetsAdded;
        }

        #endregion

        #region DetermineBestTargets with context

        /// <inheritdoc cref="DetermineBestTargets{TContext, TTarget}(TContext, IFlexData{TTarget}, ICollection{TTarget}, IReadOnlyList{TTarget}, TargetFilter{TContext, TTarget})" />
        public static int DetermineBestTargets<TContext, TTarget>(TContext context,
            [NotNull] IFlexData<TTarget> data,
            ICollection<TTarget> bestTargets,
            [NotNull] TargetFilter<TContext, TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            return DetermineBestTargets(context,
                data,
                bestTargets,
                inputTargets: FlexTargetRepository.GetTargets<TTarget>(),
                targetFilter);
        }

        /// <summary>
        /// <para>
        /// Filters and scores the targets in the <paramref name="inputTargets"/> list and outputs the passing targets
        /// to the <paramref name="bestTargets"/> collection. The data and scoring method that is used to determine the
        /// best targets is contained within the provided <paramref name="data"/> structure. Every target that passes
        /// all conditions will be returned in the order of best scoring to worst. An optional
        /// <paramref name="targetFilter"/> can be provided to further filter out valid targets.
        /// </para>
        /// <remarks>
        /// When using a <paramref name="targetFilter"/> it is important to be aware of what does and doesn't allocate
        /// memory on the heap when working with delegates. Use this method which includes a context object to properly
        /// use an instance or static method without allocating memory. See this link for more info:
        /// https://github.com/KyryloKuzyk/PrimeTween?tab=readme-ov-file#zero-allocations-with-delegates
        /// </remarks>
        /// <example>
        /// An example of how to utilize context:
        /// <code>
        /// // context is not passed in resulting in a delegate allocation every function call!
        /// FlexTargetingCore.DetermineBestTargets(data, bestTargets, inputTargets, (target) => FilterMethod(target));
        /// // an example in which the context IS passed in. this results in no allocation.
        /// FlexTargetingCore.DetermineBestTargets(context: this, data, bestTargets, inputTargets, (context, target) => context.FilterMethod(target));
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="context">The context object to pass along into the <paramref name="targetFilter"/> in
        ///     order to avoid allocating heap memory with delegates. Typically this parameter is simply the calling
        ///     object: <c>this</c></param>
        /// <param name="data">A structure with the data and scoring method to use for sorting and ultimately
        ///     determining the best target to return.</param>
        /// <param name="bestTargets">The collection that will contain the targets that passed all conditions. These
        ///     targets will be added to the collection in the order of best scoring to worst.</param>
        /// <param name="inputTargets">A list of targets that will be filtered, scored, and sorted to determine which
        ///     are the best targets to return.</param>
        /// <param name="targetFilter">A delegate that can further process each input target to determine whether it
        ///     should be considered as a valid target to return.</param>
        /// <typeparam name="TContext">The type of the <paramref name="context"/> object passed in which will be
        /// passed along into the <paramref name="targetFilter"/></typeparam>
        /// <typeparam name="TTarget">The type of flex target to be considered.</typeparam>
        /// <returns>Returns the number of targets that were added to the <paramref name="bestTargets"/> collection.
        /// </returns>
        public static int DetermineBestTargets<TContext, TTarget>(TContext context,
            [NotNull] IFlexData<TTarget> data,
            ICollection<TTarget> bestTargets,
            IReadOnlyList<TTarget> inputTargets,
            [NotNull] TargetFilter<TContext, TTarget> targetFilter)
            where TTarget : class, IFlexTarget
        {
            Assert.IsNotNull(data);
            Assert.IsNotNull(targetFilter);
            bestTargets.Clear();
            s_potentialTargets.Clear();

            // filter valid targets and score them
            for (int i = 0; i < inputTargets.Count; i++)
            {
                var target = inputTargets[i];

                if (data.DoesPassFilter(target) &&
                    targetFilter.Invoke(context, target) &&
                    data.ScoreTarget(target, out float score) &&
                    !IsTargetLosBlocked(target, data.TargeterPosition, data.LosBufferRadius, data.LosLayerMask,
                        data.LosQueryTriggerInteraction))
                {
                    s_potentialTargets.Add(new FlexTargetListItem{ FlexTarget = target, Score = score });
                }
            }

            s_potentialTargets.Sort(data.ScoreComparer);

            // send the filtered targets to the final targets list
            int numTargetsAdded = 0;
            foreach (var targetItem in s_potentialTargets)
            {
                bestTargets.Add(targetItem.FlexTarget as TTarget);
                numTargetsAdded++;
            }

            return numTargetsAdded;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if there is anything blocking line of sight between the origin and the target.
        /// </summary>
        /// <param name="target">The target we are checking</param>
        /// <param name="targeterPosition">The position of the targeter</param>
        /// <param name="losBufferRadius">The LOS buffer radius of the targeter</param>
        /// <param name="losLayerMask">The layer mask that will block LOS</param>
        /// <param name="losQueryTriggerInteraction">The QueryTriggerInteraction to use for the LOS check</param>
        /// <returns>Returns true if there is anything in between the raycast between the origin and the target.
        /// Returns false otherwise. Also returns false if the data has specified not to care about LOS.</returns>
        private static bool IsTargetLosBlocked(IFlexTarget target,
            Vector3 targeterPosition,
            float losBufferRadius,
            LayerMask losLayerMask,
            QueryTriggerInteraction losQueryTriggerInteraction)
        {
            // if the LOS layer mask is set to nothing then automatically return as not blocked
            if (losLayerMask.value == FlexTargetingExtras.NoLayers) return false;

            Vector3 origin = targeterPosition;
            Vector3 oToTarget = target.TargetPosition - origin;
            float rayDistance = oToTarget.magnitude - target.LosBufferRadius - losBufferRadius;
            // if a target is so close that the distance is non-positive than we consider it not LOS blocked
            if (rayDistance <= Mathf.Epsilon) return false;

            int numHits = Physics.RaycastNonAlloc(
                origin: origin + (oToTarget.normalized * losBufferRadius),
                direction: oToTarget.normalized,
                s_losResults,
                rayDistance,
                losLayerMask.value,
                losQueryTriggerInteraction);

            // if there are any hits then the target is LOS blocked
            return numHits > 0;
        }

        #endregion
    }
}
