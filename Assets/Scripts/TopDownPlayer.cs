using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TopDownPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector2 moveInput;

    public PlayerInput playerInput; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Подписка на InputSystem
    private void OnEnable()
    {
        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput != null)
        {
            var moveAction = playerInput.actions["Move"];
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }
    }

    private void OnDisable()
    {
        if (playerInput == null) { 
            playerInput = FindFirstObjectByType<PlayerInput>(); 
        }
        if (playerInput != null)
        {
            var moveAction = playerInput.actions["Move"];
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);

        // Поворот по направлению движения
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.2f));
        }
    }
}
