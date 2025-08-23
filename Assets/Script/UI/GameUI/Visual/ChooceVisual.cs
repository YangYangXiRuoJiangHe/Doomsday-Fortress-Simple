using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooceVisual : MonoBehaviour
{
    public Button[] chooceVisuals;

    public void OpenAllChooceColor()
    {
        foreach(Button button in chooceVisuals)
        {
            Color color = button.image.color;
            color.a = 1f;
            button.image.color = color;
        }
    }
    public void ClickChooceColor(int index)
    {
        OpenAllChooceColor();
        if(index >= chooceVisuals.Length)
        {
            Debug.Log("点击按钮索引超出了原所有按钮的索引！");
            return;
        }
        Color color = chooceVisuals[index].image.color;
        color.a = .8f;
        chooceVisuals[index].image.color = color;
    }
}
