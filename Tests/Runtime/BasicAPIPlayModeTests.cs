using Cyclic.FlexTargeting;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BasicAPIPlayModeTests
{
    private class TestTarget : FlexTargetComponent<TestTarget>
    {
        public override bool IsTargetValid => enabled;
        public override Vector3 TargetPosition { get; } = Vector3.forward;
        public override float LosBufferRadius => 0.0f;
    }

    private class TestData : IFlexData<IFlexTarget>
    {
        public Vector3 TargeterPosition { get; } = Vector3.zero;
        public float MaxRange => 10.0f;
        public LayerMask LosLayerMask => FlexTargetingExtras.NoLayers;

        public bool ScoreTarget(IFlexTarget target, out float score)
        {
            score = Vector3.Distance(TargeterPosition, target.TargetPosition);
            return score <= MaxRange;
        }
    }

    private TestData _testData;
    private TestTarget _testTarget;

    [SetUp]
    public void Setup()
    {
        _testData = new TestData();
        _testTarget = null;
    }

    [Test]
    public void BasicAPI_Method01_TargetFound()
    {
        // Arrange
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineBestTarget(_testData, out TestTarget bestTarget);

        // Assert
        Assert.IsTrue(wasTargetFound);
        Assert.That(bestTarget, Is.EqualTo(_testTarget));

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void BasicAPI_Method03_TargetFound()
    {
        // Arrange
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineBestTarget(_testData, out TestTarget bestTarget,
            target => target.IsTargetValid);

        // Assert
        Assert.IsTrue(wasTargetFound);
        Assert.That(bestTarget, Is.EqualTo(_testTarget));

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void BasicAPI_Method05_TargetFound()
    {
        // Arrange
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        bool wasTargetFound = FlexTargetingCore.DetermineBestTarget(this, _testData, out TestTarget bestTarget,
            (context, target) => context._testTarget == target);

        // Assert
        Assert.IsTrue(wasTargetFound);
        Assert.That(bestTarget, Is.EqualTo(_testTarget));

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void BasicAPI_Method07_TargetFound()
    {
        // Arrange
        List<TestTarget> bestTargets = new();
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        int numTargets = FlexTargetingCore.DetermineBestTargets(_testData, bestTargets);

        // Assert
        Assert.That(numTargets, Is.EqualTo(1));
        Assert.That(bestTargets[0], Is.EqualTo(_testTarget));

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void BasicAPI_Method09_TargetFound()
    {
        // Arrange
        List<TestTarget> bestTargets = new();
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        int numTargets = FlexTargetingCore.DetermineBestTargets(_testData, bestTargets, target => target.IsTargetValid);

        // Assert
        Assert.That(numTargets, Is.EqualTo(1));
        Assert.That(bestTargets[0], Is.EqualTo(_testTarget));

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void BasicAPI_Method11_TargetFound()
    {
        // Arrange
        List<TestTarget> bestTargets = new();
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        int numTargets = FlexTargetingCore.DetermineBestTargets(this, _testData, bestTargets,
            (context, target) => context._testTarget == target);

        // Assert
        Assert.That(numTargets, Is.EqualTo(1));
        Assert.That(bestTargets[0], Is.EqualTo(_testTarget));

        // Cleanup
        Object.DestroyImmediate(go);
    }
}
