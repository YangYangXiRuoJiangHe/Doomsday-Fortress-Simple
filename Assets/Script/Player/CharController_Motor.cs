using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame
{
    public class CharController_Motor : MonoBehaviour
    {
        [Header("移动详情")]
        public float speed = 10.0f;
        public float runSpeed = 30f;
        public float verticalVelocity = 0f;
        public float isGroundOffset = 1f;
        public Transform checkIsGround;
        public float jumpHeight = 2;
        public float sensitivity = 30.0f;
        public float WaterHeight = 15.5f;
        public float jumpTime = .2f;
        public float jumpTimeDelay = 0f;
        float gravity = -9.8f;
        public float gravityMultiple = 1;
        [Header("摄象机详情")]
        public CharacterController character;
        public GameObject cam;
        float moveFB, moveLR;
        float rotX, rotY;
        public bool webGLRightClickRotation = true;
        [Header("输入系统")]
        private InputActions inputActions;
        [Header("动画详情")]
        public Animator anim;
        private void Awake()
        {
            if (inputActions == null)
            {
                inputActions = new InputActions();
            }
            anim = GetComponentInChildren<Animator>();
        }
        void Start()
        {
            //LockCursor ();
            character = GetComponent<CharacterController>();
            if (Application.isEditor)
            {
                webGLRightClickRotation = false;
            }
        }
        void Update()
        {
            Movement();
        }

        private void Movement()
        {
            Vector2 move = inputActions.PlayerInput.Move.ReadValue<Vector2>();
            float currentSpeed = speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = speed;
            }
            if (!IsGrounded())
            {
                verticalVelocity += gravity * Time.deltaTime * gravityMultiple; // 累加重力
            }
            moveFB = move.x * currentSpeed;
            moveLR = move.y * currentSpeed;

            rotX = Input.GetAxis("Mouse X") * sensitivity;
            rotY = Input.GetAxis("Mouse Y") * sensitivity * .6f;

            //rotX = Input.GetKey (KeyCode.Joystick1Button4);
            //rotY = Input.GetKey (KeyCode.Joystick1Button5);
            //后期如果开发游泳用
            CheckForWaterHeight();
            Vector3 movement = new Vector3(moveFB, verticalVelocity, moveLR);

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
            movement = transform.rotation * movement;
            AnimatorControllers(movement);
            character.Move(movement * Time.deltaTime);
        }

        void CheckForWaterHeight()
        {
            if (transform.position.y < WaterHeight)
            {
                gravity = 0f;
            }
            else
            {
                gravity = -9.8f;
            }
        }
        public void SetPlayerInput(bool isActive)
        {
            if (inputActions == null)
            {
                return;
            }
            if (isActive)
            {
                inputActions.PlayerInput.Enable();
                inputActions.PlayerInput.Jump.performed += OnJump;

            }
            else
            {

                inputActions.PlayerInput.Disable();
                inputActions.PlayerInput.Jump.performed -= OnJump;
            }
        }
        private void OnJump(InputAction.CallbackContext context)
        {
            if (!IsGrounded())
            {
                return;
            }
            Debug.Log("Jump");
            verticalVelocity = Mathf.Sqrt(2 * jumpHeight * -gravity); // 计算起跳初速度
            Debug.Log(verticalVelocity);
            jumpTimeDelay = Time.time;
        }
        private bool IsGrounded()
        {
            return Physics.Raycast(checkIsGround.position, Vector3.down,isGroundOffset);
        }
        public void AnimatorControllers(Vector3 movement)
        {
            float xVelocity = Vector3.Dot(movement.normalized, transform.right);
            float zVelocity = Vector3.Dot(movement.normalized, transform.forward);

            anim.SetFloat("xVelocity", xVelocity,.1f,Time.deltaTime);
            anim.SetFloat("zVelocity", zVelocity,.1f,Time.deltaTime);
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
}
