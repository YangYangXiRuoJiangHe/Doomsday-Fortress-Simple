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

        // �������������Ļ����
        // ��Ჶ��������룬��ʹ�������Ļ��Ҳ�ܻ�ȡ����ƶ�
        Cursor.lockState = CursorLockMode.Locked;
    }
}
