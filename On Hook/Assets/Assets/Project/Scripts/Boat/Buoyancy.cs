
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Water")]
    [SerializeField] private WaterSurface water;

    [Header("Buoyancy")]
    [SerializeField] private float buoyancyForce = 1500f;
    [SerializeField] private float maxSubmersion = 1f;

    [Header("Damping")]
    [SerializeField] private float verticalDamping = 5f;

    [Header("Buoyancy Points")]
    [SerializeField] private Transform[] buoyancyPoints;

    [Header("Walking Mode")]
    [SerializeField] private float walkingHeight = 0f;
    [SerializeField] private float walkingForce = 5000f;
    [SerializeField] private float walkingDamping = 1000f;

    private Rigidbody rb;
    private bool walkingMode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetWalkingMode(bool enabled)
    {
        walkingMode = enabled;
    }

    private void FixedUpdate()
    {
        if (walkingMode)
        {
            ApplyWalkingBuoyancy();
            return;
        }

        foreach (Transform point in buoyancyPoints)
        {
            if (point != null)
                ApplyBuoyancy(point);
        }
    }

    private void ApplyWalkingBuoyancy()
    {
        float waterHeight = water.GetWaterHeight(
            transform.position.x,
            transform.position.z
        );

        // Поддерживаем корпус около заданной высоты над водой.
        float targetHeight = waterHeight + walkingHeight;
        float heightDifference = targetHeight - transform.position.y;

        float verticalVelocity = rb.linearVelocity.y;

        float force =
            heightDifference * walkingForce -
            verticalVelocity * walkingDamping;

        rb.AddForce(
            Vector3.up * force,
            ForceMode.Force
        );
    }

    private void ApplyBuoyancy(Transform point)
    {
        float waterHeight = water.GetWaterHeight(
            point.position.x,
            point.position.z
        );

        float depth = waterHeight - point.position.y;

        if (depth <= 0f)
            return;

        float submersion = Mathf.Clamp01(depth / maxSubmersion);

        Vector3 force = Vector3.up * buoyancyForce * submersion;

        rb.AddForceAtPosition(
            force,
            point.position,
            ForceMode.Force
        );

        float verticalVelocity =
            rb.GetPointVelocity(point.position).y;

        Vector3 dampingForce =
            Vector3.down *
            verticalVelocity *
            verticalDamping *
            submersion;

        rb.AddForceAtPosition(
            dampingForce,
            point.position,
            ForceMode.Force
        );
    }
}
