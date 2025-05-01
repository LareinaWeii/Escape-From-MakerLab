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
    public float rotationSpeed_Y = 30f; // Speed of camera rotation
    public float rotationSpeed_X = 20f; // Speed of camera rotation
    public float screenEdgeThreshold = 0.1f; // 10% of the screen width   
    public float screenEdgeThreshold_H = 0.1f; // 10% of the screen height 

    [Header("References")]
    public HandPoseDetector FistDetector;
    public Camera mainCamera;
    public GameObject player;
    public GameObject playerCam; // The camera that follows the player

    // private BagManage bagManage;
    
    //movement
    private float currentSpeed = 0f;

    //rotation
    private float TargetRotationY = 0f; // Target Y-axis rotation
    private float CurrentRotationY = 0f; // Current Y-axis rotation

    private float TargetRotationX = 0f; // Target X-axis rotation
    private float CurrentRotationX = 0f; // Current X-axis rotation

    private MainSystem mainSystem;
    

    private RotationType camRotationDir = RotationType.NoRotation; // Type of rotation for the camera
    enum RotationType
    {
        NoRotation = 0,
        Left,
        Right
    }

    void Start()
    {
        mainSystem = GameObject.Find("Game Manager").GetComponent<MainSystem>();
        // bagManage = GameObject.Find("BagManager").GetComponent<BagManage>();
    }

    void Update()
    {
        HandPoseScriptableObject detectedPose = FistDetector.GetCurrentlyDetectedPose();
        if (detectedPose != null)
        {
            // Debug.Log("Detected pose: " + detectedPose.name);
            currentSpeed = speed;
        }
        else
        {
            // Debug.Log("No pose detected");
            currentSpeed = 0f;
        }
        

         // Get the left hand's position in screen space
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
                    // Debug.Log("Left hand is in the leftmost 1/5 of the screen");
                    TargetRotationY = -rotationSpeed_Y;
                    TargetRotationX = 0f;
                    // // Rotate the camera to the left
                    // mainCamera.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
                }
                else if (rightHandScreenPos.x >= Screen.width * (1-screenEdgeThreshold))
                {
                    // Debug.Log("Right hand is in the righttmost 1/5 of the screen");
                    TargetRotationY = rotationSpeed_Y;
                    TargetRotationX = 0f;
                    // // Rotate the camera to the left
                    // mainCamera.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
                }
                else
                {
                    TargetRotationY = 0f;
                    float current_rotation_x = playerCam.transform.rotation.eulerAngles.x;
                    // Debug.Log("Current X Rotation: " + current_rotation_x);

                    // Check Pitch Rotation
                    if (leftHandScreenPos.y <= Screen.height * screenEdgeThreshold_H)
                    {
                        // Debug.Log("Hand is in the bottom 1/10 of the screen");
                        if ((current_rotation_x > 20) && (current_rotation_x < 50)) TargetRotationX = 0f;
                        else TargetRotationX = rotationSpeed_X;
                    }
                    else if (leftHandScreenPos.y >= Screen.height * (1-screenEdgeThreshold_H))
                    {
                        // Debug.Log("Hand is in the top 1/10 of the screen");
                        if ((current_rotation_x < 330) && (current_rotation_x > 310)) TargetRotationX = 0f;
                        else TargetRotationX = -rotationSpeed_X;
                    }
                    else
                    {
                        // Debug.Log("No X Rotation");
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

        if(mainSystem.gameState == 0)
        {
            // Rotation
            CurrentRotationY = Mathf.Lerp(CurrentRotationY, TargetRotationY, Time.deltaTime * rotationSpeed_Y);
            CurrentRotationX = Mathf.Lerp(CurrentRotationX, TargetRotationX, Time.deltaTime * rotationSpeed_X);
            player.transform.Rotate(Vector3.up, CurrentRotationY * Time.deltaTime, Space.Self);
            playerCam.transform.Rotate(Vector3.right, CurrentRotationX * Time.deltaTime, Space.Self);
            // leapServiceProvider.transform.Rotate(Vector3.up, CamCurrentRotationY * Time.deltaTime, Space.World);

            // Movement
            player.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
            // leapServiceProvider.transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
        }
    }
}
