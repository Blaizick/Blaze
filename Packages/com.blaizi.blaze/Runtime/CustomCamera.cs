using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Blaze.Runtime
{
    public class CustomCamera : MonoBehaviour
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

        public void Update()
        {
            _Update();
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