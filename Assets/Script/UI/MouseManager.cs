using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void HideMouseCursor()
    {
        if (CameraManager.instance.currentCamera.GetComponent<CameraType>().cameraType == CameraOfType.GodViewCamera)
        {
            return;
        }
        Cursor.visible = false;

        // 锁定鼠标光标在屏幕中央
        // 这会捕获鼠标输入，即使光标在屏幕外也能获取相对移动
        Cursor.lockState = CursorLockMode.Locked;
    }
}
