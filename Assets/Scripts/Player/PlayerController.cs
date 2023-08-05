using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public float horizontalMaxSpeed = 5f;
    public float horizontalAccelTime = 0.1f;
    public float horizontalDecelTime = 0.1f;
    public float rotationSpeed = 20f;
    public float sprintMultiplier = 1.5f;
    [SerializeField] private Transform playerRotator;
    [SerializeField] private Animator animator;

    private bool isSprinting = false;

    private Vector2 horizontalInputValue;
    private Vector2 horizontalDirection;
    private Vector2 horizontalSpeed = Vector2.zero;
    private Vector2 horizontalAccel;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        SetHorizontalSpeed();
        Move();
        Rotate(horizontalDirection);
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        horizontalInputValue = value.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            animator.SetBool("Sprinting", true);
            isSprinting = true;
        }
        else if (value.canceled)
        {
            animator.SetBool("Sprinting", false);
            isSprinting = false;
        }
    }

    void SetHorizontalSpeed()
    {
        if (horizontalInputValue.magnitude > 0f)
        {
            animator.SetBool("Moving", true);
            horizontalDirection = horizontalInputValue;
            float modifiedMaxSpeed = horizontalMaxSpeed * (isSprinting ? sprintMultiplier : 1);
            horizontalSpeed = Vector2.SmoothDamp(horizontalSpeed, horizontalDirection * modifiedMaxSpeed, ref horizontalAccel, horizontalAccelTime);
        }
        else
        {
            animator.SetBool("Moving", false);
            horizontalSpeed = Vector2.SmoothDamp(horizontalSpeed, Vector2.zero, ref horizontalAccel, horizontalDecelTime);
            if (horizontalSpeed.magnitude < 0.1f)
            {
                horizontalSpeed = Vector3.zero;
            }
        }
    }

    void Move()
    {
        // aplying movement
        Vector3 motion = new Vector3(horizontalSpeed.x * Time.deltaTime, 0, horizontalSpeed.y * Time.deltaTime);
        characterController.Move(motion);
    }

    void Rotate(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            Quaternion facingAngle = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
            playerRotator.localRotation = Quaternion.Lerp(playerRotator.localRotation, facingAngle, rotationSpeed * Time.deltaTime);
        }
    }
}
