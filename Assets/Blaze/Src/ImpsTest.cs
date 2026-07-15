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
        public Blaze.Runtime.World.BlazeGrid grid;

        public void Awake()
        {
            BlazeCoroutineRunner.Instance.BStartCoroutine(Coroutine2());
            BlazeCoroutineRunner.Instance.BStartCoroutine(Coroutine());
            // BlazeCoroutineRunner.Instance.StartCoroutine();
        }

        public IEnumerator Coroutine()
        {
            grid.size = new(16, 16);
            yield return grid.Init();
            grid.Resize(new(4, 4));
            grid.Resize(new(16, 16));
            // yield return new WaitForSeconds(1.0f);
            // yield return grid.Resize(new(32, 32));

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