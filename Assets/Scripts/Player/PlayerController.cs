using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private float horizontalMaxSpeed = 5f;
    [SerializeField] private float horizontalAccelTime = 0.1f;
    [SerializeField] private float horizontalDecelTime = 0.1f;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float sprintMultiplier = 1.5f;

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
            isSprinting = true;
        }
        else if (value.canceled)
        {
            isSprinting = false;
        }
    }

    void SetHorizontalSpeed()
    {
        if (horizontalInputValue.magnitude > 0f)
        {
            horizontalDirection = horizontalInputValue;
            float modifiedMaxSpeed = horizontalMaxSpeed * (isSprinting ? sprintMultiplier : 1);
            horizontalSpeed = Vector2.SmoothDamp(horizontalSpeed, horizontalDirection * modifiedMaxSpeed, ref horizontalAccel, horizontalAccelTime);
        }
        else
        {
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
            transform.rotation = Quaternion.Lerp(transform.rotation, facingAngle, rotationSpeed * Time.deltaTime);
        }
    }
}
