using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : LivingEntilty
{
    [Header("Navigation Settings")]
    public Transform target;
    public float distanceThreshold = 5f;
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float rotationSpeed = 120f;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform firePosition;

    [Header("Collision Damage")]
    public int collisionDamage = 100;    // ������ײ�˺���
    public float damageCooldown = 1f;    // �˺���ȴʱ��
    private float lastDamageTime;

    private NavMeshAgent agent;
    private Animator animator;
    private float currentSpeed;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        ConfigureNavAgent();
    }

    void ConfigureNavAgent()
    {
        agent.speed = walkSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.acceleration = 8f;
        agent.stoppingDistance = 1f;
    }

    void Update()
    {
        if (target != null)
        {
            UpdateNavigation();
            UpdateAnimation();
        }
        CheckContinuousCollision();
    }

    void UpdateNavigation()
    {
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);
        currentSpeed = distance > distanceThreshold ? runSpeed : walkSpeed;
        agent.speed = currentSpeed;
    }

    void UpdateAnimation()
    {
        float speedNormalized = 0f;

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            speedNormalized = agent.velocity.magnitude / currentSpeed;
            // �����ƶ��ٶ����͵�����������
            speedNormalized *= (currentSpeed == runSpeed) ? 1f : 0.5f;
        }

        animator.SetFloat("speed", speedNormalized);
    }

    // ʾ�������������Ҫ�ⲿ���û���Ӵ���������
    public void Fire()
    {
        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
        }
    }

    // ��ײ���
    void CheckContinuousCollision()
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        // ʹ�����μ����浥����ײ���
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            GetComponent<CapsuleCollider>().radius * 1.2f,  // ��ⷶΧ�Դ���������
            LayerMask.GetMask("Infantry")                   // ʹ��ר�ò㼶
        );

        foreach (Collider hit in hits)
        {
            ApplyCollisionDamage(hit);
            lastDamageTime = Time.time;
            break; // ���μ��ֻ����һ���˺�
        }
    }

    void ApplyCollisionDamage(Collider infantryCollider)
    {
        if (infantryCollider.CompareTag("infantry"))
        {
            Idamageble damageable = infantryCollider.GetComponent<Idamageble>();
            if (damageable != null)
            {
                // ģ���ӵ����˺����ݷ�ʽ
                RaycastHit hitInfo = new RaycastHit();
                hitInfo.point = infantryCollider.ClosestPoint(transform.position);
                //hitInfo.collider = infantryCollider;

                damageable.TaskHit(collisionDamage, hitInfo);
                Debug.Log($"Player collision dealt {collisionDamage} damage!");
            }
        }
    }

    // ���ӻ���ⷶΧ�����ڱ༭����ʾ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            GetComponent<CapsuleCollider>().radius * 1.2f
        );
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class PlayerController : MonoBehaviour
//{
//    public float walkSpeed = 2;
//    public float runSpeed = 6;
//    public float gravity = -12;
//    public float jumpHeight = 1;
//    float velocityY;
//    public Transform infantry;
//    public GameObject LaserPrefab;
//    public Transform firePosition;
//    private Animator animator;
//    private CharacterController characterController;
//    Transform cameraT;
//    void Start()
//    {
//        characterController = GetComponent<CharacterController>();
//        animator = GetComponentInChildren<Animator>();
//        cameraT = Camera.main.transform;
//    }

//    // Update is called once per frame

//    void FixedUpdate()
//    {
//        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
//        Vector2 inputDir = input.normalized;
//        if (inputDir != Vector2.zero)
//        {
//            transform.eulerAngles = Vector3.up * (Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y);
//        }


//        bool running = Input.GetKey(KeyCode.LeftShift);
//        float speed = ((running ? runSpeed : walkSpeed)) * inputDir.magnitude;
//        velocityY += Time.fixedDeltaTime * gravity;
//        Vector3 velocity = transform.forward * speed + Vector3.up * velocityY;
//        characterController.Move(velocity * Time.fixedDeltaTime);
//        if (characterController.isGrounded)
//        {
//            velocityY = 0;
//        }
//        if (Input.GetKey(KeyCode.Space))
//        {
//            Jump();
//        }
//        if (Input.GetMouseButton(0))
//        {
//            Fire();
//        }
//        float animationSpeedPercent = ((running) ? 1 : .5f) * inputDir.magnitude;
//        animator.SetFloat("speed", animationSpeedPercent);
//    }
//    void Jump()
//    {
//        if (characterController.isGrounded)
//        {
//            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
//            velocityY = jumpVelocity;
//        }
//    }
//    void Fire()
//    {
//        Instantiate(LaserPrefab, firePosition.position, firePosition.rotation);
//    }
//}
