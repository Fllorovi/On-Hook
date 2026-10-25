using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTest : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ZOOM BUTTON CLICKED");
    }
}