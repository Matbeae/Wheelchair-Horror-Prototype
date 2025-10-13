using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class WheelchairController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float turnSpeed = 100f; // скорость поворота A/D

    [Header("Look")]
    public Transform cameraPivot;
    public float sensitivity = 2f;
    public float maxHeadTurn = 60f; // ограничение поворота головы влево/вправо
    public float maxLookX = 60f;    // ограничение вверх/вниз
    public float minLookX = -40f;

    private Rigidbody rb;
    private Vector2 moveInput;   // y = вперед/назад, x = поворот
    private Vector2 lookInput;

    private Vector3 currentVelocity;
    private float camRotationX = 0f;   // наклон вверх/вниз
    private float headYawOffset = 0f;  // поворот головы влево/вправо

    [Header("Audio")]
    public AudioSource wheelchairSound1;
    public AudioSource wheelchairSound2;
    public AudioSource wheelchairSound3;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // ==== InputSystem events ====
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    // ==== Движение ====
    void FixedUpdate()
    {
        // Вперёд/назад
        Vector3 forward = transform.forward * moveInput.y;
        Vector3 desiredVel = forward.normalized * moveSpeed;

        currentVelocity = Vector3.MoveTowards(
            new Vector3(currentVelocity.x, 0, currentVelocity.z),
            desiredVel,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);

        // Поворот A/D
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            float rotation = moveInput.x * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotation, 0));
        }
        bool isMoving = moveInput.y != 0f || Mathf.Abs(moveInput.x) > 0f;
        if (isMoving && !wheelchairSound1.isPlaying && !wheelchairSound2.isPlaying && !wheelchairSound3.isPlaying)
        {
            wheelchairSound1.Play();
            wheelchairSound2.Play();
            wheelchairSound3.Play();
        }
        else if (!isMoving && wheelchairSound1.isPlaying && wheelchairSound2.isPlaying && wheelchairSound3.isPlaying)
        {
            wheelchairSound1.Stop();
            wheelchairSound2.Stop();
            wheelchairSound3.Stop();
        }
    }

    // ==== Осмотр ====
    void LateUpdate()
    {
        // Горизонт мыши — поворот головы (ограниченный угол)
        headYawOffset += lookInput.x * sensitivity;
        headYawOffset = Mathf.Clamp(headYawOffset, -maxHeadTurn, maxHeadTurn);

        // Вертикаль мыши — наклон головы вверх/вниз
        camRotationX -= lookInput.y * sensitivity;
        camRotationX = Mathf.Clamp(camRotationX, minLookX, maxLookX);

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(camRotationX, headYawOffset, 0);
        }
    }
}
