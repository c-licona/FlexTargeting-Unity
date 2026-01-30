namespace Cyclic.FlexTargeting.Builtin
{
    /// <inheritdoc cref="ComboTargetingData"/>
    public class ComboTargeting : FlexTargetingComponent<ComboTargetingData>
    {
        private void OnDrawGizmosSelected() => _data.DrawGizmos(transform);
    }
}
