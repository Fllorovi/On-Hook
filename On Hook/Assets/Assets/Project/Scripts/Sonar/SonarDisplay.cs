using UnityEngine;
using UnityEngine.UI;

public class SonarDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DepthMap depthMap;
    [SerializeField] private RawImage mapImage;

    private void Start()
    {
        mapImage.texture = depthMap.GetTexture();
    }
}