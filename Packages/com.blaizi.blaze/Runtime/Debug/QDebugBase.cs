using System;
using Blaze.Runtime.Utils;
using UnityEngine;

namespace Blaze.Runtime
{
    [Flags]
	public enum InternalLogChannel
	{
		None = 0,
		UI = 1,
		System = 2,
		Importers = 4,
		IO = 8,
		PlatformService = 16,
		CommandLine = 32,
		Native = 64,
		Mods = 128,
		FileSystemReporter = 256,
		Packer = 512,
		Serialization = 1024,
		Localization = 2048,
		HTTP = 4096,
		Settings = 8192,
		Rendering = 16384,
		Profile = 32768,
		Default = 32767,
		All = 65535
	}

	public static class InternalLogChannelExtension
	{
		public static bool IsFlagSet(this InternalLogChannel value, InternalLogChannel flag)
		{
			return flag == (value & flag);
		}

		public static InternalLogChannel SetFlag(this InternalLogChannel value, InternalLogChannel flag, bool on)
		{
			if (on)
			{
				return value | flag;
			}
			return value & ~flag;
		}

		public static InternalLogChannel ClearFlags(this InternalLogChannel value, InternalLogChannel flags)
		{
			return value.SetFlags(flags, false);
		}
	}

	public enum ErrorLevel
	{
		Info,
		Warning,
		Error
	}

    public class QDebugBase<T> : SingletonLite<QDebugBase<T>> where T : struct, IConvertible
	{
		public static bool Verbose
		{
			get
			{
				return SingletonLite<QDebugBase<T>>.Instance.m_Verbose;
			}
			set
			{
				SingletonLite<QDebugBase<T>>.Instance.m_Verbose = value;
			}
		}

		public static void Warn(T ll, string msg)
		{
			QDebugBase<T>.Log(ll, msg, null, ErrorLevel.Warning);
		}

		public static void Warn(T ll, string msg, global::UnityEngine.Object o)
		{
			QDebugBase<T>.Log(ll, msg, o, ErrorLevel.Warning);
		}

		public static void Error(T ll, string msg)
		{
			QDebugBase<T>.Log(ll, msg, null, ErrorLevel.Error);
		}

		public static void Error(T ll, string msg, global::UnityEngine.Object o)
		{
			QDebugBase<T>.Log(ll, msg, o, ErrorLevel.Error);
		}

		public static void VerboseWarn(T ll, string msg)
		{
			if (QDebugBase<T>.Verbose)
			{
				QDebugBase<T>.Log(ll, msg, null, ErrorLevel.Warning);
			}
		}

		public static void VerboseWarn(T ll, string msg, global::UnityEngine.Object obj)
		{
			if (QDebugBase<T>.Verbose)
			{
				QDebugBase<T>.Log(ll, msg, obj, ErrorLevel.Warning);
			}
		}

		public static void VerboseLog(T ll, string msg)
		{
			if (QDebugBase<T>.Verbose)
			{
				QDebugBase<T>.Log(ll, msg, null, ErrorLevel.Info);
			}
		}

		public static void VerboseLog(T ll, string msg, global::UnityEngine.Object obj)
		{
			if (QDebugBase<T>.Verbose)
			{
				QDebugBase<T>.Log(ll, msg, obj, ErrorLevel.Info);
			}
		}

		public static void VerboseError(T ll, string msg)
		{
			if (QDebugBase<T>.Verbose)
			{
				QDebugBase<T>.Log(ll, msg, null, ErrorLevel.Error);
			}
		}

		public static void VerboseError(T ll, string msg, global::UnityEngine.Object obj)
		{
			if (QDebugBase<T>.Verbose)
			{
				QDebugBase<T>.Log(ll, msg, obj, ErrorLevel.Error);
			}
		}

		public static void Log(T ll, string msg)
		{
			QDebugBase<T>.Log(ll, msg, null, ErrorLevel.Info);
		}

		public static void Log(T ll, string format, params object[] p)
		{
			QDebugBase<T>.Log(ll, string.Format(format, p), null, ErrorLevel.Info);
		}

		public static void Log(T ll, string msg, global::UnityEngine.Object obj)
		{
			QDebugBase<T>.Log(ll, msg, obj, ErrorLevel.Info);
		}

		public static void Log(T ll, string msg, ErrorLevel el)
		{
			QDebugBase<T>.Log(ll, msg, null, el);
		}

		public static void EnableChannels(T channels)
		{
			QDebugBase<T> instance = SingletonLite<QDebugBase<T>>.Instance;
			SingletonLite<QDebugBase<T>>.Instance.m_EnabledLogChannels = SingletonLite<QDebugBase<T>>.Instance.m_EnabledLogChannels.SetFlags(channels);
		}

		public static void DisableChannels(T channels)
		{
			QDebugBase<T> instance = SingletonLite<QDebugBase<T>>.Instance;
			SingletonLite<QDebugBase<T>>.Instance.m_EnabledLogChannels = SingletonLite<QDebugBase<T>>.Instance.m_EnabledLogChannels.ClearFlags(channels);
		}

		public static bool IsChannelEnabled(T channels)
		{
			QDebugBase<T> instance = SingletonLite<QDebugBase<T>>.Instance;
			return SingletonLite<QDebugBase<T>>.Instance.m_EnabledLogChannels.IsFlagSet(channels);
		}

		public static void Log(T ll, string msg, UnityEngine.Object obj, ErrorLevel el)
		{
			QDebugBase<T> instance = SingletonLite<QDebugBase<T>>.Instance;
			string text = "";
			if (QDebugBase<T>.s_IsInternal)
			{
				text = " - Internal";
			}
			if (SingletonLite<QDebugBase<T>>.Instance.m_EnabledLogChannels.IsFlagSet(ll) || el == ErrorLevel.Warning || el == ErrorLevel.Error)
			{
				if (el == ErrorLevel.Error)
				{
					Debug.LogError(string.Concat(new string[]
					{
						msg,
						"  [",
						ll.ToString(),
						text,
						"]"
					}), obj);
					return;
				}
				if (el == ErrorLevel.Warning)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						msg,
						"  [",
						ll.ToString(),
						text,
						"]"
					}), obj);
					return;
				}
				if (el == ErrorLevel.Info)
				{
					Debug.Log(string.Concat(new string[]
					{
						msg,
						"  [",
						ll.ToString(),
						text,
						"]"
					}), obj);
				}
			}
		}

		public static void DrawDirectionalArrow(Vector3 start, Vector3 end, float arrowSize, Color color, bool renderAtMidPoint, bool drawInXYplane)
		{
			Vector3 vector = (renderAtMidPoint ? ((start + end) * 0.5f) : end);
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, start - end);
			Debug.DrawLine(start, end, color);
			if (drawInXYplane)
			{
				Vector3 vector2 = end - start;
				vector2.z = 0f;
				vector2.Normalize();
				Vector3 vector3 = new Vector3(vector2.y, -vector2.x, 0f);
				Debug.DrawLine(vector, vector - (vector2 + vector3) * arrowSize, color);
				Debug.DrawLine(vector, vector - (vector2 - vector3) * arrowSize, color);
				return;
			}
			Debug.DrawLine(vector, vector + quaternion * new Vector3(arrowSize, arrowSize, arrowSize), color);
			Debug.DrawLine(vector, vector + quaternion * new Vector3(arrowSize, arrowSize, -arrowSize), color);
			Debug.DrawLine(vector, vector + quaternion * new Vector3(arrowSize, -arrowSize, arrowSize), color);
			Debug.DrawLine(vector, vector + quaternion * new Vector3(arrowSize, -arrowSize, -arrowSize), color);
		}

		public static void DrawBounds(Bounds bounds, Color col)
		{
			Vector3[] array = new Vector3[] { bounds.min, bounds.max };
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					Vector3 vector;
					vector.x = array[i].x;
					Vector3 vector2;
					vector2.x = array[i].x;
					vector.y = array[j].y;
					vector2.y = array[j].y;
					vector.z = array[0].z;
					vector2.z = array[1].z;
					Debug.DrawLine(vector, vector2, col);
					vector.y = array[i].y;
					vector2.y = array[i].y;
					vector.z = array[j].z;
					vector2.z = array[j].z;
					vector.x = array[0].x;
					vector2.x = array[1].x;
					Debug.DrawLine(vector, vector2, col);
					vector.z = array[i].z;
					vector2.z = array[i].z;
					vector.x = array[j].x;
					vector2.x = array[j].x;
					vector.y = array[0].y;
					vector2.y = array[1].y;
					Debug.DrawLine(vector, vector2, col);
				}
			}
		}

		public static string Precise(Vector3 v)
		{
			return string.Format("({0}, {1}, {2})", v.x, v.y, v.z);
		}

		public const bool EnableLogs = true;

		private bool m_Verbose;

		protected T m_EnabledLogChannels;

		private static bool s_IsInternal = typeof(T) == typeof(InternalLogChannel);
	}
}