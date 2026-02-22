// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using Cyclic.FlexTargeting;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AdapterTests
{
    private class TestNonTarget
    {
        private float _dummyValue;
        public Vector3 Position => Vector3.zero;
    }

    private class TestNonTarget2
    {
        private float _dummyValue;
        public Vector3 Position => Vector3.zero;
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
    private TestNonTarget _testNonTarget;
    private TestNonTarget2 _testNonTarget2;
    private List<TestNonTarget> _inputTargets;
    private List<TestNonTarget2> _inputTargets2;
    private readonly int _numIterations = 8;
    private int _firstAllocCount = 0;
    private bool _noAllocOnRemainingIterations = true;

    [SetUp]
    public void Setup()
    {
        _testData = new TestData();
        _testNonTarget = new();
        _testNonTarget2 = new();
        _inputTargets = new List<TestNonTarget> { _testNonTarget };
        _inputTargets2 = new List<TestNonTarget2> { _testNonTarget2 };

        _firstAllocCount = 0;
        _noAllocOnRemainingIterations = true;
    }

    [Test]
    public void Adapter_List_NoAlloc()
    {
        // Arrange
        FlexTargetAdapter.SetAdapterFuncs<TestNonTarget>(
            isTargetValidFunc: target => true,
            targetPositionFunc: target => target.Position,
            losBufferRadiusFunc: target => 0.0f);

        var adaptedTargets = _inputTargets.AdaptWithoutFuncs();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget(_testData, out FlexTargetAdapter<TestNonTarget> _, adaptedTargets);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);
    }

    [Test]
    public void Adapter_ListWithFuncs_NoAlloc()
    {
        // Arrange
        var adaptedTargets = _inputTargets2.Adapt(
            isTargetValidFunc: target => true,
            targetPositionFunc: target => target.Position,
            losBufferRadiusFunc: target => 0.0f);

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget(_testData, out FlexTargetAdapter<TestNonTarget2> _, adaptedTargets);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);
    }
}
