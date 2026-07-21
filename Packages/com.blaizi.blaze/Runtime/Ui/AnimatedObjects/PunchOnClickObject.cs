using UnityEngine;
using UnityEngine.EventSystems;
using Blaze.Runtime.Tweening;

namespace Blaze.Runtime.Ui
{
    public class PunchOnClickObject : ManagedBehaviour
    {
        private QTween m_PunchTween;

        public Vector2 punchEffect = new(0.25f, 0.25f);
        public float animationDuration = 0.1f;

        public virtual void OnDestroy()
        {
            QTween.CompleteAll(transform);
        }

        public virtual void OnClick()
        {
            if (animationDuration > 0.0f)
            {
                m_PunchTween.Complete();

                if (TryGetComponent(out UiScaleOnHoverObject scaleOnHoverObject))
                {
                    scaleOnHoverObject.SetBackToNormalSize();
                }
                else
                {
                    transform.localScale = Vector3.one;
                }
                
                m_PunchTween = QTween.PunchLocalScale(transform, punchEffect, animationDuration).
                    SetDeltaTimeSource(DeltaTimeSource.UnscaledDeltaTime);
            }
        }
    }
}