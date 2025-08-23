using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CameraManager instance;
    public Camera currentCamera;
    public List<Camera> cameraManager = new List<Camera>();
    // Start is called before the first frame update
    void Start()
    {
        //ÖÆ×÷µ¥Àý
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        foreach (Camera camera in cameraManager)
        {
            camera.gameObject.SetActive(false);
        }
        cameraManager[0].gameObject.SetActive(true);
        currentCamera = cameraManager[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetCameraActivate(0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetCameraActivate(1);
        }
    }
    public void SetCameraActivate(int position)
    {
        if(position >= cameraManager.Count)
        {
            return;
        }
        foreach(Camera camera in cameraManager)
        {
            camera.gameObject.SetActive(false);
        }
        cameraManager[position].gameObject.SetActive(true);
        currentCamera = cameraManager[position];
    }
}
