using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Blaze.Runtime.Ui
{
    public class Tooltip : ManagedBehaviour
    {
        public TMP_Text title;
        public TMP_Text description;
 
        public GameObject root;
        public RectTransform RootRectTr => (RectTransform)root.transform;

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

        public override void Update()
        {
            RootRectTr.anchoredPosition = Mouse.current.position.ReadValue();

            Vector2 defaultMin = new(RootRectTr.anchoredPosition.x, RootRectTr.anchoredPosition.y - RootRectTr.sizeDelta.y);
            Vector2 defaultMax = new(RootRectTr.anchoredPosition.x + RootRectTr.sizeDelta.x, RootRectTr.anchoredPosition.y);

            if (defaultMax.x > Screen.width)
            {
                RootRectTr.pivot = new(1.0f, RootRectTr.pivot.y);
            }
            else
            {
                RootRectTr.pivot = new(0.0f, RootRectTr.pivot.y);
            }
            if (defaultMin.y < 0.0f)
            {
                RootRectTr.pivot = new(RootRectTr.pivot.x, 0.0f);
            }
            else
            {
                RootRectTr.pivot = new(RootRectTr.pivot.x, 1.0f);
            }

            base.Update();
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