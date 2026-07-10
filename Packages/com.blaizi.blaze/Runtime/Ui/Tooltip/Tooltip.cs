using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Blaze.Runtime.Ui
{
    public class Tooltip : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text description;
 
        public GameObject root;
        public RectTransform rootRectTr;

        public bool Active => root.activeInHierarchy;

        public virtual void Show()
        {
            root.SetActive(true);
        }
        public virtual void Hide()
        {
            root.SetActive(false);
        }

        public virtual void Awake()
        {
            
        }

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            Vector2 defaultMin = new(rootRectTr.anchoredPosition.x, rootRectTr.anchoredPosition.y - rootRectTr.sizeDelta.y);
            Vector2 defaultMax = new(rootRectTr.anchoredPosition.x + rootRectTr.sizeDelta.x, rootRectTr.anchoredPosition.y);

            if (defaultMax.x > Screen.width)
            {
                rootRectTr.pivot = new(1.0f, rootRectTr.pivot.y);
            }
            else
            {
                rootRectTr.pivot = new(0.0f, rootRectTr.pivot.y);
            }
            if (defaultMin.y < 0.0f)
            {
                rootRectTr.pivot = new(rootRectTr.pivot.x, 0.0f);
            }
            else
            {
                rootRectTr.pivot = new(rootRectTr.pivot.x, 1.0f);
            }
        }

        public virtual void OnDestroy()
        {
            
        }
    }

    public class TooltipRaycaster
    {
        public Tooltip tooltip;
        public TooltipTrigger curTrigger = null;

        public TooltipRaycaster(Tooltip tooltip)
        {
            this.tooltip = tooltip;
        }

        public void Raycast()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            TooltipTrigger trigger = null;
            if (raycastResults.Count > 0)
            {
                if (!raycastResults.First().gameObject.TryGetComponent(out trigger))
                {
                    trigger = raycastResults.First().gameObject.GetComponentInParent<TooltipTrigger>();
                }
            }

            if (trigger)
            {
                tooltip.title.text = trigger.title;
                tooltip.description.text = trigger.description;
                if (curTrigger == trigger && !tooltip.Active)
                {
                    tooltip.Show();
                }
            }
            else
            {
                tooltip.Hide();
            }

            curTrigger = trigger;
        }
    }
}