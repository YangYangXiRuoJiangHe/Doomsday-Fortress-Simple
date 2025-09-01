using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftClickBuild : MonoBehaviour
{
    public LayerMask isBuild;
    public Detail_UI detaioUI;
    public void ClickBuild(InputActions inputActions)
    {
        if (PreBuildCheck.instance.GetWaitBuilder())
        {
            return;
        }
        foreach(UI_Response ui in UIManager.instance.inGameUI.uiResponse)
        {
            if (ui.inOverUI)
            {
                return;
            }
        }
        Vector2 mousePosition = inputActions.CameraInput.MousePosition.ReadValue<Vector2>();
        Camera currentCamera = CameraManager.instance.currentCamera;
        Ray ray = currentCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,100f,isBuild))
        {
            UIManager.instance.inGameUI.OnDetailUI(hit.transform.GetComponent<DetailDescribe>(),hit.transform.gameObject);
        }
        else
        {
            UIManager.instance.inGameUI.OffDetailUI();
        }
    }
}
