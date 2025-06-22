using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputWrapper : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public bool ShootPressed { get; private set; }

    private GameInputActions inputActions; 

    private void Awake()
    {
        inputActions = new GameInputActions();

        inputActions.Player.Move.performed += ctx => MovementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => MovementInput = Vector2.zero;

        inputActions.Player.Shoot.performed += ctx => ShootPressed = true;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void LateUpdate()
    {
        // Reset Shoot each frame after being read
        ShootPressed = false;
    }
}
