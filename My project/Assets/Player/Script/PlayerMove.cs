using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float climbingSpeed = 2f;
    [SerializeField] float mass = 2f;
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
    private State _state;
    public State CurrentState
    {
        get => _state;
        set
        {
            _state = value;
            velocity = Vector3.zero;
        }
    }
    public enum State
    {
        Walking,
        Climbing
    }

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
        movementSpeedMultiplier = 1f;
        switch (CurrentState)
        {
            case State.Walking:
                UpdateGround();
                UpdateMovement();
                UpdateGravity();
                UpdateLook();
                break;
            case State.Climbing:
                UpdateMovementClimbing();
                UpdateLook();
                break;
        }
    }


    void UpdateGround()
    {
        if (wasGrounded != IsGrounded)
        {
            OnGroundStateChange?.Invoke(IsGrounded);
            wasGrounded = IsGrounded;
        }
    }

    Vector3 GetMovementInput(float speed, bool horizontal = true)
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        var input = new Vector3();
        var referenceTransform = horizontal ? transform : cameraTransform; 
        input += referenceTransform.forward * moveInput.y;
        input += referenceTransform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed * movementSpeedMultiplier;
        return input;
    }

    void UpdateMovement()
    {
        OnBeforeMove?.Invoke();

        var input = GetMovementInput(movementSpeed);

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateMovementClimbing()
    {
        var input = GetMovementInput(climbingSpeed, false);
        var forwardInputFactor = Vector3.Dot(transform.forward, input.normalized);

        if (forwardInputFactor > 0)
        {
            input.x = input.x * 0.5f;
            input.y = input.z * 0.5f;

            if (Mathf.Abs(input.y) > 2f)
            {
                input.y = Mathf.Sign(input.y) * climbingSpeed;
                Debug.DrawLine(transform.position, transform.position + input, Color.red, 3f);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + input, Color.yellow, 3f);
            }
        }
        else
        {
            input.y = 0;
            input.x = input.x * 3f;
            input.z = input.z * 3f;
            Debug.DrawLine(transform.position, transform.position + input, Color.green, 3f);
        }

        var factor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, factor);

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
