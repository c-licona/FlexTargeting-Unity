// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

namespace Cyclic.FlexTargeting.Builtin
{
    /// <inheritdoc cref="ComboTargetingData"/>
    public class ComboTargeting : FlexTargetingComponent<ComboTargetingData>
    {
        private void OnDrawGizmosSelected() => _data.DrawGizmos(transform);
    }
}
