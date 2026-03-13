// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

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
        /// <typeparam name="TTarget">The type of flex target to get a list of.</typeparam>
        /// <returns>Returns a read only list of targets of only type <typeparamref name="TTarget"/></returns>
        IReadOnlyList<TTarget> GetTargets<TTarget>() where TTarget : class, IFlexTarget;

        /// <summary>
        /// Add the target to the repository.
        /// </summary>
        /// <param name="targetToAdd">The target to add to the repository.</param>
        /// <typeparam name="TTarget">The specific type of flex target that will be added.</typeparam>
        void AddTarget<TTarget>(TTarget targetToAdd) where TTarget : class, IFlexTarget;

        /// <summary>
        /// Removes the target from the repository.
        /// </summary>
        /// <param name="targetToRemove">The target to remove from the repository.</param>
        /// <typeparam name="TTarget">The specific type of flex target that will be removed.</typeparam>
        void RemoveTarget<TTarget>(TTarget targetToRemove) where TTarget : class, IFlexTarget;

        /// <summary>
        /// Executes any cleanup functionality on the repository that should be run when the repository is being
        /// replaced.
        /// </summary>
        void Cleanup();
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
        /// Get a list of all the targets of type <typeparamref name="TTarget"/> from the repository.
        /// </summary>
        /// <typeparam name="TTarget">The specific type of flex target to get a list of.</typeparam>
        /// <returns>Returns a readonly list, typically for iteration through the core methods.</returns>
        public static IReadOnlyList<TTarget> GetTargets<TTarget>() where TTarget : class, IFlexTarget
        {
            return s_repository.GetTargets<TTarget>();
        }

        /// <summary>
        /// Add the target of type <typeparamref name="TTarget"/> to the repository. Ensure that this target is removed
        /// from the repository with <see cref="RemoveTarget"/>.
        /// </summary>
        /// <param name="targetToAdd">The target to add to the repository.</param>
        /// <typeparam name="TTarget">The specific type of flex target to add to the repository.</typeparam>
        public static void AddTarget<TTarget>(TTarget targetToAdd) where TTarget : class, IFlexTarget
        {
            s_repository.AddTarget(targetToAdd);
        }

        /// <summary>
        /// Remove the target of type <typeparamref name="TTarget"/> from the repository.
        /// </summary>
        /// <param name="targetToRemove">The target to remove from the repository.</param>
        /// <typeparam name="TTarget">The specific type of flex target to remove from the repository.</typeparam>
        public static void RemoveTarget<TTarget>(TTarget targetToRemove) where TTarget : class, IFlexTarget
        {
            s_repository.RemoveTarget(targetToRemove);
        }

        /// <summary>
        /// Replace the current repository with a custom solution. Targets will be added and removed from your new
        /// repository. Target lists from your new repository will be retrieved by the core methods.
        /// </summary>
        /// <param name="repository">The new repository to use throughout the project.</param>
        /// <param name="shouldCleanupOldRepository">Whether the <see cref="IFlexTargetRepository.Cleanup"/> method
        /// should be ran on the repository that is being replaced.</param>
        public static void ReplaceRepository(IFlexTargetRepository repository, bool shouldCleanupOldRepository = true)
        {
            if (shouldCleanupOldRepository)
            {
                s_repository.Cleanup();
            }

            s_repository = repository;
        }
    }
}
