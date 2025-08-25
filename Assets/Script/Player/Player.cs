using UnityEngine;

public class Player : MonoBehaviour
{
    public InputActions inputActions;
    private void Awake()
    {
        inputActions = new InputActions();
    }
    public void SetPlayerInputIsActive(bool enable)
    {
        if (inputActions == null)
        {
            return;
        }
        if (enable)
        {
            inputActions.PlayerInput.Enable();
        }
        else
        {
            inputActions.PlayerInput.Disable();
        }
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
