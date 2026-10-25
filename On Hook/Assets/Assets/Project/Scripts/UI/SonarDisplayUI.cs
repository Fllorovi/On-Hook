using UnityEngine;
using TMPro;

public class SonarDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoatSpeed boatSpeed;
    [SerializeField] private SonarTransducer sonar;

    [Header("UI")]
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text depthText;

    private void Update()
    {
        speedText.text =
            "SPEED\n" +
            boatSpeed.SpeedKilometersPerHour.ToString("0.0") +
            " km/h";

        if (sonar.HasBottomHit)
        {
            depthText.text =
                "DEPTH\n" +
                sonar.CurrentDepth.ToString("0.0") +
                " m";
        }
        else
        {
            depthText.text =
                "DEPTH\n--";
        }
    }
}