using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Blaze.Runtime.Ui
{
    public class Popup : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public TMP_Text titleText;
        public TMP_Text descText;
        public Button closeBtn;
        public RectTransform optionsRootTransform;

        [Header("Settings")]
        public RectTransform dragArea;
        public bool clampToCanvas = true;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;

        private Vector2 m_Offset;

        private bool m_Dragging;


        public void Init()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_Canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            if (!m_Dragging)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_Canvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                Camera.main,
                out Vector2 localPoint
            );

            Vector2 targetPos = localPoint - m_Offset;

            if (clampToCanvas)
            {
                targetPos = ClampToCanvas(targetPos);
            }

            m_RectTransform.anchoredPosition = targetPos;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_Dragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out m_Offset
            );
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_Dragging = false;
        }

        private Vector2 ClampToCanvas(Vector2 pos)
        {
            RectTransform canvasRect = m_Canvas.transform as RectTransform;

            Vector2 min = canvasRect.rect.min - m_RectTransform.rect.min;
            Vector2 max = canvasRect.rect.max - m_RectTransform.rect.max;

            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);

            return pos;
        }
    }
}