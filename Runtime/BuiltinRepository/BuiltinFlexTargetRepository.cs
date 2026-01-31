// Copyright (c) 2026 Christian Licona. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyclic.FlexTargeting.BuiltinRepository
{
    /// <summary>
    /// <para>
    /// A repository of repositories. From this component, repositories of specific types of targets can be acquired,
    /// added to, and removed from. Use the <see cref="OfType{T}"/> method to get a repository of a specific target type
    /// </para>
    /// <para>
    /// Persistent Singleton pattern inspired by:
    /// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Singleton/PersistentSingleton.cs
    /// </para>
    /// </summary>
    public class BuiltinFlexTargetRepository : MonoBehaviour
    {
        private interface IInternalFlexTargetRepository
        {
            void Cleanup();
        }

        private readonly Dictionary<Type, IInternalFlexTargetRepository> _internalRepositories = new();

        /// <summary>
        /// Return an internal repository containing targets of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of target to get a repository for.</typeparam>
        /// <returns>Returns the repository containing the targets and methods for adding and removing targets from the
        /// repository</returns>
        public InternalFlexTargetRepository<T> OfType<T>() where T : class, IFlexTarget
        {
            if (!_internalRepositories.ContainsKey(typeof(T)))
            {
                var newInternalRepository = new InternalFlexTargetRepository<T>();
                _internalRepositories.Add(typeof(T), newInternalRepository);
            }

            return InternalFlexTargetRepository<T>.InternalInstance;
        }

        /// <summary>
        /// Only returns the internal repository if the application is still running.
        /// </summary>
        /// <param name="repository">The repository containing targets of the specified type.</param>
        /// <typeparam name="T">The type of target to get a repository for.</typeparam>
        /// <returns>Returns true if the repository could be returned at this time. Returns false otherwise.</returns>
        public bool TryOfType<T>(out InternalFlexTargetRepository<T> repository) where T : class, IFlexTarget
        {
            if (!_isApplicationQuitting)
            {
                repository = OfType<T>();
                return true;
            }

            repository = null;
            return false;
        }

        public bool AutoUnparentOnAwake = true;
        private static BuiltinFlexTargetRepository s_instance = null;
        private bool _isApplicationQuitting = false;

        /// <summary>
        /// Return the singleton instance of the repository. If an instance hasn't been made yet then first an instance
        /// will be searched for in the active scenes. If an instance is still not found then a new one will be created
        /// and added to "Dont Destroy on Load".
        /// </summary>
        public static BuiltinFlexTargetRepository Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindAnyObjectByType<BuiltinFlexTargetRepository>(FindObjectsInactive.Exclude);
                    if (s_instance == null)
                    {
                        var go = new GameObject("BuiltinFlexTargetRepository Auto-Generated");
                        s_instance = go.AddComponent<BuiltinFlexTargetRepository>();
                        DontDestroyOnLoad(go);
                    }
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Try to get the singleton instance of this class only if the instance exists and the application is still
        /// running. This method will not cause a new instance to be made if one doesn't exist.
        /// </summary>
        /// <param name="outInstance">The instance if there is one available at this time. Will be null if the method
        /// returns false.</param>
        /// <returns>Returns true if the instance was returned. Returns false otherwise.</returns>
        public static bool TryGetInstance(out BuiltinFlexTargetRepository outInstance)
        {
            if (s_instance != null && !s_instance._isApplicationQuitting)
            {
                outInstance = s_instance;
                return true;
            }

            outInstance = null;
            return false;
        }

        private void Awake()
        {
            if (!Application.isPlaying)
                return;

            if (AutoUnparentOnAwake)
                transform.SetParent(null);

            if (s_instance == null)
            {
                s_instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (s_instance != this)
                {
                    Destroy(gameObject);
                    Debug.Log($"Destroyed duplicate BuiltinFlexTargetRepository because there is already an instance: " +
                              $"[{s_instance}]", this);
                }
            }
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuitting = true;

            foreach (var repository in _internalRepositories.Values)
            {
                repository.Cleanup();
            }

            _internalRepositories.Clear();
        }

        public class InternalFlexTargetRepository<T> : IInternalFlexTargetRepository
            where T : class, IFlexTarget
        {
            private readonly List<T> _targets = new();

            public List<T> Targets => _targets;
            public void AddTarget(T targetToAdd) => _targets.Add(targetToAdd);
            public void RemoveTarget(T targetToRemove) => _targets.Remove(targetToRemove);

            public static InternalFlexTargetRepository<T> InternalInstance { get; private set; } = null;

            public InternalFlexTargetRepository()
            {
                if (InternalInstance != null)
                {
                    InternalInstance._targets.Clear();
                    Debug.LogWarning($"Something is wrong, there is already an instance of InternalFlexTargetRepository " +
                                     $"of type {typeof(T).Name}. Cleaning up old instance and replacing with new.");
                }

                InternalInstance = this;
            }

            public void Cleanup()
            {
                _targets.Clear();
                InternalInstance = null;
            }
        }
    }
}
