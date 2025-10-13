using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionHint : MonoBehaviour
{
    [Header("Scene")]
    public string sceneName = "Scene_2D";

    [Header("Interaction")]
    public Transform player;              // ссылка на игрока / коляску
    public float interactDistance = 0.85f;   // дистанция, при которой появляется подсказка

    [Header("World Hint UI (World Space Canvas)")]
    public Canvas worldCanvas;            // Canvas (Render Mode = World Space)
    public Text hintText;                 // UI Text внутри Canvas (содержит "Нажмите E")
    public string hintString = "Нажмите E";

    private InputAction interactAction;
    private bool isInRange = false;
    private Camera mainCam;

    void Start()
    {
        // Спрячем подсказку по умолчанию
        if (worldCanvas != null)
            worldCanvas.gameObject.SetActive(false);

        // Попытка найти объект PlayerInput и action "Interact"
        var playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null && playerInput.actions != null)
        {
            // Имя action — "Interact" (у тебя должно быть так названо)
            interactAction = playerInput.actions["Interact"];
        }
        else
        {
            Debug.LogWarning("[SceneTransitionWithHint] PlayerInput или actions не найдены в сцене.");
        }

        mainCam = Camera.main;

        // если есть текст — выставим нужную строку (локаль)
        if (hintText != null)
            hintText.text = hintString;
    }

    void Update()
    {
        if (player == null || worldCanvas == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        // игрок в зоне взаимодействия
        if (dist <= interactDistance)
        {
            if (!isInRange)
            {
                isInRange = true;
                ShowHint(true);
            }

            // заставляем хинт смотреть на камеру
            if (mainCam != null)
            {
                // делаем так, чтобы плоскость текст-канавас "смотрела" на камеру лицом
                Vector3 dir = worldCanvas.transform.position - mainCam.transform.position;
                if (dir.sqrMagnitude > 0.001f)
                    worldCanvas.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }

            // проверка нажатия кнопки взаимодействия
            if (interactAction != null && interactAction.WasPressedThisFrame())
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        // игрок вышел из зоны
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

    // визуальная отладка в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
