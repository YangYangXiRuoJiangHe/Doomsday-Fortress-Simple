using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions inputActions;
    private void Awake()
    {
        if (inputActions == null)
        {
            inputActions = new InputActions();
        }
    }
}
