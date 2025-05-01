using System.Collections;

using System.Collections.Generic;

using UnityEngine;
public class ThirdPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    public Transform target;
    public float offset;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public bool lockCursor;

    float yaw = 0;

    float pitch = 0;

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        Vector3 targeteRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = targeteRotation;
        transform.position = target.position - transform.forward * offset;
    }
}



