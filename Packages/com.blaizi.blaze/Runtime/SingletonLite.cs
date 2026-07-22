
namespace Blaze.Runtime
{
    public abstract class SingletonLite<T> where T : new()
	{
		public static T Instance
		{
			get
			{
				if (SingletonLite<T>.s_Instance == null)
				{
					SingletonLite<T>.s_Instance = new T();
				}
				return SingletonLite<T>.s_Instance;
			}
			set
			{
				s_Instance = value;
			}
		}

		public static bool Exists
		{
			get
			{
				return SingletonLite<T>.s_Instance != null;
			}
		}

		public static void Ensure()
		{
			T instance = SingletonLite<T>.Instance;
		}

		private static T s_Instance;
	}
}