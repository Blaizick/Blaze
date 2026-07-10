using Blaze.Runtime.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Blaze.Runtime.Ui
{
    [RequireComponent(typeof(ScaleOnHoverObject))]
    public class BlazeButton : UIBehaviour, IPointerClickHandler
    {
        private Tween m_PunchTween;

        public Vector2 punchEffect = new(0.25f, 0.25f);
        public float animDuration = 0.1f;

        public UnityEvent onClick = new();

        private ScaleOnHoverObject m_ScaleOnHoverObject;
        public ScaleOnHoverObject ScaleOnHoverObject
        {
            get
            {
                if (m_ScaleOnHoverObject == null)
                {
                    m_ScaleOnHoverObject = GetComponent<ScaleOnHoverObject>();
                }
                return m_ScaleOnHoverObject;
            }
        }

        protected override void OnDestroy()
        {
            transform.DOKill();

            base.OnDestroy();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (animDuration > 0.0f)
            {
                if (TweenUtils.IsTweenActive(m_PunchTween))
                {
                    m_PunchTween.Complete();
                }
                ScaleOnHoverObject.SetBackToNormalSize();
                m_PunchTween = transform.DOPunchScale(punchEffect, animDuration);
            }
            onClick?.Invoke();
        }
    }
}