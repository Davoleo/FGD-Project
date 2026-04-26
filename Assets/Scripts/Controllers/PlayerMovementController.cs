using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float stepAngle = 90f;


    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference rotateLeftAction;
    [SerializeField] private InputActionReference rotateRightAction;

    private Vector3 currentVelocity = Vector3.zero;
    private CharacterController characterController;
    private Transform cameraTransform;

    // TODO: Refactor private variables 
    private bool isRotating = false;
    private float rotationTimer = 1f;
    private float currentYAngle = 0;
    private float targetYAngle = 0;
    
    void OnEnable() => moveAction.action.Enable();
    void OnDisable() => moveAction.action.Disable();

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = GameObject.Find("PlayerCamera").transform;
    }

    void Update()
    {
        HandleMovementInput();
        ApplyMovement();
        HandleRotationInput();
        ApplyRotation();
    }

    // Transform movement input to camera space
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
     // //////
    void HandleRotationInput()
    {

        if (isRotating) return;

        if (rotateLeftAction.action.triggered)
        {
            targetYAngle -= stepAngle;
            StartRotation();
        }
        else if (rotateRightAction.action.triggered)
        {
            targetYAngle += stepAngle;
            StartRotation();
        }
    }

    void StartRotation()
    {
        currentYAngle = transform.eulerAngles.y;
        rotationTimer = 0f;
        isRotating = true;
    }

    void ApplyRotation()
    {
        if (!isRotating) return;

        rotationTimer += Time.deltaTime / 0.3f;
        rotationTimer = Mathf.Clamp01(rotationTimer);

        float t = Mathf.SmoothStep(0f, 1f, rotationTimer);

        float newY = Mathf.LerpAngle(currentYAngle, targetYAngle, t);

        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            newY,
            transform.eulerAngles.z
        );

        if (rotationTimer >= 1f)
        {
            isRotating = false;
            targetYAngle = Mathf.Round(targetYAngle / stepAngle) * stepAngle;
        }
    }
}