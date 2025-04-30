using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

[RequireComponent(typeof(PlayerController))]

public class NewBehaviourScript : LivingEntilty
{
    [Header("Leapmotion Settings")]
    public GameObject leftHandWist;
    public GameObject rightHandWist;
    public HandPoseDetector detector;

    [Header("Player Behaviour Settings")]
    public float moveSpeed = 5f;
    private Vector3 inputMove;
    private Vector3 moveVelocity;
    PlayerController playerController;
    private float initialY;

    [Header("Mini Game Manager")]
    private MainSystem mainSystem;
    public int ControllerType = 0; // 0: Keyboard, 1: Leap Motion
    public GameObject enemy;
    public float movementDeadZone = 0.01f;
    public float movementSmoothness = 5f;
    GunController gunController;
    Plane plane;
    private Vector3 lastLeftHandPosition;
    private bool isFinished = false;

    protected override void Start()
    {
        base.Start();
        mainSystem = GameObject.Find("Game Manager").GetComponent<MainSystem>();

        playerController = GetComponent<PlayerController>();
        plane = new Plane(Vector3.up, Vector3.zero);
        gunController = GetComponent<GunController>();

        initialY = transform.position.y;
        lastLeftHandPosition = leftHandWist.transform.position;
    }

    void Update()
    {
        if (ControllerType == 0) KeyboardControl();
        else HandsControl();

        StartCoroutine(CheckFinish());

        if (isFinished || Input.GetKeyDown(KeyCode.Alpha2))
        {
            mainSystem.gamePass[1] = 1;
            mainSystem.ReturnToMainScene();
        }
    }

    void HandsControl()
    {
        if (leftHandWist != null)   HandleMovement();
        aiming();

        if (rightHandWist != null) 
        {   HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
            if (detectedPose != null)
            {
                Debug.Log("Detected pose: " + detectedPose.name);
                gunController.Shoot();
            }
        }
    }

    void KeyboardControl()
    {
        // Player movement
        inputMove = new Vector3(Input.GetAxis("Horizontal") , 0 , Input.GetAxis("Vertical"));
        moveVelocity = inputMove.normalized * moveSpeed;
        playerController.Move(moveVelocity);

        // Player rotation
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发射一条射线
        float f;
        if (playerController.enemy == null) return;
        
        if (plane.Raycast(ray, out f))//射线与平面的交点
        {
            Debug.DrawLine(ray.origin, ray.GetPoint(f), Color.red);
            playerController.LookAt(ray.GetPoint(f));   //让玩家朝向射线与平面的交点
        }

        // Player shooting
        if (Input.GetMouseButton(0))//鼠标左键按下
        {
            gunController.Shoot();
        }
    }

    void HandleMovement()
    {
        Vector3 currentLeftHandPos = leftHandWist.transform.position;
        Vector3 moveDelta = currentLeftHandPos - lastLeftHandPosition;
        moveDelta.y = 0f; 

        if (moveDelta.magnitude > movementDeadZone)
        {
            // 左手位置控制移动
            Vector3 targetPosition = new Vector3(
                leftHandWist.transform.position.x,
                initialY, // 固定y轴
                leftHandWist.transform.position.z
            );

            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f; // 只允许XZ方向移动

            moveVelocity = direction.normalized * moveSpeed;
            // moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, Time.deltaTime * movementSmoothness);
        }
        else
        {
            moveVelocity = Vector3.zero; // 停止移动
        }
        playerController.Move(moveVelocity);
    }

    void HandleRotation()
    {
        if (playerController.enemy == null) return;

        Vector3 directionToRightHand = new Vector3(
            rightHandWist.transform.position.x - transform.position.x,
            0f,
            rightHandWist.transform.position.z - transform.position.z
        );

        Vector3 lookTarget = transform.position + directionToRightHand;
        playerController.LookAt(lookTarget);
    }

    void aiming()
    {
        if (playerController.enemy == null) return;

        Vector3 directionToRightHand = new Vector3(
            enemy.transform.position.x - transform.position.x,
            0f,
            enemy.transform.position.z - transform.position.z
        );

        Vector3 lookTarget = transform.position + directionToRightHand;
        playerController.LookAt(lookTarget);
    }

    private IEnumerator CheckFinish()
    {
        if (enemy != null) yield break;
        else
        {
            yield return new WaitForSeconds(4f);
            isFinished = true; // Set the flag to true
        }
    }

}
