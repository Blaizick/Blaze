using Blaze.Runtime.Utils;
using DG.Tweening;
using UnityEngine;

namespace Blaze.Runtime.Ui
{
    public class BlazePunchTooltip : Tooltip
    {
        private Tween m_Tween;

        public override void OnDestroy()
        {
            rootRectTr.DOKill();

            base.OnDestroy();
        }

        public override void Show()
        {
            if (TweenUtils.IsTweenActive(m_Tween))
            {
                m_Tween.Complete();
            }
            rootRectTr.localScale = Vector3.one;
            m_Tween = rootRectTr.DOPunchScale(new Vector3(0.1f, 0.1f, 0.0f), 0.25f);
            
            base.Show();
        }
    }
}