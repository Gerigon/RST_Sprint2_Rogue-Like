using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    public Text theText;

    public void OnPointerDown(PointerEventData evd)
    {
        theText.color = new Color(1f, 0.4f, 0f); //Or however you do your color
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        theText.color = new Color(1f, 0.4f, 0f); //Or however you do your color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = new Color(1f,1f,1f); //Or however you do your color
    }
}
