using Cyclic.FlexTargeting;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TargetingTestsWithOneTarget
{
    private class TestTarget : IFlexTarget
    {
        public bool FilterResult { get; set; } = true;

        public bool IsTargetValid { get; set; } = true;
        public Vector3 TargetPosition { get; } = Vector3.forward;
        public float LosBufferRadius => 0.0f;
    }

    private class TestData : IFlexData<IFlexTarget>
    {
        public Vector3 TargeterPosition { get; } = Vector3.zero;
        public float MaxRange { get; set; } = 10.0f;
        public LayerMask LosLayerMask => FlexTargetingExtras.NoLayers;

        public bool ScoreTarget(IFlexTarget flexTarget, out float score)
        {
            score = Vector3.Distance(TargeterPosition, flexTarget.TargetPosition);
            return score <= MaxRange;
        }
    }

    private TestData _testData;
    private TestTarget _testTarget;
    private List<TestTarget> _inputTargets;
    private bool _contextFilterValue;

    [SetUp]
    public void Setup()
    {
        _testData = new TestData();
        _testTarget = new TestTarget();
        _inputTargets = new List<TestTarget> { _testTarget };
    }

    [TestCase(10.0f, true)]
    [TestCase(0.5f, false)]
    public void Method2_IsTargetInRange(float maxRange, bool shouldFindTarget)
    {
        // Arrange
        _testData.MaxRange = maxRange;

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(_testData, out var finalTarget, _inputTargets);

        // Assert
        Assert.That(wasTargetFound, Is.EqualTo(shouldFindTarget));
        if (shouldFindTarget)
            Assert.IsNotNull(finalTarget);
        else
            Assert.IsNull(finalTarget);
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void Method2_IsTargetValid(bool isTargetValid, bool shouldFindTarget)
    {
        // Arrange
        _testTarget.IsTargetValid = isTargetValid;

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(_testData, out var finalTarget, _inputTargets);

        // Assert
        Assert.That(wasTargetFound, Is.EqualTo(shouldFindTarget));
        if (shouldFindTarget)
            Assert.IsNotNull(finalTarget);
        else
            Assert.IsNull(finalTarget);
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void Method4_TestFilter(bool shouldFilterPass, bool shouldFindTarget)
    {
        // Arrange
        _testTarget.FilterResult = shouldFilterPass;

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(_testData, out var finalTarget, _inputTargets,
            target => target.FilterResult);

        // Assert
        Assert.That(wasTargetFound, Is.EqualTo(shouldFindTarget));
        if (shouldFindTarget)
            Assert.IsNotNull(finalTarget);
        else
            Assert.IsNull(finalTarget);
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void Method6_TestContext(bool shouldFilterPass, bool shouldFindTarget)
    {
        // Arrange
        _testTarget.FilterResult = shouldFilterPass;
        _contextFilterValue = shouldFilterPass;

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(this, _testData, out var finalTarget,
            _inputTargets, (context, target) => context._contextFilterValue && target.FilterResult);

        // Assert
        Assert.That(wasTargetFound, Is.EqualTo(shouldFindTarget));
        if (shouldFindTarget)
            Assert.IsNotNull(finalTarget);
        else
            Assert.IsNull(finalTarget);
    }
}
