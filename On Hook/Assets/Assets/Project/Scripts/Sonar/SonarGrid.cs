using UnityEngine;
using UnityEngine.UI;

public class SonarGrid : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private int verticalLines = 8;
    [SerializeField] private int horizontalLines = 6;

    [Header("Appearance")]
    [SerializeField] private float lineWidth = 1f;
    [SerializeField] private float lineAlpha = 0.15f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int i = 1; i < verticalLines; i++)
        {
            float x =
                (float)i / verticalLines;

            CreateVerticalLine(x);
        }

        for (int i = 1; i < horizontalLines; i++)
        {
            float y =
                (float)i / horizontalLines;

            CreateHorizontalLine(y);
        }
    }

    private void CreateVerticalLine(float x)
    {
        GameObject lineObject =
            new GameObject(
                "VerticalLine",
                typeof(Image)
            );

        lineObject.transform.SetParent(
            transform,
            false
        );

        Image image =
            lineObject.GetComponent<Image>();

        image.color =
            new Color(
                1f,
                1f,
                1f,
                lineAlpha
            );

        RectTransform line =
            lineObject.GetComponent<RectTransform>();

        line.anchorMin =
            new Vector2(x, 0f);

        line.anchorMax =
            new Vector2(x, 1f);

        line.offsetMin =
            new Vector2(
                -lineWidth * 0.5f,
                0f
            );

        line.offsetMax =
            new Vector2(
                lineWidth * 0.5f,
                0f
            );
    }

    private void CreateHorizontalLine(float y)
    {
        GameObject lineObject =
            new GameObject(
                "HorizontalLine",
                typeof(Image)
            );

        lineObject.transform.SetParent(
            transform,
            false
        );

        Image image =
            lineObject.GetComponent<Image>();

        image.color =
            new Color(
                1f,
                1f,
                1f,
                lineAlpha
            );

        RectTransform line =
            lineObject.GetComponent<RectTransform>();

        line.anchorMin =
            new Vector2(0f, y);

        line.anchorMax =
            new Vector2(1f, y);

        line.offsetMin =
            new Vector2(
                0f,
                -lineWidth * 0.5f
            );

        line.offsetMax =
            new Vector2(
                0f,
                lineWidth * 0.5f
            );
    }
}