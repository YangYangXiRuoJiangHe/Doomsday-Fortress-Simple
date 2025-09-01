using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public Camera currentCamera;
    public List<Camera> cameraManager = new List<Camera>();
    private float pitch = 0f; // 单独存储上下角度
    float rotY,rotZ;
    [Header("输入系统")]
    public float sensitivity = 30.0f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ClosedAllCamera();
        SetCameraActivate(0);
        SetPlayerSensitivity(PlayerPrefs.GetFloat("playerSensitivityui"));
    }

    private void ClosedAllCamera()
    {
        foreach (Camera camera in cameraManager)
        {
            camera.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetCameraActivate(0);
            UIManager.instance.inGameUI.OffBuildUI();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetCameraActivate(2);
            UIManager.instance.inGameUI.OffBuildUI();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetCameraActivate(1);
            MouseManager.instance.ShowMouseCursor();
        }
    }
    public void SetCameraActivate(int index)
    {
        if(index >= cameraManager.Count)
        {
            return;
        }
        foreach(Camera camera in cameraManager)
        {
            camera.gameObject.SetActive(false);
        }
        cameraManager[index].gameObject.SetActive(true);
        currentCamera = cameraManager[index];
        if(index == 0)
        {
            rotZ = 15;
        }else if(index == 2)
        {
            rotZ = 0;
        }
    }
    public void CameraRotation(InputActions inputActions)
    {
        rotY = inputActions.PlayerInput.CameraRotate.ReadValue<Vector2>().y * sensitivity * .6f;

        // 更新相机上下角度
        pitch += -rotY;
        pitch = Mathf.Clamp(pitch, -80f, 89f);
        // 直接设置相机的本地旋转（推荐使用本地旋转）
        if(currentCamera.GetComponent<CameraType>().cameraType == CameraOfType.GodViewCamera)
        {
            return;
        }
        currentCamera.gameObject.transform.localEulerAngles = new Vector3(pitch, 0, rotZ);
    }
    public void SetPlayerSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

}
