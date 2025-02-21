using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    public Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (DialogSystem.Instance.dialogUIActive == false && EscMenuManager.Instance.isActive == false)
        {
            LookAround();
        }
    }

    public void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (PlayerController.Instance.IsHanging)
        {
            yRotation += mouseX;
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        else
        {
            // Normal durumda kamera ve playerBody birlikte döner
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
