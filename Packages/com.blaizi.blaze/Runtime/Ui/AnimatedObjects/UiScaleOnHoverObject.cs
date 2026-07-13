using UnityEngine.EventSystems;

namespace Blaze.Runtime.Ui
{
    public class UiScaleOnHoverObject : ScaleOnHoverObject, IPointerEnterHandler, IPointerExitHandler
    {
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit();
        }
    }
}