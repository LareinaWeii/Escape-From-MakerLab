using UnityEngine;
using System.Collections;
using Leap;

[RequireComponent(typeof(CharacterController))]
public class InfantryController : LivingEntilty
{
    [Header("Leap Motion Settings")]
    public HandPoseDetector FistDetector;
    // public LeapServiceProvider leapServiceProvider;
    // public GameObject hands;
    // public GameObject leftHandWist;
    // public GameObject rightHandWist;

    [Header("Movement Settings")]
    public float moveSpeed = 12f;
    public float gravity = 9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float checkRadius = 0.4f;
    public LayerMask groundMask;
    public Camera mainCamera;
    public float screenEdgeThreshold = 0.1f;
    public float screenEdgeThreshold_H = 0.1f;

    [Header("Combat Settings")]
    public GameObject greenGrapePrefab;
    public Transform weaponHold;
    public float fireRate = 0.1f;
    public float bulletSpeed = 50f;

    [Header("Missile Settings")]
    public Transform missileShoot;       // 导弹挂载点
    public float missileCaptureRange = 2f; // 导弹捕获范围
    public float missileLaunchForce = 30f; // 导弹发射力度
    public LayerMask missileLayer;       // 导弹所在层级

    private GameObject currentMissile;  // 当前持有的导弹
    private bool hasRespawned;           // 复活标记
    private Coroutine respawnCoroutine;  // 复活协程

    private CharacterController controller;
    private Vector3 velocity;
    private float nextFireTime;
    private bool isGrounded;
    private float currentSpeed = 0f;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();

        // 武器挂载点安全检测
        if (weaponHold == null)
        {
            Debug.LogError("WeaponHold reference missing in InfantryController!");
        }
    }


    void Update()
    {
        HandleGroundCheck();
        // HandleMovement();
        MovementByHand(); // 使用手势控制移动
        HandleJump();
        HandleShooting();
        ApplyGravity();
        HandleMissileControl();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            checkRadius,
            groundMask
        );
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
        float vertical = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;
        controller.Move(moveDir * Time.deltaTime);
    }

    void MovementByHand()
    {
        HandPoseScriptableObject detectedPose = FistDetector.GetCurrentlyDetectedPose();
        if (detectedPose != null)
        {
            // Debug.Log("Detected pose: " + detectedPose.name);
            currentSpeed = moveSpeed;
        }
        else
        {
            // Debug.Log("No pose detected");
            currentSpeed = 0f;
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireBullet()
    {
        if (greenGrapePrefab && weaponHold)
        {
            // 创建子弹实例
            GameObject bullet = Instantiate(
                greenGrapePrefab,
                weaponHold.position,
                weaponHold.rotation
            );

            // 设置子弹参数
            Greengrape grape = bullet.GetComponent<Greengrape>();
            if (grape != null)
            {
                grape.SetSpeed(bulletSpeed); // 设置子弹移动速度
            }
            else
            {
                Debug.LogWarning("Greengrape component missing on bullet prefab!");
            }

            // 移除可能的物理组件（根据子弹脚本需求）
            RemovePhysicsComponents(bullet);
        }
    }

    void RemovePhysicsComponents(GameObject bullet)
    {
        // 移除可能冲突的刚体组件
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        // 移除可能冲突的碰撞体（如果子弹脚本自行处理碰撞）
        Collider col = bullet.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMissileControl()
    {
        // 自动捕获附近的导弹
        if (currentMissile == null)
        {
            Debug.Log("Missile not found, searching...");
            Collider[] missiles = Physics.OverlapSphere(
                transform.position,
                missileCaptureRange,
                missileLayer
            );
            Debug.Log("Missile found");

            foreach (Collider missile in missiles)
            {
                Debug.Log("Missile detected");
                Guidedmissile missileScript = missile.GetComponent<Guidedmissile>();
                if (missile.CompareTag("GuidedMissile") && missileScript != null && !missileScript.IsLaunched)
                {
                    AttachMissile(missile.gameObject);
                    break;
                }
            }
        }

        // 右键发射导弹
        if (Input.GetMouseButtonDown(1) && currentMissile != null)
        {
            LaunchMissile();
        }
    }

    void AttachMissile(GameObject missile)
    {
        currentMissile = missile;

        // 禁用导弹物理和碰撞
        Rigidbody rb = currentMissile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        currentMissile.GetComponent<Collider>().enabled = false;

        // 定位到挂载点
        currentMissile.transform.SetParent(missileShoot);
        currentMissile.transform.localPosition = Vector3.zero;
        currentMissile.transform.localRotation = Quaternion.identity;
    }

    void LaunchMissile()
    {
        if (currentMissile == null) return;

        // 解除父子关系
        currentMissile.transform.SetParent(null);

        // 获取并配置刚体
        Rigidbody rb = currentMissile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 防止穿透
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.AddForce(missileShoot.forward * missileLaunchForce, ForceMode.Impulse);
        }

        // 启用碰撞体
        Collider col = currentMissile.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // 标记导弹已发射
        Guidedmissile missileScript = currentMissile.GetComponent<Guidedmissile>();
        if (missileScript != null)
        {
            missileScript.IsLaunched = true;  // 需要导弹脚本支持此属性
            missileScript.Activate();
        }

        currentMissile = null;
    }

    IEnumerator DisableMissileAfterTime(Guidedmissile missile, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (missile != null)
        {
            Destroy(missile.gameObject);
        }
    }

    // 可视化捕获范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, missileCaptureRange);
    }
}




//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class infantryController : LivingEntilty
//{
//    private CharacterController controller;
//    public float speed = 12f;
//    private float horizontalMove, verticalMove;
//    private Vector3 moveDirection;
//    private float gravity = 9.81f;
//    private Vector3 velocity;

//    public Transform groundCheck;
//    public float checkRadius;
//    public LayerMask groundMask;
//    public bool isGrounded;

//    // Start is called before the first frame update
//    protected override void Start()
//    {
//        base.Start();
//        controller = GetComponent<CharacterController>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundMask);
//        if (isGrounded && velocity.y < 0)
//        {
//            velocity.y = -2f;
//        }

//        horizontalMove = Input.GetAxis("Horizontal") * speed;
//        verticalMove = Input.GetAxis("Vertical") * speed;
//        moveDirection = transform.right * horizontalMove + transform.forward * verticalMove;
//        controller.Move(moveDirection * Time.deltaTime);

//        velocity.y -= gravity * Time.deltaTime;
//        controller.Move(velocity * Time.deltaTime);
//    }
//}
