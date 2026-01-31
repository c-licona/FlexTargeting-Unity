namespace Cyclic.FlexTargeting.Builtin
{
    /// <inheritdoc cref="RangeTargetingData"/>
    public class RangeTargeting : FlexTargetingComponent<RangeTargetingData>
    {
        private void OnDrawGizmosSelected() => _data.DrawGizmos(transform);
    }
}
