using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterSurface : MonoBehaviour
{
    [Header("Water")]
    [SerializeField] private float waterLevel = 0f;
    [SerializeField] private float waterSize = 500f;

    [Header("Grid")]
    [SerializeField] private int resolution = 200;

    [Header("Waves")]
    [SerializeField] private float waveHeight = 0.20f;
    [SerializeField] private float waveLength = 18f;
    [SerializeField] private float waveSpeed = 1.5f;

    [Header("Secondary Wave")]
    [SerializeField] private float secondaryHeight = 0.06f;
    [SerializeField] private float secondaryLength = 9f;
    [SerializeField] private float secondarySpeed = 2.2f;

    private Mesh waterMesh;
    private Vector3[] vertices;

    public float WaterLevel => waterLevel;

    private void Awake()
    {
        GenerateMesh();
    }

    private void Update()
    {
        UpdateWaveMesh();
    }

    private void GenerateMesh()
    {
        waterMesh = new Mesh();
        waterMesh.name = "Procedural Water";

        int vertexCount = (resolution + 1) * (resolution + 1);

        vertices = new Vector3[vertexCount];

        Vector2[] uv = new Vector2[vertexCount];

        int[] triangles =
            new int[resolution * resolution * 6];

        float step =
            waterSize / resolution;

        int vertexIndex = 0;

        for (int z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float worldX =
                    -waterSize * 0.5f + x * step;

                float worldZ =
                    -waterSize * 0.5f + z * step;

                vertices[vertexIndex] =
                    new Vector3(
                        worldX,
                        0f,
                        worldZ
                    );

                uv[vertexIndex] =
                    new Vector2(
                        (float)x / resolution,
                        (float)z / resolution
                    );

                vertexIndex++;
            }
        }

        int triangleIndex = 0;

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int current =
                    z * (resolution + 1) + x;

                int nextRow =
                    (z + 1) * (resolution + 1) + x;

                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = nextRow;
                triangles[triangleIndex++] = current + 1;

                triangles[triangleIndex++] = current + 1;
                triangles[triangleIndex++] = nextRow;
                triangles[triangleIndex++] = nextRow + 1;
            }
        }

        waterMesh.vertices = vertices;
        waterMesh.triangles = triangles;
        waterMesh.uv = uv;

        waterMesh.RecalculateNormals();
        waterMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = waterMesh;
    }

    private void UpdateWaveMesh()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            float height =
                GetWaveHeight(
                    vertex.x + transform.position.x,
                    vertex.z + transform.position.z
                );

            vertex.y = height;

            vertices[i] = vertex;
        }

        waterMesh.vertices = vertices;
        waterMesh.RecalculateNormals();
        waterMesh.RecalculateBounds();
    }

    public float GetWaterHeight(
        float worldX,
        float worldZ
    )
    {
        return waterLevel +
               GetWaveHeight(worldX, worldZ);
    }

    private float GetWaveHeight(
        float worldX,
        float worldZ
    )
    {
        float time = Time.time;

        float primaryWave =
            Mathf.Sin(
                (worldX + worldZ * 0.35f)
                / waveLength
                + time * waveSpeed
            ) * waveHeight;

        float secondaryWave =
            Mathf.Sin(
                (worldX * 0.7f - worldZ)
                / secondaryLength
                + time * secondarySpeed
            ) * secondaryHeight;

        return primaryWave + secondaryWave;
    }

    public bool IsUnderwater(Vector3 position)
    {
        return position.y <
               GetWaterHeight(
                   position.x,
                   position.z
               );
    }

    public float GetSubmersion(
        Vector3 position,
        float maxDepth
    )
    {
        float waterHeight =
            GetWaterHeight(
                position.x,
                position.z
            );

        float depth =
            waterHeight - position.y;

        if (depth <= 0f)
            return 0f;

        return Mathf.Clamp01(
            depth / maxDepth
        );
    }
}