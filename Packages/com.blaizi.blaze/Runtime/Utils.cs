using System.Collections.Generic;
using DG.Tweening;

namespace Blaze.Runtime.Utils
{
    public static class ListUtils
    {
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }

    public static class CurrencyFormatUtils
    {
        public const string thousand = "K.";
        public const string million = "M.";

        public static string FormatCurrency(float count)
        {
            float thousands = count / 1000;
            float millions = count / 1000000;

            if (millions >= 1f)
            {
                return $"{millions} {million}";
            }
            if (thousands >= 1f)
            {
                return $"{thousands} {thousand}";
            }

            return count.ToString();
        }
    }

    public static class TweenUtils
    {
        public static bool IsTweenActive(Tween tween)
        {
            return tween != null && tween.IsActive();
        }
    }
}