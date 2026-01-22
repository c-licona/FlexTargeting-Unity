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

        public bool DetermineBestTarget<TTarget>(out TTarget target) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out target);
        public bool DetermineBestTarget<TTarget>(out TTarget target, in List<TTarget> inputTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out target, in inputTargets);
        public bool DetermineBestTarget<TTarget>(out TTarget target, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out target, targetFilter);
        public bool DetermineBestTarget<TTarget>(out TTarget target, in List<TTarget> inputTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(_data, out target, in inputTargets, targetFilter);
        public bool DetermineBestTarget<TContext, TTarget>(TContext context, out TTarget target, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(context, _data, out target, targetFilter);
        public bool DetermineBestTarget<TContext, TTarget>(TContext context, out TTarget target, in List<TTarget> inputTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTarget(context, _data, out target, in inputTargets, targetFilter);

        public int DetermineBestTargets<TTarget>(in List<TTarget> finalTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in finalTargets);
        public int DetermineBestTargets<TTarget>(in List<TTarget> finalTargets, in List<TTarget> inputTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in finalTargets, in inputTargets);
        public int DetermineBestTargets<TTarget>(in List<TTarget> finalTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in finalTargets, targetFilter);
        public int DetermineBestTargets<TTarget>(in List<TTarget> finalTargets, in List<TTarget> inputTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(_data, in finalTargets, in inputTargets, targetFilter);
        public int DetermineBestTargets<TContext, TTarget>(TContext context, in List<TTarget> finalTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(context, _data, in finalTargets, targetFilter);
        public int DetermineBestTargets<TContext, TTarget>(TContext context, in List<TTarget> finalTargets, in List<TTarget> inputTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineBestTargets(context, _data, in finalTargets, in inputTargets, targetFilter);
    }
}
