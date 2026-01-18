using Cyclic.FlexTargeting;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BasicAPIEditorTests
{
    private class TestTarget : IFlexTarget
    {
        public bool IsTargetValid => true;
        public Vector3 TargetPosition { get; } = Vector3.forward;
        public float LosRadius => 0.0f;
    }

    private class TestData : IFlexData<IFlexTarget>
    {
        public Vector3 TargetingOriginPosition { get; } = Vector3.zero;
        public float MaxRange => 10.0f;
        public LayerMask LosLayerMask => FlexTargetingExtras.NoLayers;

        public bool ScoreTarget(IFlexTarget flexTarget, out float score)
        {
            score = Vector3.Distance(TargetingOriginPosition, flexTarget.TargetPosition);
            return score <= MaxRange;
        }
    }

    private TestData _testData;
    private TestTarget _testTarget;
    private List<TestTarget> _inputTargets;

    [SetUp]
    public void Setup()
    {
        _testData = new TestData();
        _testTarget = new TestTarget();
        _inputTargets = new List<TestTarget> { _testTarget };
    }

    [Test]
    public void BasicAPI_Method02_TargetFound()
    {
        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(_testData, out var finalTarget, _inputTargets);

        // Assert
        Assert.IsTrue(wasTargetFound);
        Assert.That(finalTarget, Is.EqualTo(_testTarget));
    }

    [Test]
    public void BasicAPI_Method04_TargetFound()
    {
        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(_testData, out var finalTarget, _inputTargets,
            target => target.IsTargetValid);

        // Assert
        Assert.IsTrue(wasTargetFound);
        Assert.That(finalTarget, Is.EqualTo(_testTarget));
    }

    [Test]
    public void BasicAPI_Method06_TargetFound()
    {
        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineFinalTarget(this, _testData, out var finalTarget,
            _inputTargets, (context, target) => context._testTarget == target);

        // Assert
        Assert.IsTrue(wasTargetFound);
        Assert.That(finalTarget, Is.EqualTo(_testTarget));
    }

    [Test]
    public void BasicAPI_Method08_TargetFound()
    {
        // Arrange
        List<TestTarget> finalTargets = new();

        // Act
        int numTargets = FlexTargetingCore.DetermineFinalTargets(_testData, finalTargets, _inputTargets);

        // Assert
        Assert.That(numTargets, Is.EqualTo(1));
        Assert.That(finalTargets[0], Is.EqualTo(_testTarget));
    }

    [Test]
    public void BasicAPI_Method10_TargetFound()
    {
        // Arrange
        List<TestTarget> finalTargets = new();

        // Act
        int numTargets = FlexTargetingCore.DetermineFinalTargets(_testData, finalTargets, _inputTargets,
            target => target.IsTargetValid);

        // Assert
        Assert.That(numTargets, Is.EqualTo(1));
        Assert.That(finalTargets[0], Is.EqualTo(_testTarget));
    }

    [Test]
    public void BasicAPI_Method12_TargetFound()
    {
        // Arrange
        List<TestTarget> finalTargets = new();

        // Act
        int numTargets = FlexTargetingCore.DetermineFinalTargets(this, _testData, finalTargets, _inputTargets,
            (context, target) => context._testTarget == target);

        // Assert
        Assert.That(numTargets, Is.EqualTo(1));
        Assert.That(finalTargets[0], Is.EqualTo(_testTarget));
    }
}
