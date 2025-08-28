using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Response : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public bool inOverUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inOverUI = false;
    }
    protected void OnDisable()
    {
        inOverUI = false;
    }
}
