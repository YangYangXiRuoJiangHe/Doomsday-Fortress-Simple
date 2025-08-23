using UnityEngine;

public class Player : MonoBehaviour
{
    public InputActions inputActions;
    private void Awake()
    {
        inputActions = new InputActions();
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
