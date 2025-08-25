using UnityEngine;
using UnityEngine.InputSystem;

public class CharController_Motor : MonoBehaviour
{
    [Header("移动详情")]
    private Vector2 moveInput = new Vector2();
    public float speed = 10.0f;
    public float runSpeed = 30f;
    public float currentSpeed;
    public float verticalVelocity = 0f;
    public float isGroundOffset = 1f;
    public Transform checkIsGround;
    public float jumpHeight = 2;
    public float sensitivity = 30.0f;
    public float WaterHeight = 15.5f;
    public float jumpTime = .2f;
    public float jumpTimeDelay = 0f;
    public float gravity = -9.8f;
    float currentGravity = -9.8f;
    public float gravityMultiple = 1;
    [Header("摄象机详情")]
    public CharacterController character;
    public GameObject cam;
    float moveFB, moveLR;
    float rotX, rotY;
    public bool webGLRightClickRotation = true;
    [Header("输入系统")]
    private Player player;
    private InputActions inputActions;
    [Header("动画详情")]
    public Animator anim;
    public bool isRunning;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();
    }
    void Start()
    {
        //LockCursor ();
        character = GetComponent<CharacterController>();
        if (Application.isEditor)
        {
            webGLRightClickRotation = false;
        }
        currentSpeed = speed;
        SetPlayerSensitivity(PlayerPrefs.GetFloat("playerSensitivityui"));
    }
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (!IsGrounded())
        {
            verticalVelocity += currentGravity * Time.deltaTime * gravityMultiple; // 累加重力
        }
        moveFB = moveInput.x * currentSpeed;
        moveLR = moveInput.y * currentSpeed;
        //后期如果开发游泳用
        CheckForWaterHeight();
        Vector3 movement = new Vector3(moveFB, verticalVelocity, moveLR);


        movement = transform.rotation * movement;
        AnimatorControllers(movement);
        character.Move(movement * Time.deltaTime);
    }

    void CheckForWaterHeight()
    {
        if (transform.position.y < WaterHeight)
        {
            currentGravity = 0f;
        }
        else
        {
            currentGravity = gravity;
        }
    }
    public void SetPlayerInput(bool isActive)
    {
        inputActions = player.inputActions;
        if (inputActions == null)
        {
            return;
        }
        if (isActive)
        {
            inputActions.PlayerInput.Enable();
            inputActions.PlayerInput.Move.performed += context => moveInput = context.ReadValue<Vector2>();
            inputActions.PlayerInput.Move.canceled += context => moveInput = Vector2.zero;
            inputActions.PlayerInput.Run.performed += context => IsRunning(true);
            inputActions.PlayerInput.Run.canceled += context => IsRunning(false);
            inputActions.PlayerInput.CameraRotate.performed += context => CameraRotate();
            inputActions.PlayerInput.Jump.performed += OnJump;
        }
        else
        {
            inputActions.PlayerInput.Disable();
            inputActions.PlayerInput.Move.performed -= context => moveInput = context.ReadValue<Vector2>();
            inputActions.PlayerInput.Move.canceled -= context => moveInput = Vector2.zero;
            inputActions.PlayerInput.Run.performed -= context => IsRunning(true);
            inputActions.PlayerInput.Run.canceled -= context => IsRunning(false);
            inputActions.PlayerInput.CameraRotate.performed -= context => CameraRotate();
            inputActions.PlayerInput.Jump.performed -= OnJump;

        }
    }
    private void CameraRotate()
    {
        //rotX = Input.GetKey (KeyCode.Joystick1Button4);
        //rotY = Input.GetKey (KeyCode.Joystick1Button5);
        //rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotX = inputActions.PlayerInput.CameraRotate.ReadValue<Vector2>().x * sensitivity;
        //rotY = Input.GetAxis("Mouse Y") * sensitivity * .6f;
        rotY = inputActions.PlayerInput.CameraRotate.ReadValue<Vector2>().y * sensitivity * .6f;
        if (webGLRightClickRotation)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                CameraRotation(cam, rotX, rotY);
            }
        }
        else if (!webGLRightClickRotation)
        {
            CameraRotation(cam, rotX, rotY);
        }
    }
    private void IsRunning(bool isRunning)
    {
        if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = speed;
        }
        this.isRunning = isRunning;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!IsGrounded())
        {
            return;
        }
        verticalVelocity = Mathf.Sqrt(2 * jumpHeight * -currentGravity); // 计算起跳初速度
        jumpTimeDelay = Time.time;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(checkIsGround.position, Vector3.down, isGroundOffset);
    }
    public void AnimatorControllers(Vector3 movement)
    {
        float xVelocity = Vector3.Dot(movement.normalized, transform.right);
        float zVelocity = Vector3.Dot(movement.normalized, transform.forward);

        anim.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        anim.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);
        anim.SetBool("isRunning", isRunning);
    }
    void CameraRotation(GameObject cam, float rotX, float rotY)
    {
        transform.Rotate(0, rotX, 0);
        Vector3 currentRotation = cam.transform.eulerAngles;
        float newRotationX = currentRotation.x + (-rotY);
        if (newRotationX > 180f)
            newRotationX -= 360f;
        newRotationX = Mathf.Clamp(newRotationX, -80f, 89f);
        cam.transform.eulerAngles = new Vector3(newRotationX, currentRotation.y, currentRotation.z);
    }
    public void SetPlayerSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }
    private void OnDrawGizmos()
    {
        float rayLength = isGroundOffset;
        Vector3 rayOrigin = checkIsGround.position;
        Vector3 rayEnd = rayOrigin + Vector3.down * rayLength;

        // 检测是否接地（注意：Physics.Raycast 在 OnDrawGizmos 中可能不准确，建议只画线）
        // 所以我们不在这儿调用 Physics.Raycast，只画线

        Gizmos.color = Color.red;

        Gizmos.DrawLine(rayOrigin, rayEnd);
    }


}
