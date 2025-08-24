using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMove))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpPressBufferTime = 0.05f;
    [SerializeField] private float jumpGroundGraceTime = 0.2f;
    private PlayerMove player;
    private bool tryingToJump;
    float lastJumpPressTime;
    float lastGroundedTime;


    void Awake()
    {
        player = GetComponent<PlayerMove>();
    }

    void OnEnable()
    {
        player.OnBeforeMove += OnBeforeMove;
        player.OnGroundStateChange += OnGroundStateChange;
    }
    void OnDisable()
    {
        player.OnBeforeMove -= OnBeforeMove;
        player.OnGroundStateChange -= OnGroundStateChange;
    }


    void OnJump()
    {
        tryingToJump = true;
        lastJumpPressTime = Time.deltaTime;
    }

    void OnBeforeMove()
    {
        bool wasTryingToJump = Time.time - lastJumpPressTime < jumpPressBufferTime;
        bool wasGrounded = Time.time - lastGroundedTime < jumpGroundGraceTime;

        bool isOrWasTryingJump = tryingToJump || (wasTryingToJump && player.IsGrounded);
        bool isOrWasGrounded = player.IsGrounded || wasGrounded; 

        if (isOrWasTryingJump && isOrWasGrounded)
        {
            player.velocity.y += jumpForce;
        }
        tryingToJump = false;
    }

    void OnGroundStateChange(bool isGrounded)
    {
        if (!isGrounded) lastGroundedTime = Time.time;
    }
}
