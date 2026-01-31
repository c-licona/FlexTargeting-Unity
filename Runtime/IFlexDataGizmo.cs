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
