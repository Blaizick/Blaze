using Blaze.Runtime.Tweening;
using UnityEngine;

namespace Blaze.Runtime.Ui
{
    public class ScaleOnHoverObject : ManagedBehaviour
    {
        private QTween m_Tween;

        public Vector2 minScale = Vector2.one;
        public Vector2 maxScale = new(1.25f, 1.25f);

        public Vector3 MinScaleVec3 => new(minScale.x, minScale.y, 1.0f);
        public Vector3 MaxScaleVec3 => new(maxScale.x, maxScale.y, 1.0f);
        
        public float animationDuration = 0.1f;

        private bool m_Scaled;
        public bool Scaled => m_Scaled;

        public void OnPointerEnter()
        {
            if (animationDuration > 0.0f)
            {
                if (QTweenUtils.IsTweenActive(m_Tween))
                {
                    m_Tween.Complete();
                }
                transform.localScale = MinScaleVec3;
                m_Tween = transform.
                    QLocalScale(MaxScaleVec3, animationDuration).
                    SetDeltaTimeSource(DeltaTimeSource.UnscaledDeltaTime);
            }
            else
            {
                transform.localScale = MaxScaleVec3;
            }
            m_Scaled = true;
        }

        public virtual void OnPointerExit()
        {
            if (animationDuration > 0.0f)
            {
                if (QTweenUtils.IsTweenActive(m_Tween))
                {
                    m_Tween.Complete();
                }
                transform.localScale = MaxScaleVec3;
                m_Tween = transform.
                    QLocalScale(MinScaleVec3, animationDuration).
                    SetDeltaTimeSource(DeltaTimeSource.UnscaledDeltaTime);
            }
            else
            {
                transform.localScale = MinScaleVec3;
            }
            m_Scaled = false;
        }
        public virtual void SetBackToNormalSize()
        {
            if (m_Scaled)
            {
                transform.localScale = MaxScaleVec3;
            }
            else
            {
                transform.localScale = MinScaleVec3;
            }
        }

        public virtual void OnDestroy()
        {
            transform.QKill();
        }
    }
}