using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Engine")]
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float reverseAcceleration = 1.5f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float reverseMaxSpeed = 5f;

    [Header("Steering")]
    [SerializeField] private float turnAcceleration = 2.5f;
    [SerializeField] private float steeringResponse = 5f;

    [Header("Water Resistance")]
    [SerializeField] private float forwardDrag = 0.8f;
    [SerializeField] private float lateralDrag = 6f;

    [Header("Rotation")]
    [SerializeField] private float angularDamping = 2f;

    private Rigidbody rb;

    private float throttleInput;
    private float steeringInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularDamping = angularDamping;
    }

    private void Update()
    {
        throttleInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
        ApplyWaterResistance();
        LimitSpeed();
    }

    private void ApplyEngineForce()
    {
        if (Mathf.Abs(throttleInput) < 0.01f)
            return;

        float currentAcceleration =
            throttleInput > 0f
                ? acceleration
                : reverseAcceleration;

        Vector3 force =
            transform.forward *
            throttleInput *
            currentAcceleration;

        // Значение теперь является именно ускорением,
        // а не силой в Ньютонах.
        rb.AddForce(
            force,
            ForceMode.Acceleration
        );
    }

    private void ApplySteering()
    {
        float forwardSpeed =
            Vector3.Dot(
                rb.linearVelocity,
                transform.forward
            );

        float speedFactor =
            Mathf.Clamp01(
                Mathf.Abs(forwardSpeed) / 1.5f
            );

        if (speedFactor < 0.05f)
            return;

        float direction =
            forwardSpeed >= 0f ? 1f : -1f;

        float targetAngularAcceleration =
            steeringInput *
            turnAcceleration *
            speedFactor *
            direction;

        float currentAngularVelocity =
            rb.angularVelocity.y;

        float angularAcceleration =
            targetAngularAcceleration -
            currentAngularVelocity;

        rb.AddTorque(
            Vector3.up *
            angularAcceleration *
            steeringResponse,
            ForceMode.Acceleration
        );
    }

    private void ApplyWaterResistance()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 forwardVelocity =
            Vector3.Project(
                velocity,
                transform.forward
            );

        Vector3 lateralVelocity =
            velocity - forwardVelocity;

        rb.AddForce(
            -forwardVelocity * forwardDrag,
            ForceMode.Acceleration
        );

        rb.AddForce(
            -lateralVelocity * lateralDrag,
            ForceMode.Acceleration
        );
    }

    private void LimitSpeed()
    {
        Vector3 velocity = rb.linearVelocity;

        float forwardSpeed =
            Vector3.Dot(
                velocity,
                transform.forward
            );

        if (forwardSpeed > maxSpeed)
        {
            Vector3 excessVelocity =
                transform.forward *
                (forwardSpeed - maxSpeed);

            rb.linearVelocity -= excessVelocity;
        }

        if (forwardSpeed < -reverseMaxSpeed)
        {
            Vector3 excessVelocity =
                transform.forward *
                (forwardSpeed + reverseMaxSpeed);

            rb.linearVelocity -= excessVelocity;
        }
    }
}