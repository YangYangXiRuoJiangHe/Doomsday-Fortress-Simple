using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodViewMove : MonoBehaviour
{
    private InputActions inputActions;
    [Header("旋转详情")]
    [SerializeField] private Transform focusPoint;
    public float maxFocusPositionDistance = 30f;
    [SerializeField] private float rotationSpeed = 200;
    private float pitch;
    public float minPitch = 5f;
    public float maxPitch = 85f;
    public Vector3 zoomVelocity = Vector3.zero;
    public Vector2 currentMousePoint;

    [Header("缩放详情")]
    public float zoomDurationTime = .5f;
    public List<float> zooms = new List<float>();
    public Vector3 orinalZoomPosition;
    public Coroutine currentZoom;

    [Header("移动详情")]
    [SerializeField] private Camera targetCamera; // 指定你的摄像机
    [SerializeField] private float dragSpeed = 5f;
    private Vector2 dragOrigin;

    private void OnEnable()
    {
        inputActions.CameraInput.Enable();
    }
    private void OnDisable()
    {
        inputActions.CameraInput.Disable();
    }
    private void Awake()
    {
        if (inputActions == null)
        {
            inputActions = new InputActions();
        }
        // 绑定 LeftClick 事件
        inputActions.CameraInput.RightClick.started += ctx => StartDrag();
        inputActions.CameraInput.RightClick.canceled += ctx => StopDrag();
    }
    private void Start()
    {
        /*for(int i = 0; i < zooms.Count; i++)
        {
            zooms[i] += transform.position.y;
        }*/
        currentMousePoint = inputActions.CameraInput.CameraMove.ReadValue<Vector2>();
    }
    private void Update()
    {
        //Vector2 move = inputActions.CameraInput.Move.ReadValue<Vector2>();
        if (Mouse.current.rightButton.isPressed)
        {
            HandleMove();
        }
        HandleRotation();

        HandleZoom();
        focusPoint.position = transform.position + (transform.forward * GetFocusPointDistance());
    }

    private void HandleMove()
    {
        Vector2 currentMouse = inputActions.CameraInput.MousePosition.ReadValue<Vector2>();
        Vector2 delta = currentMouse - dragOrigin;

        // 计算屏幕到世界坐标的射线
        Ray rayOrigin = targetCamera.ScreenPointToRay(dragOrigin);
        Ray rayCurrent = targetCamera.ScreenPointToRay(currentMouse);

        // 假设我们想沿地面平面（y=0）移动
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // 计算两个射线与地面相交的点
        float distance;
        groundPlane.Raycast(rayOrigin, out distance);
        Vector3 worldPointOrigin = rayOrigin.GetPoint(distance);

        groundPlane.Raycast(rayCurrent, out distance);
        Vector3 worldPointCurrent = rayCurrent.GetPoint(distance);

        // 计算世界坐标下的平移向量
        Vector3 move = worldPointCurrent - worldPointOrigin;

        // 根据需要调整move向量，例如只沿水平方向移动
        move.y = 0f;

        transform.Translate(-move * dragSpeed * Time.deltaTime, Space.World);

        // 更新上一帧位置，实现连续拖动
        dragOrigin = currentMouse;
    }
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (transform.position.y < zooms[0] && scroll > 0)
        {
            return;
        }
        if (transform.position.y > zooms[zooms.Count-1] && scroll < 0)
        {
            return;
        }
        if(scroll > 0)
        {
            StartZoomOut();

        }
        if (scroll < 0)
        {
            StartZoomIn();

        }
    }
    public void StartZoomIn()
    {
        if (currentZoom != null) return;
        currentZoom = StartCoroutine(ZoomIn());
    }
    public void StartZoomOut()
    {
        if (currentZoom != null) return;
        currentZoom = StartCoroutine(ZoomOut());
    }

    IEnumerator ZoomIn()
    {
        int currentIndex = GetClosestZoomIndex();
        int targetIndex = currentIndex + 1;
        if (targetIndex >= zooms.Count)
        {
            currentZoom = null;
            yield break;
        }

        yield return StartCoroutine(MoveToHeight(zooms[targetIndex]));

        currentZoom = null;
    }

    IEnumerator ZoomOut()
    {
        int currentIndex = GetClosestZoomIndex();
        int targetIndex = currentIndex - 1;
        if (targetIndex < 0)
        {
            currentZoom = null;
            yield break;
        }
        yield return StartCoroutine(MoveToHeight(zooms[targetIndex]));
        currentZoom = null;
    }
    int GetClosestZoomIndex()
    {
        int bestIndex = 0;
        float bestDistance = Mathf.Infinity;
        for (int i = 0; i < zooms.Count; i++)
        {
            float distance = Mathf.Abs(transform.position.y - zooms[i]);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestIndex = i;
            }
        }
        return bestIndex;
    }
    IEnumerator MoveToHeight(float targetY)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
        float time = 0f;
        while (time < zoomDurationTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / zoomDurationTime);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        transform.position = targetPosition;
    }
    private float GetFocusPointDistance()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxFocusPositionDistance))
        {
            return hit.distance;
        }
        return maxFocusPositionDistance;
    }
    private void HandleRotation()
    {
        if (Input.GetMouseButton(2))
        {
            float horizontalRotaion = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            //沿着focusPoint以Vector.Up为轴旋转horizontalRotation角度。
            pitch = Mathf.Clamp(pitch - verticalRotation, minPitch, maxPitch);
            transform.RotateAround(focusPoint.position, Vector3.up, horizontalRotaion);
            //transform.RotateAround(focusPoint.position, transform.right, pitch - transform.eulerAngles.x);
            transform.LookAt(focusPoint);

        }
    }
    private void StartDrag()
    {
        // 获取当前鼠标位置
        dragOrigin = inputActions.CameraInput.MousePosition.ReadValue<Vector2>();
    }

    private void StopDrag()
    {
    }
}
