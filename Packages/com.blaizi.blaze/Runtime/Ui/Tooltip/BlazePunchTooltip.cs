using Blaze.Runtime.Tweening;
using Blaze.Runtime.Utils;
using UnityEngine;

namespace Blaze.Runtime.Ui
{
    public class BlazePunchTooltip : Tooltip
    {
        private QTween m_Tween;

        public override void OnDestroy()
        {
            rootRectTr.QKill();

            base.OnDestroy();
        }

        public override void Show()
        {
            if (QTweenUtils.IsTweenActive(m_Tween))
            {
                m_Tween.Complete();
            }
            rootRectTr.localScale = Vector3.one;
            m_Tween = rootRectTr.QPunchLocalScale(new Vector3(0.1f, 0.1f, 0.0f), 0.25f);
            
            base.Show();
        }
    }
}