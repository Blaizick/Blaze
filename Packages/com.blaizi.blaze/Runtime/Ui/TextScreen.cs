using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blaze.Runtime.Ui
{
    public class TextScreen : MonoBehaviour
    {
        public TextTypewriter typewriter;
        public GameObject root;
        public CanvasGroup canvasGroup;

        public void Init()
        {
            root.SetActive(false);
        }

        public IEnumerator WriteTextCoroutine(string text)
        {
            yield return typewriter.WriteCoroutine(text);
        }

        public IEnumerator WaitUntilClick()
        {
            clicked = false;
            while (!clicked)
            {
                yield return null;                
            }
        }

        [NonSerialized] public bool clicked = false;

        public void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (typewriter.writeCoroutine == null)
                {
                    if (!clicked)
                    {
                        clicked = true;
                    }
                }
                else
                {
                    typewriter.Skip();
                }    
            }
        }

        public IEnumerator HideCoroutine()
        {
            root.SetActive(true);
            canvasGroup.alpha = 1.0f;
            float t = 0.0f;
            while (t < 1.0f)
            {
                canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, t);
                yield return null;
            }
            canvasGroup.alpha = 0.0f;
            root.SetActive(false);
        }

        public IEnumerator ShowCoroutine()
        {
            root.SetActive(true);
            canvasGroup.alpha = 0.0f;
            float t = 0.0f;
            while (t < 1.0f)
            {
                canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, t);
                yield return null;
            }
            canvasGroup.alpha = 1.0f;
        }
    }
}