using Cyclic.FlexTargeting.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cyclic.FlexTargeting
{
    public interface IFlexTargetRepository<T> : IDisposable where T : IFlexTarget
    {
        List<T> Targets();
        void AddTarget(T targetToAdd);
        void RemoveTarget(T targetToRemove);
    }

    public class FlexTargetRepository : PersistentSingleton<FlexTargetRepository>
    {
        private Dictionary<Type, IDisposable> _repositories = new();

        public IFlexTargetRepository<T> OfType<T>() where T : class, IFlexTarget
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                IFlexTargetRepository<T> repository = new InternalFlexTargetRepository<T>();
                _repositories.Add(typeof(T), repository);
            }

            return InternalFlexTargetRepository<T>.Instance;
        }

        public bool TryOfType<T>(out IFlexTargetRepository<T> repository) where T : class, IFlexTarget
        {
            if (!IsApplicationQuitting)
            {
                repository = OfType<T>();
                return true;
            }

            repository = null;
            return false;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            foreach (var repository in _repositories)
            {
                repository.Value.Dispose();
            }
        }
    }

    public class InternalFlexTargetRepository<T> : IFlexTargetRepository<T>
        where T : class, IFlexTarget
    {
        private List<T> _targets = new();

        public List<T> Targets() => _targets;
        public void AddTarget(T targetToAdd) => _targets.Add(targetToAdd);
        public void RemoveTarget(T targetToRemove) => _targets.Remove(targetToRemove);

        public void Dispose()
        {
            _isApplicationQuitting = true;
            _targets.Clear();
        }

        private static InternalFlexTargetRepository<T> instance = null;
        private static bool _isApplicationQuitting = false;

        public static bool IsApplicationQuitting => _isApplicationQuitting;
        public static bool HasInstance => instance != null;

        /// <summary>
        /// Tries to get the current instance only if it currently exists and the application isn't quitting. This
        /// method call will not trigger the creation of a new instance. This is most useful if theres a possibility
        /// that the application is quitting.
        /// </summary>
        /// <param name="outInstance">The current singleton instance</param>
        /// <returns>Returns true if there is currently an instance and the application isn't quitting.</returns>
        public static bool TryGetInstance(out InternalFlexTargetRepository<T> outInstance)
        {
            if (HasInstance && !IsApplicationQuitting)
            {
                outInstance = instance;
                return true;
            }

            outInstance = null;
            return false;
        }

        public static InternalFlexTargetRepository<T> Instance
        {
            get
            {
                // do not attempt to create a new instance is the application is quitting
                if (instance == null && !IsApplicationQuitting)
                {
                    instance = new InternalFlexTargetRepository<T>();
                }

                return instance;
            }
        }

        public InternalFlexTargetRepository() => InitializeSingleton();

        private void InitializeSingleton()
        {
            if (_isApplicationQuitting) return;

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                if (instance != this)
                {
                    // Destroy(gameObject);
                    this.Dispose();
                    Debug.Log("Destroyed another copy of this singleton");
                }
            }
        }
    }
}
