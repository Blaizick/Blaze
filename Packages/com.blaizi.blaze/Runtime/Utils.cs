using System.Collections.Generic;
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
}