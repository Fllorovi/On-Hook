
using UnityEngine;

public class BoatInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform driverSeat;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform cameraPoint;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;

    private GameObject player;
    private PlayerController playerController;
    private CharacterController characterController;
    private BoatController boatController;
    private Buoyancy buoyancy;
    private Transform playerCamera;

    private bool playerInside;

    private void Awake()
    {
        boatController = GetComponent<BoatController>();
        buoyancy = GetComponent<Buoyancy>();
    }

    private void Update()
    {
        if (player == null)
            FindPlayer();

        if (player == null)
            return;

        if (playerInside)
        {
            if (Input.GetKeyDown(KeyCode.E))
                ExitBoat();
        }
        else
        {
            TryEnterBoat();
        }
    }

    private void FindPlayer()
    {
        GameObject foundPlayer =
            GameObject.FindGameObjectWithTag("Player");

        if (foundPlayer == null)
            return;

        player = foundPlayer;

        playerController =
            player.GetComponent<PlayerController>();

        characterController =
            player.GetComponent<CharacterController>();

        Camera cameraComponent =
            player.GetComponentInChildren<Camera>();

        if (cameraComponent != null)
            playerCamera = cameraComponent.transform;
    }

    private void TryEnterBoat()
    {
        if (driverSeat == null || cameraPoint == null)
            return;

        float distance = Vector3.Distance(
            player.transform.position,
            transform.position
        );

        if (distance > interactionDistance)
            return;

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        EnterBoat();
    }

    private void EnterBoat()
    {
        playerInside = true;

        // Включаем обычную плавучесть.
        if (buoyancy != null)
            buoyancy.SetWalkingMode(false);

        // Отключаем управление персонажем.
        if (playerController != null)
            playerController.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        // Сажаем игрока на место водителя.
        player.transform.SetParent(driverSeat);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        // Перемещаем камеру к месту водителя.
        if (playerCamera != null)
        {
            playerCamera.SetParent(cameraPoint);
            playerCamera.localPosition = Vector3.zero;
            playerCamera.localRotation = Quaternion.identity;
        }

        // Включаем управление лодкой.
        if (boatController != null)
            boatController.enabled = true;
    }

    private void ExitBoat()
    {
        if (exitPoint == null)
        {
            Debug.LogWarning(
                "BoatInteraction: Exit Point не назначен!",
                this
            );
            return;
        }

        playerInside = false;

        // Включаем режим плавучести без раскачки от точек.
        if (buoyancy != null)
            buoyancy.SetWalkingMode(true);

        // Возвращаем камеру к игроку.
        if (playerCamera != null)
        {
            playerCamera.SetParent(player.transform);
            playerCamera.localPosition =
                new Vector3(0f, 0.65f, 0f);
            playerCamera.localRotation = Quaternion.identity;
        }

        // Отвязываем игрока от лодки и ставим на точку выхода.
        player.transform.SetParent(null);

        player.transform.SetPositionAndRotation(
            exitPoint.position,
            exitPoint.rotation
        );

        // Снова включаем управление персонажем.
        if (characterController != null)
            characterController.enabled = true;

        if (playerController != null)
            playerController.enabled = true;

        // Отключаем управление лодкой.
        if (boatController != null)
            boatController.enabled = false;
    }
}
