using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatSpeed : MonoBehaviour
{
    private Rigidbody rb;

    public float SpeedMetersPerSecond { get; private set; }
    public float SpeedKilometersPerHour { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 horizontalVelocity =
            new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

        SpeedMetersPerSecond =
            horizontalVelocity.magnitude;

        SpeedKilometersPerHour =
            SpeedMetersPerSecond * 3.6f;
    }
}