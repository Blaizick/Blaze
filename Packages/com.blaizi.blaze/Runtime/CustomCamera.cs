using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blaze.Runtime
{
    public class CustomCamera : ManagedBehaviour
    {
        public Camera _camera;
        public float targetZoom;
        public float zoomSpeed;
        
        [SerializeField]
        public bool customUpdate;

        public void Init()
        {
            targetZoom = _camera.orthographicSize;
        }
        public void _Update()
        {
            _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize, targetZoom, zoomSpeed * Time.unscaledDeltaTime);
        }

        public override void Update()
        {
            _Update();
            base.Update();
        }

        public float Zoom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return targetZoom;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                targetZoom = value;
                _camera.orthographicSize = value;                
            }
        }
    }
}