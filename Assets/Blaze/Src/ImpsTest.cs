using System.Collections;
using System.Xml.Schema;
using Blaze.Runtime;
using Blaze.Runtime.Tweening;
using Blaze.Runtime.Ui;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blaze.Test
{
    public class ImpsTest : ManagedBehaviour
    {
        public CanvasGroup canvasGroup;
        public TextScreen textScreen;
        public Blaze.Runtime.World.BlazeGrid grid;

        public void Awake()
        {
            QDebugBase<InternalLogChannel>.Verbose = true;
            QDebugBase<InternalLogChannel>.EnableChannels(InternalLogChannel.All);

            QStartCoroutine(Coroutine());
            QStartCoroutine(Coroutine2());

            // this.QStartCoroutine(Coroutine2());
            // this.QStartCoroutine(Coroutine());
            
            // BlazeCoroutineRunner.Instance.StartCoroutine();
        }

        public override void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            var mouseGridPos = grid.WorldToGridPosition(mouseWorldPos);
            QDebugBase<InternalLogChannel>.Log(InternalLogChannel.System, grid.IsRectInBounds(new RectInt(mouseGridPos, new Vector2Int(0, 0))).ToString());

            base.Update();
        }

        public IEnumerator Coroutine()
        {
            grid.Size = new(16, 16);
            grid.Init();
            grid.Resize(new(4, 4));
            
            yield break;
            // grid.Resize(new(16, 16));
            // // yield return new WaitForSeconds(1.0f);
            // // yield return grid.Resize(new(32, 32));

            // yield return canvasGroup.QFade(0.0f, 1.0f).WaitForCompletion();            
            // yield return canvasGroup.QFade(1.0f, 1.0f).WaitForCompletion();            
            
            // yield return textScreen.ShowCoroutine();
            // yield return textScreen.typewriter.WriteCoroutine("dadwadasd");
            // yield return textScreen.typewriter.WaitForClick();
            // yield return textScreen.HideCoroutine();
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