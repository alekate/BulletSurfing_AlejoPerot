using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public NewSkateMovement skateMovement;
    public CameraMove cameraMove;
    public MenuController menuController;
    private void Start()
    {
        if (skateMovement == null)
            skateMovement = FindObjectOfType<NewSkateMovement>();

        if (cameraMove == null)
            cameraMove = FindObjectOfType<CameraMove>();

        if (cameraMove == null)
            menuController = FindObjectOfType<MenuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (menuController.isPaused)
        {
            cameraMove.inputDir = Vector2.zero;
            return;
        }

        skateMovement.stop = Input.GetMouseButton(1);

        skateMovement.isSkating = Input.GetMouseButton(0);

        if (Input.GetMouseButtonDown(1))
        {
            skateMovement.Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            skateMovement.StartCharge();
        }

        if (Input.GetMouseButtonUp(0))
        {
            skateMovement.ReleaseCharge();
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        cameraMove.inputDir = new Vector2(mouseX,mouseY);
    }
}
