namespace Cyclic.FlexTargeting.Builtin
{
    /// <inheritdoc cref="RangeTargetingData"/>
    public class RangeTargeting : FlexTargetingComponent<RangeTargetingData>
    {
        private void OnDrawGizmos() => _data.DrawGizmos(transform);
    }
}
