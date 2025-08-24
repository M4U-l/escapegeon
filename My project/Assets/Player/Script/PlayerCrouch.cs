using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMove))]
public class PlayerCrouch : MonoBehaviour
{
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    private PlayerMove player;
    private PlayerInput playerInput;
    private InputAction crouchAction;
    private Vector3 initialCameraPos;
    private float currentHeight;
    private float standingHeight;
    private bool IsCrouching => standingHeight - currentHeight > 0.1f;

    void Awake()
    {
        player = GetComponent<PlayerMove>();
        playerInput = GetComponent<PlayerInput>();
        crouchAction = playerInput.actions["Crouch"];
    }

    void Start()
    {
        initialCameraPos = player.cameraTransform.localPosition;
        standingHeight = currentHeight = player.Height;
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        var isTryingToCrouch = crouchAction.ReadValue<float>() > 0;

        var heightTarget = isTryingToCrouch ? crouchHeight : standingHeight;

        if (IsCrouching && !isTryingToCrouch)
        {
            var castOrigin = transform.position + new Vector3(0, currentHeight / 2, 0);
            if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.2f))
            {
                var distanceToCeiling = hit.point.y - castOrigin.y;
                heightTarget = Mathf.Max(currentHeight + distanceToCeiling - 0.01f, crouchHeight);
            }
        }

        if (!Mathf.Approximately(heightTarget, currentHeight))
        {
            var crouchDelta = Time.deltaTime * crouchTransitionSpeed;
            currentHeight = Mathf.Lerp(currentHeight, heightTarget, crouchDelta);

            var halfHeightDifference = new Vector3(0, (standingHeight - currentHeight) / 2, 0);
            var newCameraPos = initialCameraPos - halfHeightDifference;

            player.cameraTransform.localPosition = newCameraPos;
            player.Height = currentHeight;
        }

        if (IsCrouching)
        {
            player.movementSpeedMultiplier *= crouchSpeedMultiplier;
        }
    }
}
