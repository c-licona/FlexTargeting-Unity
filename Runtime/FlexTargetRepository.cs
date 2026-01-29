using System.Collections.Generic;

namespace Cyclic.FlexTargeting
{
    public interface IFlexTargetRepository
    {
        /// <summary>
        /// Get the list of all targets currently in the repository
        /// </summary>
        IReadOnlyList<T> GetTargets<T>() where T : class, IFlexTarget;

        /// <summary>
        /// Add the target to the repository
        /// </summary>
        /// <param name="targetToAdd">The target to add</param>
        void AddTarget<T>(T targetToAdd) where T : class, IFlexTarget;

        /// <summary>
        /// Remove the target from the repository
        /// </summary>
        /// <param name="targetToRemove">The target to remove</param>
        void RemoveTarget<T>(T targetToRemove) where T : class, IFlexTarget;
    }

    public static class FlexTargetRepository
    {
        private static IFlexTargetRepository s_repository = new BuiltinFlexTargetRepositoryAccessor();

        public static IReadOnlyList<T> GetTargets<T>() where T : class, IFlexTarget
        {
            return s_repository.GetTargets<T>();
        }

        public static void AddTarget<T>(T targetToAdd) where T : class, IFlexTarget
        {
            s_repository.AddTarget(targetToAdd);
        }

        public static void RemoveTarget<T>(T targetToRemove) where T : class, IFlexTarget
        {
            s_repository.RemoveTarget(targetToRemove);
        }

        public static void ReplaceRepository(IFlexTargetRepository repository)
        {
            s_repository = repository;
        }
    }
}
