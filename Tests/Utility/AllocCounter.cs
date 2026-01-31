using System;
using UnityEngine.Profiling;

/// <summary>
/// See UnityEngine.TestTools.Constraints.AllocatingGCMemoryConstraint
/// and https://maingauche.games/devlog/20230504-counting-allocations-in-unity/
/// and https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/AllocCounter.cs
/// </summary>
public class AllocCounter
{
    private Recorder _rec;

    public AllocCounter()
    {
        _rec = Recorder.Get("GC.Alloc");

        /*
         * The recorder was created enabled, which means it captured the creation of the Recorder object itself, etc.
         * Disabling it flushes its data, so that we can retrieve the sample block count and have it correctly account
         * for these initial allocations.
         */
        _rec.enabled = false;

        #if !UNITY_WEBGL
        _rec.FilterToCurrentThread();
        #endif

        _rec.enabled = true;
    }

    public int Stop()
    {
        if (_rec == null) throw new InvalidOperationException("AllocCounter was not started.");

        _rec.enabled = false;

        #if !UNITY_WEBGL
        _rec.CollectFromAllThreads();
        #endif

        int result = _rec.sampleBlockCount;
        _rec = null;
        return result;
    }
}
