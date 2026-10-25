
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatStability : MonoBehaviour
{
    [Header("Walking Mode")]
    [SerializeField] private float uprightStrength = 25f;
    [SerializeField] private float rotationDamping = 10f;

    private Rigidbody rb;

    public bool WalkingMode { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetWalkingMode(bool enabled)
    {
        WalkingMode = enabled;
    }

    private void FixedUpdate()
    {
        if (!WalkingMode)
            return;

        // Возвращаем лодку в горизонтальное положение.
        Vector3 correction =
            Vector3.Cross(transform.up, Vector3.up);

        rb.AddTorque(
            correction * uprightStrength,
            ForceMode.Acceleration
        );

        // Подавляем вращение по крену и тангажу,
        // не мешая лодке поворачиваться вокруг вертикальной оси.
        Vector3 tiltAngularVelocity =
            Vector3.ProjectOnPlane(
                rb.angularVelocity,
                Vector3.up
            );

        rb.AddTorque(
            -tiltAngularVelocity * rotationDamping,
            ForceMode.Acceleration
        );

        // Дополнительно гасим остаточное вращение,
        // чтобы лодка быстрее успокаивалась после шага игрока.
        rb.angularVelocity -=
            tiltAngularVelocity *
            Mathf.Clamp01(15f * Time.fixedDeltaTime);
    }
}
