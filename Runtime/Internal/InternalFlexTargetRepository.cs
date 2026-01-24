using System.Collections.Generic;
using UnityEngine;

namespace Cyclic.FlexTargeting.Internal
{
    public interface IInternalFlexTargetRepository
    {
        void Cleanup();
    }

    public class InternalFlexTargetRepository<T> : IFlexTargetRepository<T>
        where T : class, IFlexTarget
    {
        private readonly List<T> _targets = new();

        public List<T> Targets => _targets;
        public void AddTarget(T targetToAdd) => _targets.Add(targetToAdd);
        public void RemoveTarget(T targetToRemove) => _targets.Remove(targetToRemove);

        public static InternalFlexTargetRepository<T> Instance { get; private set; } = null;

        public InternalFlexTargetRepository()
        {
            if (Instance != null)
            {
                Instance._targets.Clear();
                Debug.LogWarning($"Something is wrong, there is already an instance of InternalFlexTargetRepository " +
                                 $"of type {typeof(T).Name}. Cleaning up old instance and replacing with new.");
            }

            Instance = this;
        }

        public void Cleanup()
        {
            _targets.Clear();
            Instance = null;
        }
    }
}
