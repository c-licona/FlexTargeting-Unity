// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using UnityEngine;

namespace Cyclic.FlexTargeting.Builtin
{
    /// <summary>
    /// Determine whether targeter origin values are sourced from a specific Transform or from values set manually
    /// through code.
    /// </summary>
    public enum TargeterOriginSource
    {
        [Tooltip("The targeter origin should come from the referenced Transform.")]
        Transform,
        [Tooltip("The targeter origin is set manually through code.")]
        Manual
    }
}
