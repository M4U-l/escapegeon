using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;
    public Transform cameraTransform;

    public bool IsGrounded => controller.isGrounded;
    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;
    internal float movementSpeedMultiplier;

    CharacterController controller;
    Vector2 look;
    internal Vector3 velocity;
    private bool wasGrounded;

    PlayerInput playerInput;
    InputAction moveAction, lookAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGround();
        UpdateMovement();
        UpdateGravity();
        UpdateLook();
    }

    void UpdateGround()
    {
        if (wasGrounded != IsGrounded)
        {
            OnGroundStateChange?.Invoke(IsGrounded);
            wasGrounded = IsGrounded;
        }
    }

    Vector3 GetMovementInput()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        var input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed * movementSpeedMultiplier;
        return input;
    }

    void UpdateMovement()
    {
        movementSpeedMultiplier = 1f;
        OnBeforeMove?.Invoke();
        var input = GetMovementInput();
        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        controller.Move(velocity * Time.deltaTime);
    }

    public void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    void UpdateLook()
    {
        var lookInput = lookAction.ReadValue<Vector2>();

        look.x += lookInput.x * mouseSensitivity; 
        look.y += lookInput.y * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f); 

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);    
    }
}
