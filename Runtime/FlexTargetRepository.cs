using System.Collections.Generic;

namespace Cyclic.FlexTargeting
{
    /// <summary>
    /// Target repositories provide access to flex targets of certain types. A custom repository can be used for the
    /// project by implementing this interface and replacing the global repository in <see cref="FlexTargetRepository"/>
    /// with the <see cref="FlexTargetRepository.ReplaceRepository"/> method.
    /// </summary>
    public interface IFlexTargetRepository
    {
        /// <summary>
        /// Get the list of all targets currently in the repository
        /// </summary>
        IReadOnlyList<T> GetTargets<T>() where T : class, IFlexTarget;

        /// <summary>
        /// Add the target to the repository
        /// </summary>
        /// <param name="targetToAdd">The flex target to add.</param>
        void AddTarget<T>(T targetToAdd) where T : class, IFlexTarget;

        /// <summary>
        /// Remove the target from the repository
        /// </summary>
        /// <param name="targetToRemove">The target to remove</param>
        void RemoveTarget<T>(T targetToRemove) where T : class, IFlexTarget;
    }

    /// <summary>
    /// Access to the current global flex target repository. This package provides a builtin repository, with the option
    /// to replace it with your own custom solution using the <see cref="ReplaceRepository"/> function. The repository
    /// should never be null and is used by the core methods that do not provide an input list of targets.
    /// </summary>
    public static class FlexTargetRepository
    {
        private static IFlexTargetRepository s_repository = new BuiltinRepository.BuiltinFlexTargetRepositoryAccessor();

        /// <summary>
        /// Get a list of all the targets of type <typeparamref name="T"/> from the repository.
        /// </summary>
        /// <typeparam name="T">The specific type of flex target to get a list of.</typeparam>
        /// <returns>Returns a readonly list, typically for iteration through the core methods.</returns>
        public static IReadOnlyList<T> GetTargets<T>() where T : class, IFlexTarget
        {
            return s_repository.GetTargets<T>();
        }

        /// <summary>
        /// Add the target of type <typeparamref name="T"/> to the repository. Ensure that this target is removed from
        /// the repository with <see cref="RemoveTarget"/>.
        /// </summary>
        /// <param name="targetToAdd">The target to add to the repository.</param>
        /// <typeparam name="T">The specific type of flex target to add to the repository.</typeparam>
        public static void AddTarget<T>(T targetToAdd) where T : class, IFlexTarget
        {
            s_repository.AddTarget(targetToAdd);
        }

        /// <summary>
        /// Remove the target of type <typeparamref name="T"/> from the repository.
        /// </summary>
        /// <param name="targetToRemove">The target to remove from the repository.</param>
        /// <typeparam name="T">The specific type of flex target to remove from the repository.</typeparam>
        public static void RemoveTarget<T>(T targetToRemove) where T : class, IFlexTarget
        {
            s_repository.RemoveTarget(targetToRemove);
        }

        /// <summary>
        /// Replace the current repository with a custom solution. Targets will be added and removed from your new
        /// repository. Target lists from your new repository will be retrieved by the core methods.
        /// </summary>
        /// <param name="repository">The new repository to use throughout the project.</param>
        public static void ReplaceRepository(IFlexTargetRepository repository)
        {
            s_repository = repository;
        }
    }
}
