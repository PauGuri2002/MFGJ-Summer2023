using UnityEngine;
using UnityEngine.EventSystems;

public class InteriorDoor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.StartMission();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("hovering");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("not hovering");
    }

    public void OnSelect(BaseEventData eventData)
    {
        print("selected");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        print("unselected");
    }
}
