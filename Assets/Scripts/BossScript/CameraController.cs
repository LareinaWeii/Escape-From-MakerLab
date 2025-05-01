using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class CameraController : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    private float mouseX , mouseY;
    public float mouseSensitivity = 100f;
    public float xRotation;
    public float rotationSpeed_Y = 50f; // Speed of camera rotation
    public float rotationSpeed_X = 40f; // Speed of camera rotation


    [Header("Leap Motion Settings")]
    public LeapServiceProvider leapServiceProvider;
    // public GameObject hands;
    public GameObject leftHandWist;
    public GameObject rightHandWist;
    public Camera mainCamera;
    public float screenEdgeThreshold = 0.2f;
    public float screenEdgeThreshold_H = 0.2f;

    private float TargetRotationY = 0f; // Target Y-axis rotation
    private float CurrentRotationY = 0f; // Current Y-axis rotation

    private float TargetRotationX = 0f; // Target X-axis rotation
    private float CurrentRotationX = 0f; // Current X-axis rotation


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // mouseRotation();
        HandsRotation();
    }

    void mouseRotation()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation , -30f , 2f);

        player.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation , 0, 0);
    }

    void HandsRotation()
    {
        if (leapServiceProvider != null && leapServiceProvider.CurrentFrame.Hands.Count > 0)
        {
            if (leftHandWist != null || rightHandWist != null)
            {
                // Convert Leap Motion's Vector to Unity's Vector3
                Vector3 leftHandWorldPos = leftHandWist.transform.position;
                Vector3 leftHandScreenPos = mainCamera.WorldToScreenPoint(leftHandWorldPos);

                Vector3 rightHandWorldPos = rightHandWist.transform.position;
                Vector3 rightHandScreenPos = mainCamera.WorldToScreenPoint(rightHandWorldPos);

                // Check Yaw Rotation
                if (leftHandScreenPos.x <= Screen.width * screenEdgeThreshold)
                {
                    TargetRotationY = -rotationSpeed_Y;
                    TargetRotationX = 0f;
                }
                else if (rightHandScreenPos.x >= Screen.width * (1-screenEdgeThreshold))
                {
                    TargetRotationY = -rotationSpeed_Y;
                    TargetRotationX = 0f;
                }
                else
                {
                    TargetRotationY = 0f;
                    float current_rotation_x = transform.rotation.eulerAngles.x;

                    // Check Pitch Rotation
                    if (leftHandScreenPos.y <= Screen.height * screenEdgeThreshold_H)
                    {
                        if ((current_rotation_x > 20) && (current_rotation_x < 50)) TargetRotationX = 0f;
                        else TargetRotationX = rotationSpeed_X;
                    }
                    else if (leftHandScreenPos.y >= Screen.height * (1-screenEdgeThreshold_H))
                    {
                        if ((current_rotation_x < 340) && (current_rotation_x > 320)) TargetRotationX = 0f;
                        else TargetRotationX = -rotationSpeed_X;
                    }
                    else
                    {
                        TargetRotationX = 0f;
                    }
                }
            }
            else
            {
                TargetRotationX = 0f;
                TargetRotationY = 0f;
            }
        }
        else
        {
            TargetRotationX = 0f;
            TargetRotationY = 0f;
        }

        player.transform.Rotate(Vector3.up, CurrentRotationY * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.right, CurrentRotationX * Time.deltaTime, Space.Self);
    }
}
