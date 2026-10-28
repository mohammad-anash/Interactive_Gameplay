using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 4f;

    [Header("Jump and Gravity")]
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _jumpHeight = 1f;

    private const float GroundedPushDown = -2f;

    private CharacterController _controller;
    private float _verticalVelocity;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = ReadMoveInput();
        bool jumpPressed = ReadJumpInput();

        // Direction relative to where the player is facing
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        ApplyGravityAndJump(jumpPressed);

        Vector3 velocity = move * _moveSpeed;
        velocity.y = _verticalVelocity;

        _controller.Move(velocity * Time.deltaTime);
    }

    private Vector2 ReadMoveInput()
    {
        // No walking while a UI is open (cursor is free)
        if (Keyboard.current == null || Cursor.lockState != CursorLockMode.Locked)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;

        return input.normalized;
    }

    private bool ReadJumpInput()
    {
        return Keyboard.current != null &&
               Cursor.lockState == CursorLockMode.Locked &&
               Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    private void ApplyGravityAndJump(bool jumpPressed)
    {
        // A small push down keeps the player glued to the floor
        if (_controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = GroundedPushDown;

        if (_controller.isGrounded && jumpPressed)
            _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _verticalVelocity += _gravity * Time.deltaTime;
    }
}