using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionHint : MonoBehaviour
{
    [Header("Scene")]
    public string sceneName = "Scene_2D";

    [Header("Interaction")]
    public Transform player;              // ������ �� ������ / �������
    public float interactDistance = 0.85f;   // ���������, ��� ������� ���������� ���������

    [Header("World Hint UI (World Space Canvas)")]
    public Canvas worldCanvas;            // Canvas (Render Mode = World Space)
    public Text hintText;                 // UI Text ������ Canvas (�������� "������� E")
    public string hintString = "������� E";

    private InputAction interactAction;
    private bool isInRange = false;
    private Camera mainCam;

    void Start()
    {
        // ������� ��������� �� ���������
        if (worldCanvas != null)
            worldCanvas.gameObject.SetActive(false);

        // ������� ����� ������ PlayerInput � action "Interact"
        var playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null && playerInput.actions != null)
        {
            // ��� action � "Interact" (� ���� ������ ���� ��� �������)
            interactAction = playerInput.actions["Interact"];
        }
        else
        {
            Debug.LogWarning("[SceneTransitionWithHint] PlayerInput ��� actions �� ������� � �����.");
        }

        mainCam = Camera.main;

        // ���� ���� ����� � �������� ������ ������ (������)
        if (hintText != null)
            hintText.text = hintString;
    }

    void Update()
    {
        if (player == null || worldCanvas == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        // ����� � ���� ��������������
        if (dist <= interactDistance)
        {
            if (!isInRange)
            {
                isInRange = true;
                ShowHint(true);
            }

            // ���������� ���� �������� �� ������
            if (mainCam != null)
            {
                // ������ ���, ����� ��������� �����-������� "��������" �� ������ �����
                Vector3 dir = worldCanvas.transform.position - mainCam.transform.position;
                if (dir.sqrMagnitude > 0.001f)
                    worldCanvas.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }

            // �������� ������� ������ ��������������
            if (interactAction != null && interactAction.WasPressedThisFrame())
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        // ����� ����� �� ����
        else if (isInRange)
        {
            isInRange = false;
            ShowHint(false);
        }
    }

    private void ShowHint(bool show)
    {
        if (worldCanvas != null)
            worldCanvas.gameObject.SetActive(show);
    }

    // ���������� ������� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
