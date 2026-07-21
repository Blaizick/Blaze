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
            QTween.CompleteAll(RootRectTr);

            base.OnDestroy();
        }

        public override void Show()
        {
            m_Tween.Complete();
            RootRectTr.localScale = Vector3.one;
            m_Tween = QTween.PunchLocalScale(RootRectTr, new Vector3(0.1f, 0.1f, 0.0f), 0.25f).
                SetDeltaTimeSource(DeltaTimeSource.UnscaledDeltaTime);
            base.Show();
        }
    }
}