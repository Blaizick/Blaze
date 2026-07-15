using System;
using System.Collections;
using UnityEngine;

namespace Blaze.Runtime.World
{
    public class BlazeTile : MonoBehaviour
    {
        [NonSerialized] public BlazeGrid grid;
        [NonSerialized] public Vector2Int gridPosition;
    
        public virtual IEnumerator Init()
        {
            yield break;
        }

        public virtual void _OnDestroy()
        {
            
        }
    }
}