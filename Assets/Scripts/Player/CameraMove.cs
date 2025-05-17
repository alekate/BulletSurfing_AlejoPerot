using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Vector2 turn;
    public float sensX = 5f;
    public float sensY = 5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        turn.x += Input.GetAxis("Mouse X") * sensX;
        turn.y += Input.GetAxis("Mouse Y") * sensY;

        turn.y = Mathf.Clamp(turn.y, -60f, 70f);
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}

