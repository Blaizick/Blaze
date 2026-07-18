using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blaze.Runtime.Ui
{
    public class PopupsUi : ManagedBehaviour
    {
        public PopupOptionUiCntPfb popupOptionUiCntPfb;
        public Popup popupUiCntPfb; 
        public RectTransform rootRectTr;

        [NonSerialized] public List<Popup> instances = new();

        public void Restart()
        {
            foreach (var i in instances)
            {
                Destroy(i.gameObject);
            }
            instances.Clear();
        }

        public void ShowPopup(string title, string desc, UnityAction onClose, List<PopupOption> options)
        {
            ShowPopup(new()
            {
                title = title,
                description = desc,
                onClose = onClose,
                options = options,
            });
        }
        public void ShowPopup(PopupInfo popupInfo)
        {
            if (popupInfo == null) return;

            var scr = Instantiate(popupUiCntPfb, rootRectTr);
            scr.titleText.text = popupInfo.title;
            scr.descText.text = popupInfo.description;
            scr.closeBtn.onClick.AddListener(() =>
            {
                instances.Remove(scr);
                Destroy(scr.gameObject);
                popupInfo.onClose?.Invoke();
            });

            if (popupInfo.options != null)
            {
                foreach (var o in popupInfo.options)
                {
                    var s = Instantiate(popupOptionUiCntPfb, scr.optionsRootTransform);
                    s.btn.onClick.AddListener(() =>
                    {
                        instances.Remove(scr);
                        Destroy(scr.gameObject);
                        o.onChoose?.Invoke();
                    });
                    s.text.text = o.title;
                    s.tooltipTrigger.title = o.tooltipTitle;
                    s.tooltipTrigger.description = o.tooltipDescription;
                }
            }
            scr.Init();

            instances.Add(scr);
        }
    }

    public class PopupOption
    {
        public string title;
        public string tooltipTitle;
        public string tooltipDescription;
        public UnityAction onChoose;
    }

    public class PopupInfo
    {
        public string title;
        public string description;
        public UnityAction onClose;
        public List<PopupOption> options;
    }
}