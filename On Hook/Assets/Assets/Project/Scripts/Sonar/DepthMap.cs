using UnityEngine;
using UnityEngine.UI;

public class DepthMap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SonarTransducer sonar;

    [Header("Map")]
    [SerializeField] private int mapResolution = 256;
    [SerializeField] private float mapSize = 500f;

    [Header("Depth")]
    [SerializeField] private float maxDepth = 30f;

    [Header("Scan Area")]
    [SerializeField] private int scanRadius = 15;
    [SerializeField] private float scanDistance = 20f;

    [Header("Navigation View")]
    [SerializeField] private RawImage mapImage;
    [SerializeField] private Transform boat;
    [SerializeField] private float zoom = 4f;

    private Texture2D depthTexture;

    private Vector3 lastScanPosition;
    private bool hasScannedBefore;

    private void Awake()
    {
        CreateMap();

        if (sonar != null)
        {
            lastScanPosition =
                sonar.transform.position;
        }
    }

    private void FixedUpdate()
    {
        UpdateMap();
        UpdateNavigationView();
    }

    private void CreateMap()
    {
        depthTexture =
            new Texture2D(
                mapResolution,
                mapResolution,
                TextureFormat.RGB24,
                false
            );

        depthTexture.name = "Depth Map";

        depthTexture.filterMode =
            FilterMode.Point;

        depthTexture.wrapMode =
            TextureWrapMode.Clamp;

        ClearMap();
    }

    private void ClearMap()
    {
        Color[] pixels =
            new Color[
                mapResolution *
                mapResolution
            ];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.lightBlue;
        }

        depthTexture.SetPixels(pixels);
        depthTexture.Apply();
    }

    private void UpdateMap()
    {
        if (sonar == null)
            return;

        if (!sonar.IsScanning)
            return;

        if (!sonar.HasBottomHit)
            return;

        Vector3 currentPosition =
            sonar.transform.position;

        if (hasScannedBefore)
        {
            float distance =
                Vector3.Distance(
                    currentPosition,
                    lastScanPosition
                );

            if (distance < scanDistance)
                return;
        }

        lastScanPosition =
            currentPosition;

        hasScannedBefore = true;

        PaintScanArea(
            currentPosition,
            sonar.CurrentDepth
        );

        depthTexture.Apply();
    }

    private void PaintScanArea(
        Vector3 worldPosition,
        float depth
    )
    {
        float normalizedX =
            worldPosition.x / mapSize;

        float normalizedZ =
            worldPosition.z / mapSize;

        int centerX =
            Mathf.RoundToInt(
                normalizedX *
                (mapResolution - 1)
            );

        int centerY =
            Mathf.RoundToInt(
                normalizedZ *
                (mapResolution - 1)
            );

        if (centerX < 0 ||
            centerX >= mapResolution ||
            centerY < 0 ||
            centerY >= mapResolution)
        {
            return;
        }

        Color depthColor =
            GetDepthColor(depth);

        PaintPentagon(
            centerX,
            centerY,
            scanRadius,
            depthColor
        );
    }

    private void PaintPentagon(
        int centerX,
        int centerY,
        int radius,
        Color color
    )
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                float distance =
                    Mathf.Sqrt(
                        x * x +
                        y * y
                    );

                if (distance > radius)
                    continue;

                float angle =
                    Mathf.Atan2(y, x);

                float normalizedAngle =
                    angle +
                    Mathf.PI * 2f;

                normalizedAngle %=
                    Mathf.PI * 2f;

                float segment =
                    Mathf.PI * 2f / 5f;

                float localAngle =
                    normalizedAngle % segment;

                float distanceToEdge =
                    radius *
                    Mathf.Cos(segment / 2f) /
                    Mathf.Cos(
                        localAngle -
                        segment / 2f
                    );

                if (distance <= distanceToEdge)
                {
                    int pixelX =
                        centerX + x;

                    int pixelY =
                        centerY + y;

                    if (pixelX < 0 ||
                        pixelX >= mapResolution ||
                        pixelY < 0 ||
                        pixelY >= mapResolution)
                    {
                        continue;
                    }

                    depthTexture.SetPixel(
                        pixelX,
                        pixelY,
                        color
                    );
                }
            }
        }
    }

    private Color GetDepthColor(float depth)
    {
        float normalizedDepth =
            Mathf.Clamp01(
                depth / maxDepth
            );

        if (normalizedDepth < 0.1f)
        {
            return Color.red;
        }

        if (normalizedDepth < 0.2f)
        {
            return Color.Lerp(
                Color.red,
                new Color(1f, 0.5f, 0f),
                (normalizedDepth - 0.1f) / 0.1f
            );
        }

        if (normalizedDepth < 0.3f)
        {
            return Color.Lerp(
                new Color(1f, 0.5f, 0f),
                Color.yellow,
                (normalizedDepth - 0.2f) / 0.1f
            );
        }

        if (normalizedDepth < 0.45f)
        {
            return Color.Lerp(
                Color.yellow,
                Color.green,
                (normalizedDepth - 0.3f) / 0.15f
            );
        }

        if (normalizedDepth < 0.6f)
        {
            return Color.Lerp(
                Color.green,
                Color.cyan,
                (normalizedDepth - 0.45f) / 0.15f
            );
        }

        if (normalizedDepth < 0.8f)
        {
            return Color.Lerp(
                Color.cyan,
                Color.blue,
                (normalizedDepth - 0.6f) / 0.2f
            );
        }

        return Color.Lerp(
            Color.blue,
            new Color(0f, 0f, 0.25f),
            (normalizedDepth - 0.8f) / 0.2f
        );
    }

    private void UpdateNavigationView()
    {
        if (mapImage == null ||
            boat == null)
        {
            return;
        }

        float viewSize =
            1f / zoom;

        float centerX =
            boat.position.x / mapSize;

        float centerY =
            boat.position.z / mapSize;

        centerX =
            Mathf.Clamp(
                centerX,
                viewSize * 0.5f,
                1f - viewSize * 0.5f
            );

        centerY =
            Mathf.Clamp(
                centerY,
                viewSize * 0.5f,
                1f - viewSize * 0.5f
            );

        mapImage.uvRect =
            new Rect(
                centerX - viewSize * 0.5f,
                centerY - viewSize * 0.5f,
                viewSize,
                viewSize
            );
    }

    public void ZoomIn()
    {
        zoom += 1f;

        zoom =
            Mathf.Clamp(
                zoom,
                1f,
                12f
            );

        Debug.Log(
            "ZOOM IN: " + zoom
        );
    }

    public void ZoomOut()
    {
        zoom -= 1f;

        zoom =
            Mathf.Clamp(
                zoom,
                1f,
                12f
            );

        Debug.Log(
            "ZOOM OUT: " + zoom
        );
    }

    public Texture2D GetTexture()
    {
        return depthTexture;
    }
}