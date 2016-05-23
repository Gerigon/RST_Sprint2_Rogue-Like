using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler
{
    public Text theText;

    public void OnPointerDown(PointerEventData eventData)
    {
        theText.color = new Color(1f, 0.4f, 0f); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        theText.color = new Color(1f, 0.4f, 0f); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = new Color(0.5f, 0.5f, 0.5f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = new Color(1f, 1f, 1f); 
    }
}
