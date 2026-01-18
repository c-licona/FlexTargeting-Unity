using UnityEngine;

namespace Cyclic.FlexTargeting.Utility
{
    /// <summary>
    /// A generic singleton class that is instantiated and placed into DontDestroyOnLoad
    ///
    /// Based off of:
    /// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Singleton/PersistentSingleton.cs
    /// </summary>
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public bool shouldAutoUnparentOnAwake = true;

        protected static T instance = null;
        private static bool _isApplicationQuitting = false;

        public static bool IsApplicationQuitting => _isApplicationQuitting;
        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        /// <summary>
        /// Tries to get the current instance only if it currently exists and the application isn't quitting. This
        /// method call will not trigger the creation of a new instance. This is most useful if theres a possibility
        /// that the application is quitting.
        /// </summary>
        /// <param name="outInstance">The current singleton instance</param>
        /// <returns>Returns true if there is currently an instance and the application isn't quitting.</returns>
        public static bool TryGetInstance(out T outInstance)
        {
            if (HasInstance && !IsApplicationQuitting)
            {
                outInstance = instance;
                return true;
            }

            outInstance = null;
            return false;
        }

        public static T Instance
        {
            get
            {
                // do not attempt to create a new instance is the application is quitting
                if (instance == null && !IsApplicationQuitting)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (shouldAutoUnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                    Debug.Log("Destroyed another copy of this singleton", this);
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }
    }
}
