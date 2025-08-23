using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodViewMove : MonoBehaviour
{
    private InputActions inputActions;
    [Header("��ת����")]
    [SerializeField] private Transform focusPoint;
    public float maxFocusPositionDistance = 30f;
    [SerializeField] private float rotationSpeed = 200;
    private float pitch;
    public float minPitch = 5f;
    public float maxPitch = 85f;
    public Vector3 zoomVelocity = Vector3.zero;
    public Vector2 currentMousePoint;

    [Header("��������")]
    public float zoomDurationTime = .5f;
    public List<float> zooms = new List<float>();
    public Vector3 orinalZoomPosition;
    public Coroutine currentZoom;

    [Header("�ƶ�����")]
    [SerializeField] private Camera targetCamera; // ָ����������
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
        // �� LeftClick �¼�
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

        // ������Ļ���������������
        Ray rayOrigin = targetCamera.ScreenPointToRay(dragOrigin);
        Ray rayCurrent = targetCamera.ScreenPointToRay(currentMouse);

        // �����������ص���ƽ�棨y=0���ƶ�
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // ������������������ཻ�ĵ�
        float distance;
        groundPlane.Raycast(rayOrigin, out distance);
        Vector3 worldPointOrigin = rayOrigin.GetPoint(distance);

        groundPlane.Raycast(rayCurrent, out distance);
        Vector3 worldPointCurrent = rayCurrent.GetPoint(distance);

        // �������������µ�ƽ������
        Vector3 move = worldPointCurrent - worldPointOrigin;

        // ������Ҫ����move����������ֻ��ˮƽ�����ƶ�
        move.y = 0f;

        transform.Translate(-move * dragSpeed * Time.deltaTime, Space.World);

        // ������һ֡λ�ã�ʵ�������϶�
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
            //����focusPoint��Vector.UpΪ����תhorizontalRotation�Ƕȡ�
            pitch = Mathf.Clamp(pitch - verticalRotation, minPitch, maxPitch);
            transform.RotateAround(focusPoint.position, Vector3.up, horizontalRotaion);
            //transform.RotateAround(focusPoint.position, transform.right, pitch - transform.eulerAngles.x);
            transform.LookAt(focusPoint);

        }
    }
    private void StartDrag()
    {
        // ��ȡ��ǰ���λ��
        dragOrigin = inputActions.CameraInput.MousePosition.ReadValue<Vector2>();
    }

    private void StopDrag()
    {
    }
}
