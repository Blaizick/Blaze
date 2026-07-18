using System;
using System.Runtime.CompilerServices;
using Blaze.Runtime.Utils;
using UnityEngine;

namespace Blaze.Runtime
{
    public abstract class Singleton<T> : ManagedBehaviour where T : Component
	{
        protected static bool s_IsQuitting = false;

        public static bool quittingCheck = false;

		private static T s_Instance;
		public static T Instance
		{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (Singleton<T>.s_Instance == null)
				{
                    if (!s_IsQuitting || !quittingCheck)
                    {
            			Singleton<T>.s_Instance = (T)((object)UnityEngine.Object.FindAnyObjectByType(typeof(T)));
            			if (Singleton<T>.s_Instance == null)
            			{
            				GameObject gameObject = new GameObject(typeof(T).Name);
            				Singleton<T>.s_Instance = gameObject.AddComponent<T>();
            				QDebugBase<InternalLogChannel>.VerboseLog(InternalLogChannel.System, $"Creating mono singleton of type {typeof(T).Name}", gameObject);
                			UnityEngine.Object.DontDestroyOnLoad(Singleton<T>.s_Instance.gameObject);
                        }
                    }
				}
				return Singleton<T>.s_Instance;
			}
		}

        protected virtual void OnApplicationQuit()
        {
            s_IsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_IsQuitting = true;
            }
        }

		public static bool Exists
		{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Singleton<T>.s_Instance != null;
			}
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ensure()
		{
			T instance = Singleton<T>.Instance;
		}
	}
}