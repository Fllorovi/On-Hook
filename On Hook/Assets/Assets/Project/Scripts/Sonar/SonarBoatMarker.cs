using UnityEngine;

public class SonarBoatMarker : MonoBehaviour
{
    [SerializeField] private Transform boat;

    private void Update()
    {
        if (boat == null)
            return;

        float angle =
            -boat.eulerAngles.y;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }
}