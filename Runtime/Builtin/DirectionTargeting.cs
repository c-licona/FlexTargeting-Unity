namespace Cyclic.FlexTargeting.Builtin
{
    /// <inheritdoc cref="DirectionTargetingData"/>
    public class DirectionTargeting : FlexTargetingComponent<DirectionTargetingData>
    {
        private void OnDrawGizmosSelected() => _data.DrawGizmos(transform);
    }
}
