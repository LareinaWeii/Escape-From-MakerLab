using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Unity.VisualScripting;

public class CameraMove : MonoBehaviour
{
    [Header("Leap Motion Settings")]
    public LeapServiceProvider leapServiceProvider;
    public GameObject hands;
    public GameObject leftHandWist;
    public GameObject rightHandWist;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 30f; // Speed of camera rotation
    public float screenEdgeThreshold = 0.1f; // 10% of the screen width    

    [Header("References")]
    public HandPoseDetector detector;
    public Camera mainCamera;

    
    //movement
    private float currentSpeed = 0f;

    //rotation
    private float currentRotationY = 0f; // Current Y-axis rotation
    private float targetRotationY = 0f; // Target Y-axis rotation

    void Update()
    {
        HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
        if (detectedPose != null)
        {
            Debug.Log("Detected pose: " + detectedPose.name);
            currentSpeed = speed;
        }
        else
        {
            Debug.Log("No pose detected");
            currentSpeed = 0f;
        }
        

        targetRotationY = 0f; // Reset target rotation

         // Get the left hand's position in screen space
        if (leapServiceProvider != null && leapServiceProvider.CurrentFrame.Hands.Count > 0)
        {
            if (leftHandWist != null)
            {
                // Convert Leap Motion's Vector to Unity's Vector3
                Vector3 leftHandWorldPos = leftHandWist.transform.position;
                Vector3 leftHandScreenPos = mainCamera.WorldToScreenPoint(leftHandWorldPos);
                // Check if the left hand is in the leftmost 1/5 of the screen
                if (leftHandScreenPos.x <= Screen.width * screenEdgeThreshold)
                {
                    Debug.Log("Left hand is in the leftmost 1/5 of the screen");
                    targetRotationY = -rotationSpeed;
                    // // Rotate the camera to the left
                    // mainCamera.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
                }
            }
            if (rightHandWist != null)
            {
                // Convert Leap Motion's Vector to Unity's Vector3
                Vector3 rightHandWorldPos = rightHandWist.transform.position;
                Vector3 rightHandScreenPos = mainCamera.WorldToScreenPoint(rightHandWorldPos);
                Debug.Log(rightHandScreenPos.x + " " + Screen.width * screenEdgeThreshold);
                // Check if the left hand is in the leftmost 1/5 of the screen
                if (rightHandScreenPos.x >= Screen.width * (1-screenEdgeThreshold))
                {
                    Debug.Log("Right hand is in the righttmost 1/5 of the screen");
                    targetRotationY = rotationSpeed;
                    // // Rotate the camera to the left
                    // mainCamera.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
                }
            }
        }

        // Rotation
        currentRotationY = Mathf.Lerp(currentRotationY, targetRotationY, Time.deltaTime * rotationSpeed);
        mainCamera.transform.Rotate(Vector3.up, currentRotationY * Time.deltaTime, Space.World);
        leapServiceProvider.transform.Rotate(Vector3.up, currentRotationY * Time.deltaTime, Space.World);

        // Movement
        mainCamera.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.World);
        leapServiceProvider.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.World);
    }
}
