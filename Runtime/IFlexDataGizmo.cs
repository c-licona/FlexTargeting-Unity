using UnityEngine;

namespace Cyclic.FlexTargeting
{
    public interface IFlexDataGizmo
    {
        /// <summary>
        /// Draw gizmos for flex data.
        /// </summary>
        /// <param name="transform">The transform of the MonoBehavior that will draw gizmos for the flex data.</param>
        public void DrawGizmos(Transform transform);
    }
}
