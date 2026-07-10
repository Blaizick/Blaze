using System;
using System.Collections;
using Blaze.Runtime.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blaze.Runtime.Ui
{
    public class TextScreen : MonoBehaviour, IPointerClickHandler
    {
        public GameObject root;
        public TextTypewriter typewriter;
        public CanvasGroup canvasGroup;

        public GameObject clickTooltipRoot;
        public CanvasGroup clickTooltipCanvasGroup;
        public Tween clickTooltipTween;
        [NonSerialized] public bool clickTooltipShown;

        public IEnumerator Init()
        {
            clickTooltipRoot.SetActive(false);
            root.SetActive(false);
        
            yield break;
        }

        public void _Update()
        {
            if (typewriter.WaitingForClick && typewriter.WaitingForClickTime >= 1.0f)
            {
                if (!clickTooltipShown)
                {
                    if (TweenUtils.IsTweenActive(clickTooltipTween))
                    {
                        clickTooltipTween.Complete();
                    }
                    clickTooltipRoot.gameObject.SetActive(true);
                    clickTooltipCanvasGroup.alpha = 0.0f;
                    clickTooltipTween = clickTooltipCanvasGroup.DOFade(1.0f, 0.25f);
                    clickTooltipShown = true;
                }
            }
            else
            {
                if (clickTooltipShown)
                {
                    if (clickTooltipShown)
                    {
                        if (TweenUtils.IsTweenActive(clickTooltipTween))
                        {
                            clickTooltipTween.Complete();
                        }
                        clickTooltipRoot.gameObject.SetActive(true);
                        clickTooltipCanvasGroup.alpha = 1.0f;
                        clickTooltipTween = clickTooltipCanvasGroup.DOFade(0.0f, 0.25f).OnComplete(() =>
                        {
                            clickTooltipRoot.SetActive(false);
                        });
                        clickTooltipShown = false;
                    }
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickTooltipShown)
            {
                if (TweenUtils.IsTweenActive(clickTooltipTween))
                {
                    clickTooltipTween.Complete();
                }
                clickTooltipRoot.gameObject.SetActive(true);
                clickTooltipCanvasGroup.alpha = 1.0f;
                clickTooltipTween = clickTooltipCanvasGroup.DOFade(0.0f, 0.25f).OnComplete(() =>
                {
                    clickTooltipRoot.SetActive(false);
                });
                clickTooltipShown = false;
            }
            typewriter.OnPointerClick(eventData);
        }

        public void ShowImmediate()
        {
            clickTooltipRoot.SetActive(false);
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
            yield return canvasGroup.DOFade(1.0f, 0.25f).WaitForCompletion();
            typewriter.Text = string.Empty;
            ShowImmediate();
        }
        public IEnumerator HideCoroutine()
        {
            ShowImmediate();
            yield return canvasGroup.DOFade(0.0f, 0.25f).WaitForCompletion();
            typewriter.Text = string.Empty;
            HideImmediate();
        }
    }
}