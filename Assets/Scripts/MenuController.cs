using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using EasyTransition;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems; // NUEVO: Obligatorio para navegar menús con mando

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Indicadores UI")]
    [SerializeField] private TextMeshProUGUI currentFpsText;

    [Header("Configuración Mouse")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;

    [Header("Menu Principal")]
    [SerializeField] private GameObject pressAnyButtonScreen;
    [SerializeField] private GameObject mainButtonsScreen;
    [SerializeField] private GameObject optionsScreen; // NUEVO: Arrastra tu objeto --Options-- aquí

    [Header("Navegación Joystick")]
    [SerializeField] private GameObject primerBotonMenu; // NUEVO: Arrastra tu botón "Play" o "Options"
    [SerializeField] private GameObject primerBotonOpciones; // NUEVO: Arrastra tu Slider "Master" o el Dropdown de resolución

    public bool isPaused;
    private float duration = 1f;
    private float startScale = 1f;
    private Coroutine currentCoroutine;
    private Resolution[] resolutions;
    private float deltaTime = 0.0f;
    private string currentScene;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Start()
    {
        resolutions = Screen.resolutions;
        currentScene = SceneManager.GetActiveScene().name;

        if (resolutionDropdown != null) SetupResolutionDropdown();

        // Inicializar el Slider de Sensibilidad
        if (sensitivitySlider != null)
        {
            float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 2.5f);
            sensitivitySlider.value = savedSens;
            UpdateSensitivityText(savedSens);

            // Conecta el slider al código automáticamente
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }

        switch (currentScene)
        {
            case "MainMenu":
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (pressAnyButtonScreen != null) pressAnyButtonScreen.SetActive(true);
                if (mainButtonsScreen != null) mainButtonsScreen.SetActive(false);
                break;
            case "Credits":
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            default:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }

    // NUEVA FUNCIÓN: Se ejecuta cada vez que mueves la barra del Slider
    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        UpdateSensitivityText(value);

        // Si el jugador está jugando (no en el menú principal), le avisa a la cámara al instante
        CameraMove cam = FindFirstObjectByType<CameraMove>();
        if (cam != null)
        {
            cam.UpdateSensitivity();
        }
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            // Escribe el número con 1 decimal (ej: "2.5")
            sensitivityValueText.text = value.ToString("F0");
        }
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if (currentFpsText != null)
        {
            float fps = 1.0f / deltaTime;
            currentFpsText.text = "Framerate: " + fps.ToString("F0");
        }

        if (controls.Gameplay.AnyKey.WasPressedThisFrame()) HandleAnyKey();
        if (controls.Gameplay.Back.WasPressedThisFrame()) HandleCancel();
        if (controls.Gameplay.Pause.WasPressedThisFrame()) TogglePause();

        // Lógica de Restart / Next Level
        if (controls.Gameplay.Select.WasPressedThisFrame())
        {
            CountdownTimer timer = FindFirstObjectByType<CountdownTimer>();
            if (timer != null && timer.isGameOver)
            {
                if (timer.hasWon)
                    LoadLevel2();
                else
                    RestartGame();
            }
        }
    }

    public void TogglePause()
    {
        if (currentScene != "MainMenu" && currentScene != "Credits")
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void HandleAnyKey()
    {
        if (currentScene == "MainMenu" || currentScene == "Credits")
        {
            if (pressAnyButtonScreen != null && pressAnyButtonScreen.activeSelf)
            {
                pressAnyButtonScreen.SetActive(false);
                mainButtonsScreen.SetActive(true);

                // NUEVO: Selecciona el primer botón del menú al entrar
                if (primerBotonMenu != null)
                {
                    EventSystem.current.SetSelectedGameObject(primerBotonMenu);
                }
            }
        }
    }

    public void HandleCancel()
    {
        if (currentScene == "MainMenu" || currentScene == "Credits")
        {
            // NUEVO: Si estamos en Opciones, usamos Back para volver al menú principal
            if (optionsScreen != null && optionsScreen.activeSelf)
            {
                CloseOptions();
            }
            // Si estamos en el menú principal, volvemos a "Press Any Button"
            else if (mainButtonsScreen != null && mainButtonsScreen.activeSelf)
            {
                mainButtonsScreen.SetActive(false);
                pressAnyButtonScreen.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null); // Soltamos el control
            }
        }
    }

    // NUEVO: Llama a esta función desde el evento OnClick() de tu botón "Options"
    public void OpenOptions()
    {
        if (mainButtonsScreen) mainButtonsScreen.SetActive(false);
        if (optionsScreen) optionsScreen.SetActive(true);

        if (primerBotonOpciones != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonOpciones);
        }
    }

    // NUEVO: Llama a esta función desde el evento OnClick() de tu botón "Back" dentro de opciones
    public void CloseOptions()
    {
        if (optionsScreen) optionsScreen.SetActive(false);
        if (mainButtonsScreen) mainButtonsScreen.SetActive(true);

        if (primerBotonMenu != null)
        {
            EventSystem.current.SetSelectedGameObject(primerBotonMenu);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (gameplayUI) gameplayUI.SetActive(false);
        if (pauseUI) pauseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentCoroutine = StartCoroutine(SlowDownTime());

        // Selecciona el primer botón al pausar (puedes crear una variable primerBotonPausa si quieres)
        if (primerBotonMenu != null) EventSystem.current.SetSelectedGameObject(primerBotonMenu);
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (gameplayUI) gameplayUI.SetActive(true);
        if (pauseUI) pauseUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentCoroutine = StartCoroutine(SpeedUpTime());

        // Limpiamos la selección al volver a jugar
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void LoadGame() { Time.timeScale = 1f; TransitionManager.Instance().Transition("Level1", transition, 0f); }
    public void LoadLevel2() { Time.timeScale = 1f; TransitionManager.Instance().Transition("Level2", transition, 0f); }
    public void LoadMainMenu() { Time.timeScale = 1f; TransitionManager.Instance().Transition("MainMenu", transition, 0f); }
    public void RestartGame() { Time.timeScale = 1f; TransitionManager.Instance().Transition(SceneManager.GetActiveScene().buildIndex, transition, 0f); }
    public void LoadCredits() { Time.timeScale = 1f; TransitionManager.Instance().Transition("Credits", transition, 0f); }
    public void QuitGame() { Application.Quit(); }

    private IEnumerator SlowDownTime()
    {
        float elapsed = 0f;
        while (Time.timeScale > 0.01f)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, 0f, elapsed / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 0f;
    }

    private IEnumerator SpeedUpTime()
    {
        float elapsed = 0f;
        while (Time.timeScale < 0.99f)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0f, startScale, elapsed / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 1f;
    }

    public void SetResolution(int resolutionIndex) { Resolution resolution = resolutions[resolutionIndex]; Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); }
    public void SetFullscreen(bool isFullscreen) { Screen.fullScreen = isFullscreen; }
    public void SetTargetFramerate(int targetFPS) { QualitySettings.vSyncCount = 0; Application.targetFrameRate = targetFPS; }
    public void SetFramerateTo30() { SetTargetFramerate(30); }
    public void SetFramerateTo60() { SetTargetFramerate(60); }
    public void SetFramerateTo120() { SetTargetFramerate(120); }
    public void SetFramerateUnlimited() { SetTargetFramerate(-1); }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
}