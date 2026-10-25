using UnityEngine;

public class SonarTransducer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaterSurface water;
    [SerializeField] private BoatSpeed boatSpeed;

    [Header("Sonar")]
    [SerializeField] private float maxDepth = 100f;

    [Header("Mapping Speed")]
    [SerializeField] private float minimumScanSpeed = 0.2f;
    [SerializeField] private float maximumScanSpeed = 2f;

    public float CurrentDepth { get; private set; }

    public bool HasBottomHit { get; private set; }

    public bool IsScanning
    {
        get
        {
            return
                boatSpeed.SpeedMetersPerSecond >= minimumScanSpeed &&
                boatSpeed.SpeedMetersPerSecond <= maximumScanSpeed;
        }
    }

    private void FixedUpdate()
    {
        ScanBottom();
    }

    private void ScanBottom()
    {
        Ray ray = new Ray(
            transform.position,
            Vector3.down
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            maxDepth
        ))
        {
            float waterHeight =
                water.GetWaterHeight(
                    transform.position.x,
                    transform.position.z
                );

            CurrentDepth =
                waterHeight - hit.point.y;

            CurrentDepth =
                Mathf.Max(0f, CurrentDepth);

            HasBottomHit = true;
        }
        else
        {
            CurrentDepth = 0f;
            HasBottomHit = false;
        }
    }
}