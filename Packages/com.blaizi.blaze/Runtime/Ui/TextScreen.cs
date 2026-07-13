using System;
using System.Collections;
using Blaze.Runtime.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blaze.Runtime.Ui
{
    public class TextScreen : MonoBehaviour, IPointerClickHandler
    {
        public GameObject root;
        public TextTypewriter typewriter;
        public CanvasGroup canvasGroup;

        public bool clickTooltip;

        public GameObject clickTooltipRoot;
        public CanvasGroup clickTooltipCanvasGroup;
        public QTween clickTooltipTween;
        [NonSerialized] public bool clickTooltipActive;

        public void Init()
        {
            if (clickTooltip)
            {
                clickTooltipRoot.SetActive(false);
            }
            root.SetActive(false);
        }

        public void _Update()
        {
            if (clickTooltip)
            {
                if (typewriter.WaitingForClick && typewriter.WaitingForClickTime >= 1.0f)
                {
                    if (!clickTooltipActive)
                    {
                        if (QTweenUtils.IsTweenActive(clickTooltipTween))
                        {
                            clickTooltipTween.Complete();
                        }
                        clickTooltipRoot.gameObject.SetActive(true);
                        clickTooltipCanvasGroup.alpha = 0.0f;
                        clickTooltipTween = clickTooltipCanvasGroup.QFade(1.0f, 0.25f);
                        clickTooltipActive = true;
                    }
                }
                else
                {
                    if (clickTooltipActive)
                    {
                        if (QTweenUtils.IsTweenActive(clickTooltipTween))
                        {
                            clickTooltipTween.Complete();
                        }
                        clickTooltipRoot.gameObject.SetActive(true);
                        clickTooltipCanvasGroup.alpha = 1.0f;
                        clickTooltipTween = clickTooltipCanvasGroup.QFade(0.0f, 0.25f).OnComplete(() =>
                        {
                            clickTooltipRoot.SetActive(false);
                        });
                        clickTooltipActive = false;
                    }
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickTooltip)
            {
                if (clickTooltipActive)
                {
                    if (QTweenUtils.IsTweenActive(clickTooltipTween))
                    {
                        clickTooltipTween.Complete();
                    }
                    clickTooltipRoot.gameObject.SetActive(true);
                    clickTooltipCanvasGroup.alpha = 1.0f;
                    clickTooltipTween = clickTooltipCanvasGroup.QFade(0.0f, 0.25f).OnComplete(() =>
                    {
                        clickTooltipRoot.SetActive(false);
                    });
                    clickTooltipActive = false;
                }
            }
            typewriter.OnPointerClick(eventData);
        }

        public void ShowImmediate()
        {
            if (clickTooltip)
            {
                if (QTweenUtils.IsTweenActive(clickTooltipTween))
                {
                    clickTooltipTween.Complete();
                }
                clickTooltipRoot.SetActive(false);
                clickTooltipActive = false;
            }
            canvasGroup.alpha = 1.0f;
            root.SetActive(true);
        }
        public void HideImmediate()
        {
            canvasGroup.alpha = 0.0f;
            root.SetActive(false);
        }

        public IEnumerator ShowCoroutine()
        {
            HideImmediate();
            root.SetActive(true);
            yield return canvasGroup.QFade(1.0f, 0.25f).WaitForCompletion();
            typewriter.Text = string.Empty;
            ShowImmediate();
        }
        public IEnumerator HideCoroutine()
        {
            ShowImmediate();
            yield return canvasGroup.QFade(0.0f, 0.25f).WaitForCompletion();
            typewriter.Text = string.Empty;
            HideImmediate();
        }
    }
}