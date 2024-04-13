using UnityEngine;

/// <summary>
/// Asteroids-style player controls:
///   A / D    rotate the ship counter-clockwise / clockwise
///   W        apply forward thrust along the ship's nose
///   S        brake (decelerate the ship smoothly toward zero)
/// All movement is omnidirectional and physics-based via Rigidbody2D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField] private float rotationSpeed = 220f;

    [Header("Thrust")]
    [Tooltip("Force applied while W is held. Higher = snappier acceleration.")]
    [SerializeField] private float thrustForce = 12f;

    [Tooltip("Maximum ship speed in units per second.")]
    [SerializeField] private float maxSpeed = 8f;

    [Header("Brake")]
    [Tooltip("How aggressively S decelerates the ship. Higher = stops faster.")]
    [SerializeField] private float brakeStrength = 6f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Reading input + visual rotation can run in Update.
        HandleRotation();
    }

    private void FixedUpdate()
    {
        // All physics goes in FixedUpdate.
        HandleThrust();
        HandleBrake();
        ClampSpeed();
    }

    private void HandleRotation()
    {
        float input = 0f;
        if (Input.GetKey(KeyCode.A)) input += 1f; // counter-clockwise
        if (Input.GetKey(KeyCode.D)) input -= 1f; // clockwise

        if (input != 0f)
        {
            transform.Rotate(0f, 0f, input * rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleThrust()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // transform.up = the direction the ship's nose currently points
            rb.AddForce(transform.up * thrustForce);
        }
    }

    private void HandleBrake()
    {
        if (Input.GetKey(KeyCode.S))
        {
            // Smoothly bring velocity toward zero. Frame-rate independent.
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                Vector2.zero,
                brakeStrength * Time.fixedDeltaTime
            );
        }
    }

    private void ClampSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}