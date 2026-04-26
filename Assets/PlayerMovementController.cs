using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;

    private CharacterController characterController;
    private Vector3 currentVelocity = Vector3.zero;

    void OnEnable() => moveAction.action.Enable();
    void OnDisable() => moveAction.action.Disable();

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovementInput();
        ApplyMovement();
    }

    void HandleMovementInput()
    {
        Vector2 inputValue = moveAction.action.ReadValue<Vector2>();

        if (inputValue.magnitude > 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight   = cameraTransform.right;
            camForward.y = 0f;
            camRight.y   = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camForward * inputValue.y + camRight * inputValue.x).normalized;

            currentVelocity = Vector3.Lerp(
                currentVelocity,
                moveDir * moveSpeed,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }
    }

    void ApplyMovement()
    {
        if (!characterController.isGrounded)
            currentVelocity.y -= 9.81f * Time.deltaTime;
        else
            currentVelocity.y = 0f;

        characterController.Move(currentVelocity * Time.deltaTime);
    }

    public Vector3 GetCurrentVelocity() => currentVelocity;
    public float GetMovementMagnitude() => new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
}