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

        public bool DetermineFinalTarget<TTarget>(out TTarget target) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTarget(_data, out target);
        public bool DetermineFinalTarget<TTarget>(out TTarget target, in List<TTarget> inputTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTarget(_data, out target, in inputTargets);
        public bool DetermineFinalTarget<TTarget>(out TTarget target, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTarget(_data, out target, targetFilter);
        public bool DetermineFinalTarget<TTarget>(out TTarget target, in List<TTarget> inputTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTarget(_data, out target, in inputTargets, targetFilter);
        public bool DetermineFinalTarget<TContext, TTarget>(TContext context, out TTarget target, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTarget(context, _data, out target, targetFilter);
        public bool DetermineFinalTarget<TContext, TTarget>(TContext context, out TTarget target, in List<TTarget> inputTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTarget(context, _data, out target, in inputTargets, targetFilter);

        public int DetermineFinalTargets<TTarget>(in List<TTarget> finalTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTargets(_data, in finalTargets);
        public int DetermineFinalTargets<TTarget>(in List<TTarget> finalTargets, in List<TTarget> inputTargets) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTargets(_data, in finalTargets, in inputTargets);
        public int DetermineFinalTargets<TTarget>(in List<TTarget> finalTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTargets(_data, in finalTargets, targetFilter);
        public int DetermineFinalTargets<TTarget>(in List<TTarget> finalTargets, in List<TTarget> inputTargets, [NotNull] TargetFilter<TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTargets(_data, in finalTargets, in inputTargets, targetFilter);
        public int DetermineFinalTargets<TContext, TTarget>(TContext context, in List<TTarget> finalTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTargets(context, _data, in finalTargets, targetFilter);
        public int DetermineFinalTargets<TContext, TTarget>(TContext context, in List<TTarget> finalTargets, in List<TTarget> inputTargets, [NotNull] TargetFilter<TContext, TTarget> targetFilter) where TTarget : class, IFlexTarget => FlexTargetingCore.DetermineFinalTargets(context, _data, in finalTargets, in inputTargets, targetFilter);
    }
}
