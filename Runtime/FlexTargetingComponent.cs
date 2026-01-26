using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// A MonoBehavior version of flex targeting that comes with data of type <typeparamref name="TFlexData"/> and
    /// methods that simplify calling into <see cref="FlexTargetingCore"/>.
    /// </summary>
    /// <typeparam name="TFlexData">The flex targeting data type to use in all flex targeting functions.</typeparam>
    public class FlexTargetingComponent<TFlexData> : MonoBehaviour where TFlexData : IFlexData<IFlexTarget>, new()
    {
        [Header("Settings")]
        [SerializeField] private TFlexData _data = new();

        public bool DetermineBestTarget<TTarget>(out TTarget bestTarget) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out bestTarget);
        public bool DetermineBestTarget<TTarget>(out TTarget bestTarget, IReadOnlyList<TTarget> inputTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out bestTarget, inputTargets);
        public bool DetermineBestTarget<TTarget>(out TTarget bestTarget, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out bestTarget, targetFilter);
        public bool DetermineBestTarget<TTarget>(out TTarget bestTarget, IReadOnlyList<TTarget> inputTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out bestTarget, inputTargets, targetFilter);
        public bool DetermineBestTarget<TContext, TTarget>(TContext context, out TTarget bestTarget, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(context, _data, out bestTarget, targetFilter);
        public bool DetermineBestTarget<TContext, TTarget>(TContext context, out TTarget bestTarget, IReadOnlyList<TTarget> inputTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(context, _data, out bestTarget, inputTargets, targetFilter);

        public int DetermineBestTargets<TTarget>(in List<TTarget> bestTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in bestTargets);
        public int DetermineBestTargets<TTarget>(in List<TTarget> bestTargets, IReadOnlyList<TTarget> inputTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in bestTargets, inputTargets);
        public int DetermineBestTargets<TTarget>(in List<TTarget> bestTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in bestTargets, targetFilter);
        public int DetermineBestTargets<TTarget>(in List<TTarget> bestTargets, IReadOnlyList<TTarget> inputTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in bestTargets, inputTargets, targetFilter);
        public int DetermineBestTargets<TContext, TTarget>(TContext context, in List<TTarget> bestTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(context, _data, in bestTargets, targetFilter);
        public int DetermineBestTargets<TContext, TTarget>(TContext context, in List<TTarget> bestTargets, IReadOnlyList<TTarget> inputTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(context, _data, in bestTargets, inputTargets, targetFilter);
    }
}
