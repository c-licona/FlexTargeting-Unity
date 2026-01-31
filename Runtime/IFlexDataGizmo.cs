// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using UnityEngine;

namespace Cyclic.FlexTargeting
{
    public interface IFlexDataGizmo
    {
        /// <summary>
        /// Draw gizmos for flex data.
        /// </summary>
        /// <param name="targeterTransform">The transform of the MonoBehavior that will draw gizmos for the
        /// flex data.</param>
        public void DrawGizmos(Transform targeterTransform);
    }
}
