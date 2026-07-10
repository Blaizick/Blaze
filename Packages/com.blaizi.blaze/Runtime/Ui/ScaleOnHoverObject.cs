using Blaze.Runtime.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blaze.Runtime.Ui
{
    public class ScaleOnHoverObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Tween m_Tween;

        public Vector2 minScale = Vector2.one;
        public Vector2 maxScale = new(1.25f, 1.25f);
        public float animDur = 0.1f;

        private bool m_Scaled;
        public bool Scaled => m_Scaled;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (animDur > 0.0f)
            {
                if (TweenUtils.IsTweenActive(m_Tween))
                {
                    m_Tween.Complete();
                }
                transform.localScale = minScale;
                m_Tween = transform.DOScale(maxScale, animDur);
            }
            else
            {
                transform.localScale = maxScale;
            }
            m_Scaled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (animDur > 0.0f)
            {
                if (TweenUtils.IsTweenActive(m_Tween))
                {
                    m_Tween.Complete();
                }
                transform.localScale = maxScale;
                m_Tween = transform.DOScale(minScale, animDur);
            }
            else
            {
                transform.localScale = minScale;
            }
            m_Scaled = false;
        }

        public void SetBackToNormalSize()
        {
            if (m_Scaled)
            {
                transform.localScale = maxScale;
            }
            else
            {
                transform.localScale = minScale;
            }
        }

        public void OnDestroy()
        {
            transform.DOKill();
        }
    }
}