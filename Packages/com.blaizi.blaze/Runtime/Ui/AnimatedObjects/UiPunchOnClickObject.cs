using UnityEngine.EventSystems;

namespace Blaze.Runtime.Ui
{
    public class UiPunchOnClickObject : PunchOnClickObject, IPointerClickHandler
    {
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
    }
}