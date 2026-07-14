using System.Collections;
using Blaze.Runtime;
using Blaze.Runtime.Tweening;
using Blaze.Runtime.Ui;
using UnityEngine;

namespace Blaze.Test
{
    public class ImpsTest : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TextScreen textScreen;

        public void Awake()
        {
            BlazeCoroutineRunner.Instance.BRunCoroutine(Coroutine2());
            BlazeCoroutineRunner.Instance.BRunCoroutine(Coroutine());
            // BlazeCoroutineRunner.Instance.StartCoroutine();
        }

        public IEnumerator Coroutine()
        {
            yield return canvasGroup.QFade(0.0f, 1.0f).WaitForCompletion();            
            yield return canvasGroup.QFade(1.0f, 1.0f).WaitForCompletion();            
            
            yield return textScreen.ShowCoroutine();
            yield return textScreen.typewriter.WriteCoroutine("dadwadasd");
            yield return textScreen.typewriter.WaitForClick();
            yield return textScreen.HideCoroutine();
        }

        public IEnumerator Coroutine2()
        {
            yield return Coroutine3();
        }
        public IEnumerator Coroutine3()
        {
            yield break;
        }
    }
}