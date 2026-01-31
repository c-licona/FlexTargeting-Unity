// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using Cyclic.FlexTargeting;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These tests ensure there are no hidden allocations going on with the methods that utilize the target repository
/// </summary>
public class AllocationTestsPlayMode
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
    private readonly int _numIterations = 8;
    private int _firstAllocCount = 0;
    private bool _noAllocOnRemainingIterations = true;

    [SetUp]
    public void Setup()
    {
        _testData = new TestData();
        _testTarget = null;
        _firstAllocCount = 0;
        _noAllocOnRemainingIterations = true;
    }

    [Test]
    public void Allocations_Method01_NoAlloc()
    {
        // Arrange
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget(_testData, out TestTarget _);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Allocations_Method03_NoAlloc()
    {
        // Arrange
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget(_testData, out TestTarget _, target => target.IsTargetValid);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Allocations_Method05_NoAlloc()
    {
        // Arrange
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget(this, _testData, out TestTarget _,
                (context, target) => context._testTarget == target);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Allocations_Method07_NoAlloc()
    {
        // Arrange
        List<TestTarget> bestTargets = new();
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTargets(_testData, bestTargets);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Allocations_Method09_NoAlloc()
    {
        // Arrange
        List<TestTarget> bestTargets = new();
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTargets(_testData, bestTargets, target => target.IsTargetValid);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Allocations_Method11_NoAlloc()
    {
        // Arrange
        List<TestTarget> bestTargets = new();
        var go = new GameObject();
        _testTarget = go.AddComponent<TestTarget>();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTargets(this, _testData, bestTargets,
                (context, target) => context._testTarget == target);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);

        // Cleanup
        Object.DestroyImmediate(go);
    }
}
