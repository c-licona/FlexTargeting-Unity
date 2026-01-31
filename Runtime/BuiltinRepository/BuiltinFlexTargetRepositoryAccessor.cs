// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Generic;

namespace Cyclic.FlexTargeting.BuiltinRepository
{
    public class BuiltinFlexTargetRepositoryAccessor : IFlexTargetRepository
    {
        public IReadOnlyList<T> GetTargets<T>() where T : class, IFlexTarget
        {
            return BuiltinFlexTargetRepository.Instance.OfType<T>().Targets;
        }

        public void AddTarget<T>(T targetToAdd) where T : class, IFlexTarget
        {
            BuiltinFlexTargetRepository.Instance.OfType<T>().AddTarget(targetToAdd);
        }

        public void RemoveTarget<T>(T targetToRemove) where T : class, IFlexTarget
        {
            if (BuiltinFlexTargetRepository.TryGetInstance(out var repository) &&
                repository.TryOfType<T>(out var targetRepository))
            {
                targetRepository.RemoveTarget(targetToRemove);
            }
        }
    }
}
