using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public NewSkateMovement skateMovement;
    public CameraMove cameraMove;
    public MenuController menuController;

    // Variable GLOBAL para que los 77 pickups sepan si estás apretando Tab/RB a la vez
    public static bool isOutlinePressed;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.JumpBrake.performed += ctx => { if (skateMovement != null) skateMovement.Jump(); };
        controls.Gameplay.Charge.started += ctx => { if (skateMovement != null) skateMovement.OnSteerChargePressed(); };
        controls.Gameplay.Charge.canceled += ctx => { if (skateMovement != null) skateMovement.OnSteerChargeReleased(); };
        controls.Gameplay.QuickTurn.performed += ctx => { if (cameraMove != null) cameraMove.QuickTurn180(); };

        controls.Gameplay.CheatWin.performed += ctx =>
        {
            Pickup[] allPickups = FindObjectsByType<Pickup>(FindObjectsSortMode.None);
            foreach (var p in allPickups) p.InstaWin();
        };
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        if (skateMovement == null) skateMovement = FindFirstObjectByType<NewSkateMovement>();
        if (cameraMove == null) cameraMove = FindFirstObjectByType<CameraMove>();
        if (menuController == null) menuController = FindFirstObjectByType<MenuController>();
    }

    void Update()
    {
        if (menuController != null && menuController.isPaused)
        {
            if (cameraMove != null) cameraMove.inputDir = Vector2.zero;
            return;
        }

        if (skateMovement != null) skateMovement.stop = controls.Gameplay.JumpBrake.IsPressed();
        if (cameraMove != null) cameraMove.inputDir = controls.Gameplay.Look.ReadValue<Vector2>();

        // Lee el estado del botón en tiempo real y lo guarda en la variable global
        isOutlinePressed = controls.Gameplay.Outline.IsPressed();
    }
}