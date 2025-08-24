using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMove))]
public class PlayerSprint : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 2f;
    PlayerMove player;
    PlayerInput playerInput;
    InputAction sprintAction;

    void Awake()
    {
        player = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["Sprint"];
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        var sprintInput = sprintAction.ReadValue<float>();
        if (sprintInput == 0) return;

        var forwardMovementFactor = Mathf.Clamp01(Vector3.Dot(player.transform.forward, player.velocity.normalized));
        var multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovementFactor);
        player.movementSpeedMultiplier *= multiplier;
    }
}
