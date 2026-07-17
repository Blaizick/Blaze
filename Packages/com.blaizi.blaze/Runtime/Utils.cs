using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Blaze.Runtime.Utils
{
    public static class ListUtils
    {
        public static List<T> Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }
    }

    public static class ListExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Shuffle<T>(this List<T> list)
        {
            return ListUtils.Shuffle(list);
        }
    }

    public static class CurrencyFormatUtils
    {
        public const string Thousand = "K.";
        public const string Million = "M.";
        public const string Billion = "B.";

        public static string FormatCurrency(float count, string format = "0", bool round = false)
        {
            float thousands = count / 1000;
            float millions = count / 1000000;
            float billions = count / 1000000000;

            if (billions >= 1f)
            {
                if (round)
                {
                    return $"{((int)billions).ToString(format)} {Billion}";
                }
                else
                {
                    return $"{billions.ToString(format)} {Billion}";
                }
            }
            if (millions >= 1f)
            {
                if (round)
                {
                    return $"{((int)millions).ToString(format)} {Million}";
                }
                else
                {
                    return $"{millions.ToString(format)} {Million}";
                }
            }
            if (thousands >= 1f)
            {
                if (round)
                {
                    return $"{((int)thousands).ToString(format)} {Thousand}";
                }
                else
                {
                    return $"{thousands.ToString(format)} {Thousand}";
                }
            }

            return count.ToString();
        }
    }

    public static class EnumExtensions
	{
		private static void CheckEnumWithFlags<T>()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
			}
			if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
			{
				throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
			}
		}

		public static bool IsFlagSet<T>(this T value, T flag) where T : struct, IConvertible
		{
			EnumExtensions.CheckEnumWithFlags<T>();
			long num = value.ToInt64(CultureInfo.InvariantCulture);
			long num2 = flag.ToInt64(CultureInfo.InvariantCulture);
			return (num & num2) != 0L;
		}

		public static IEnumerable<T> GetFlags<T>(this T value) where T : struct, IConvertible
		{
			EnumExtensions.CheckEnumWithFlags<T>();
			foreach (T t in Enum.GetValues(typeof(T)).Cast<T>())
			{
				if (value.IsFlagSet(t))
				{
					yield return t;
				}
			}
			yield break;
		}

		public static T SetFlags<T>(this T value, T flags, bool on) where T : struct, IConvertible
		{
			EnumExtensions.CheckEnumWithFlags<T>();
			long num = value.ToInt64(CultureInfo.InvariantCulture);
			long num2 = flags.ToInt64(CultureInfo.InvariantCulture);
			if (on)
			{
				num |= num2;
			}
			else
			{
				num &= ~num2;
			}
			return (T)((object)Enum.ToObject(typeof(T), num));
		}

		public static T SetFlags<T>(this T value, T flags) where T : struct, IConvertible
		{
			return value.SetFlags(flags, true);
		}

		public static T ClearFlags<T>(this T value, T flags) where T : struct, IConvertible
		{
			return value.SetFlags(flags, false);
		}

		public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct, IConvertible
		{
			EnumExtensions.CheckEnumWithFlags<T>();
			long num = 0L;
			foreach (T t in flags)
			{
				long num2 = t.ToInt64(CultureInfo.InvariantCulture);
				num |= num2;
			}
			return (T)((object)Enum.ToObject(typeof(T), num));
		}

		public static int Index<T>(this T en) where T : struct, IConvertible
		{
			return en.Index(null);
		}

		public static int Index<T>(this T en, string category) where T : struct, IConvertible
		{
			EnumPositionAttribute[] array = typeof(T).GetField(en.ToString()).GetCustomAttributes(typeof(EnumPositionAttribute), false) as EnumPositionAttribute[];
			if (array == null || array.Length == 0)
			{
				return -1;
			}
			EnumPositionAttribute enumPositionAttribute = null;
			if (string.IsNullOrEmpty(category))
			{
				enumPositionAttribute = array[0];
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].category == category)
					{
						enumPositionAttribute = array[i];
						break;
					}
				}
			}
			if (enumPositionAttribute == null)
			{
				return -1;
			}
			return enumPositionAttribute.index;
		}

		public static string Name<T>(this T en) where T : struct, IConvertible
		{
			return en.Name(null);
		}

		public static string Name<T>(this T en, string category) where T : struct, IConvertible
		{
			EnumPositionAttribute[] array = typeof(T).GetField(en.ToString()).GetCustomAttributes(typeof(EnumPositionAttribute), false) as EnumPositionAttribute[];
			if (array == null || array.Length == 0)
			{
				return null;
			}
			EnumPositionAttribute enumPositionAttribute = null;
			if (string.IsNullOrEmpty(category))
			{
				enumPositionAttribute = array[0];
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].category == category)
					{
						enumPositionAttribute = array[i];
						break;
					}
				}
			}
			if (enumPositionAttribute == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(enumPositionAttribute.name))
			{
				return enumPositionAttribute.name;
			}
			return en.ToString();
		}

		public static string Category<T>(this T en) where T : struct, IConvertible
		{
			return en.Category(null);
		}

		public static string Category<T>(this T en, string category) where T : struct, IConvertible
		{
			EnumPositionAttribute[] array = typeof(T).GetField(en.ToString()).GetCustomAttributes(typeof(EnumPositionAttribute), false) as EnumPositionAttribute[];
			if (array == null || array.Length == 0)
			{
				return null;
			}
			EnumPositionAttribute enumPositionAttribute = null;
			if (string.IsNullOrEmpty(category))
			{
				enumPositionAttribute = array[0];
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].category == category)
					{
						enumPositionAttribute = array[i];
						break;
					}
				}
			}
			if (enumPositionAttribute == null)
			{
				return null;
			}
			return enumPositionAttribute.category;
		}
	}

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class EnumPositionAttribute : Attribute
	{
		public int index
		{
			get
			{
				return this.m_Index;
			}
			set
			{
				this.m_Index = value;
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public string category
		{
			get
			{
				return this.m_Category;
			}
			set
			{
				this.m_Category = value;
			}
		}

		public EnumPositionAttribute(int index)
		{
			this.m_Index = index;
			this.m_Name = "";
		}

		public EnumPositionAttribute(string name, int index)
		{
			this.m_Index = index;
			this.m_Name = name;
		}

		public EnumPositionAttribute(string category, string name, int index)
		{
			this.m_Category = category;
			this.m_Index = index;
			this.m_Name = name;
		}

		private int m_Index;

		private string m_Name;

		private string m_Category;
	}
}