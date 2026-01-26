using Cyclic.FlexTargeting;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unfortunately I can't use test cases with different numbers of iterations because the first time allocations and
/// other static allocations do not get reset between test cases.
///
/// These tests also only work properly after a domain reload. If this group of tests is ran twice in a row, the
/// subsequent tests will fail because of static functions getting cached.
/// </summary>
public class AllocationTests
{
    private class TestTarget : IFlexTarget
    {
        public bool IsTargetValid => true;
        public Vector3 TargetPosition { get; } = Vector3.forward;
        public float LosBufferRadius => 0.0f;

        public int specialNumber = 7;
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
    private List<TestTarget> _inputTargets;
    private static readonly int s_specialNumber = 7;
    private readonly int _numIterations = 8;
    private int _firstAllocCount = 0;
    private bool _noAllocOnRemainingIterations = true;
    private bool _allocOnAllIterations = true;
    private bool _noAllocOnAllIterations = true;
    private TargetFilter<TestTarget> _cachedInstanceFunction;
    private static TargetFilter<TestTarget> s_cachedStaticFunction;
    private static TargetFilter<TestTarget> s_cachedLambdaFunction;
    private static TargetFilter<TestTarget> s_cachedAnonymousMethod;

    private bool TargetFilter(TestTarget target)
    {
        return target == _testTarget;
    }

    private static bool StaticTargetFilter(TestTarget target)
    {
        return target.specialNumber == s_specialNumber;
    }

    [SetUp]
    public void Setup()
    {
        _testData = new TestData();
        _testTarget = new TestTarget();
        _inputTargets = new List<TestTarget> { _testTarget };

        _firstAllocCount = 0;
        _noAllocOnRemainingIterations = true;
        _allocOnAllIterations = true;
        _noAllocOnAllIterations = true;

        _cachedInstanceFunction = TargetFilter;
        s_cachedStaticFunction = StaticTargetFilter;
        s_cachedLambdaFunction = target => target.specialNumber == 7;
        s_cachedAnonymousMethod = delegate(TestTarget target) { return target.specialNumber == 7; };
    }

    // Unfortunately I can't use test cases with different numbers of iterations because the first time allocations and
    // other static allocations do not get reset between test cases

    [Test]
    public void Allocations_BasicTest_BestTarget_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);
    }

    [Test]
    public void Allocations_BasicTest_BestTargets_NoAlloc()
    {
        // Arrange
        List<TestTarget> bestTargets = new();

        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTargets<TestTarget>(_testData, bestTargets, _inputTargets);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations);
    }

    /// <summary>
    /// Using an instance function for the target filter which allocates every time the targeting function is called
    /// </summary>
    [Test]
    public void Allocations_FilterWithInstanceFunction_Alloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets, TargetFilter);
            int allocCount = ac.Stop();

            _allocOnAllIterations &= allocCount != 0;
            if (i == 0) _firstAllocCount = allocCount;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_allocOnAllIterations); // instance functions cause allocation every time
    }

    /// <summary>
    /// Using an instance function with targeting, however in this case we make use of context in order to access the
    /// instance function without incurring the cost of calling the instance function directly
    ///
    /// Unfortunately I can't use test cases with different numbers of iterations because the lambda static allocation
    /// does not get reset between test cases
    /// </summary>
    [Test]
    public void Allocations_FilterWithInstanceFunction_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget(this, _testData, out TestTarget _, _inputTargets,
                (context, target) => context.TargetFilter(target));
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations); // using context and lambda avoids instance function alloc cost
    }

    /// <summary>
    /// Using static functions directly also allocates every call.
    /// See: https://www.jacksondunstan.com/articles/3765#comment-752925
    /// </summary>
    [Test]
    public void Allocations_FilterWithStaticFunction_Alloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets, StaticTargetFilter);
            int allocCount = ac.Stop();

            _allocOnAllIterations &= allocCount != 0;
            if (i == 0) _firstAllocCount = allocCount;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_allocOnAllIterations); // static function always allocates
    }

    /// <summary>
    /// Calling the static function from within a lambda is ok. The lambda is cached the first time
    /// See: https://www.jacksondunstan.com/articles/3765#comment-752925
    /// </summary>
    [Test]
    public void Allocations_FilterWithStaticFunction_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets,
                target => StaticTargetFilter(target));
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations); // lambda is cached and calling static function from within is ok
    }

    /// <summary>
    /// Lambdas only allocate the first time and then are cached. They no longer allocate afterwards
    /// </summary>
    [Test]
    public void Allocations_FilterWithLambdaFunction_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets,
                target => target.specialNumber == 7);
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations); // lambda is cached and no longer allocates
    }

    /// <summary>
    /// Anonymous methods only allocate once and then are cached. They no longer allocate afterwards
    /// </summary>
    [Test]
    public void Allocations_FilterWithAnonymousMethod_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets,
                delegate(TestTarget target) { return target.specialNumber == 7; });
            int allocCount = ac.Stop();

            if (i == 0) _firstAllocCount = allocCount;
            else _noAllocOnRemainingIterations &= allocCount == 0;
        }

        // Assert
        Assert.NotZero(_firstAllocCount); // first time always allocates
        Assert.IsTrue(_noAllocOnRemainingIterations); // anonymous method is cached and no longer allocates
    }

    [Test]
    public void Allocations_CachedInstanceFunction_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets, _cachedInstanceFunction);
            int allocCount = ac.Stop();

            _noAllocOnAllIterations &= allocCount == 0;
        }

        // Assert
        Assert.IsTrue(_noAllocOnAllIterations);
    }

    [Test]
    public void Allocations_CachedStaticFunction_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets, s_cachedStaticFunction);
            int allocCount = ac.Stop();

            _noAllocOnAllIterations &= allocCount == 0;
        }

        // Assert
        Assert.IsTrue(_noAllocOnAllIterations);
    }

    [Test]
    public void Allocations_CachedLambdaFunction_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets, s_cachedLambdaFunction);
            int allocCount = ac.Stop();

            _noAllocOnAllIterations &= allocCount == 0;
        }

        // Assert
        Assert.IsTrue(_noAllocOnAllIterations);
    }

    [Test]
    public void Allocations_CachedAnonymousMethod_NoAlloc()
    {
        // Act
        for (int i = 0; i < _numIterations; i++)
        {
            var ac = new AllocCounter();
            FlexTargetingCore.DetermineBestTarget<TestTarget>(_testData, out _, _inputTargets, s_cachedAnonymousMethod);
            int allocCount = ac.Stop();

            _noAllocOnAllIterations &= allocCount == 0;
        }

        // Assert
        Assert.IsTrue(_noAllocOnAllIterations);
    }
}
