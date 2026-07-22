using System;
using System.Collections;
using Blaze.Runtime;
using Blaze.Runtime.Cms;
using Blaze.Runtime.DependencyInjection;
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

        [QInject, NonSerialized]
        public IMonoObject monoObject2;
        [QInject, NonSerialized]
        public Object2 object2;
        [QInject, NonSerialized]
        public QDiContainer container;

        public GameObject prefab;

        public void Awake()
        {
            QDebugBase<InternalLogChannel>.Verbose = true;
            QDebugBase<InternalLogChannel>.EnableChannels(InternalLogChannel.All);

            Cms.LoadAll("Content");
            // Debug.

            QStartCoroutine(Coroutine());
            QStartCoroutine(Coroutine2());

            var inst1 = container.InstantiatePrefab(prefab);
            var inst2 = container.InstantiatePrefabForComponent<MonoObject2>(prefab);
            Debug.Log(inst1);
            Debug.Log(inst2);

            Debug.Log(object2.a);
            Debug.Log(monoObject2);

            Debug.Log(Content.CmsEntity1.GetComponent<CmsComponent1>()._int);
            Debug.Log(Content.CmsEntity1.GetComponent<CmsComponent2>().cmsEntity.AsCmsEntity().GetComponent<CmsComponent1>()._string);

            // this.QStartCoroutine(Coroutine2());
            // this.QStartCoroutine(Coroutine());
            
            // BlazeCoroutineRunner.Instance.StartCoroutine();
        }

        public override void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            var mouseGridPos = grid.WorldToGridPosition(mouseWorldPos);
            // QDebugBase<InternalLogChannel>.Log(InternalLogChannel.System, grid.IsRectInBounds(new RectInt(mouseGridPos, new Vector2Int(0, 0))).ToString());

            base.Update();
        }

        public IEnumerator Coroutine()
        {
            grid.Size = new(16, 16);
            grid.Init();
            grid.Resize(new(4, 4));
            
            // yield break;
            grid.Resize(new(16, 16));
            // yield return new WaitForSeconds(1.0f);
            // yield return grid.Resize(new(32, 32));

            yield return QTween.Alpha(canvasGroup, 1.0f, 0.0f, 1.0f).WaitForCompletion();            
            yield return QTween.Alpha(canvasGroup, 0.0f, 1.0f, 1.0f).WaitForCompletion();            
            
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

    [Serializable]
    public struct CmsComponent1 : ICmsComponent
    {
        public int _int;
        public string _string;
    }

    [Serializable]
    public struct CmsComponent2 : ICmsComponent
    {
        public CmsEntityPfb cmsEntity;
    }
}