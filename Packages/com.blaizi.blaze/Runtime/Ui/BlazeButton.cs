using Blaze.Runtime.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Blaze.Runtime.Tweening;

namespace Blaze.Runtime.Ui
{
    public class BlazeButton : UIBehaviour, IPointerClickHandler
    {
        public UnityEvent onClick = new();

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}