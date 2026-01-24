using Cyclic.FlexTargeting.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyclic.FlexTargeting
{
    public interface IFlexTargetRepository<T> : IInternalFlexTargetRepository where T : IFlexTarget
    {
        /// <summary>
        /// Get the list of all targets currently in the repository
        /// </summary>
        List<T> Targets { get; }

        /// <summary>
        /// Add the target to the repository
        /// </summary>
        /// <param name="targetToAdd">The target to add</param>
        void AddTarget(T targetToAdd);

        /// <summary>
        /// Remove the target from the repository
        /// </summary>
        /// <param name="targetToRemove">The target to remove</param>
        void RemoveTarget(T targetToRemove);
    }

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
    public class FlexTargetRepository : MonoBehaviour
    {
        private readonly Dictionary<Type, IInternalFlexTargetRepository> _repositories = new();

        /// <summary>
        /// Return a repository containing targets of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of target to get a repository for.</typeparam>
        /// <returns>Returns the repository containing the targets and methods for adding and removing targets from the
        /// repository</returns>
        public IFlexTargetRepository<T> OfType<T>() where T : class, IFlexTarget
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                var newInternalRepository = new InternalFlexTargetRepository<T>();
                _repositories.Add(typeof(T), newInternalRepository);
            }

            return InternalFlexTargetRepository<T>.Instance;
        }

        /// <summary>
        /// Only returns the repository if the application is still running.
        /// </summary>
        /// <param name="repository">The repository containing targets of the specified type.</param>
        /// <typeparam name="T">The type of target to get a repository for.</typeparam>
        /// <returns>Returns true if the repository could be returned at this time. Returns false otherwise.</returns>
        public bool TryOfType<T>(out IFlexTargetRepository<T> repository) where T : class, IFlexTarget
        {
            if (!_isApplicationQuitting)
            {
                repository = OfType<T>();
                return true;
            }

            repository = null;
            return false;
        }

        public bool autoUnparentOnAwake = true;
        private static FlexTargetRepository s_instance = null;
        private bool _isApplicationQuitting = false;

        /// <summary>
        /// Return the singleton instance of the repository. If an instance hasn't been made yet then first an instance
        /// will be searched for in the active scenes. If an instance is still not found then a new one will be created
        /// and added to "Dont Destroy on Load".
        /// </summary>
        public static FlexTargetRepository Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindAnyObjectByType<FlexTargetRepository>(FindObjectsInactive.Exclude);
                    if (s_instance == null)
                    {
                        var go = new GameObject("FlexTargetRepository Auto-Generated");
                        s_instance = go.AddComponent<FlexTargetRepository>();
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
        public static bool TryGetInstance(out FlexTargetRepository outInstance)
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

            if (autoUnparentOnAwake)
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
                    Debug.Log($"Destroyed duplicate FlexTargetRepository because there is already an instance: " +
                              $"[{s_instance}]", this);
                }
            }
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuitting = true;

            foreach (var repository in _repositories.Values)
            {
                repository.Cleanup();
            }

            _repositories.Clear();
        }
    }
}
