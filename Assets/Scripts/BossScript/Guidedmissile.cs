using UnityEngine;

public class Guidedmissile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float initialSpeed = 60f;        // 改为正值
    public float acceleration = 40f;        // 改为正值
    public float steeringSpeed = 5f;
    public float maxSpeed = 120f;           // 改为正值
    public float detectionDistance = 2f;
    public LayerMask collisionMask;
    public float damage = 990;
    public float lifetime = 5f;

    [Header("Physics Settings")]
    public float gravityMultiplier = 0.1f;
    public float drag = 0.05f;

    private Rigidbody rb;
    private bool isActive;
    private Vector3 currentDirection;
    private Transform target; // 新增目标变量
    private float activeTime;

    public bool IsLaunched { get; set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.maxAngularVelocity = 0;
        //Destroy(gameObject, lifetime);
        // 获取目标（示例：假设敌人标签为"Enemy"）
        target = GameObject.FindGameObjectWithTag("Enemy")?.transform;
    }

    public void Activate()
    {
        IsLaunched = true;
        isActive = true;
        currentDirection = transform.forward;

        if (rb != null)
        {
            rb.useGravity = true;
            rb.velocity = currentDirection * initialSpeed; // 初始速度方向正确
        }
    }

    void FixedUpdate()
    {
        if (isActive && rb != null)
        {
            // 持续加速
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(currentDirection * acceleration, ForceMode.Acceleration);
            }

            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

            // 动态调整方向：朝向目标
            if (target != null)
            {
                Vector3 targetDirection = (target.position - transform.position).normalized;
                currentDirection = Vector3.RotateTowards(currentDirection, targetDirection,
                    steeringSpeed * Time.fixedDeltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(currentDirection);
            }
            else
            {
                // 无目标时保持原逻辑
                if (rb.velocity != Vector3.zero)
                {
                    currentDirection = Vector3.Lerp(currentDirection, rb.velocity.normalized,
                        steeringSpeed * Time.fixedDeltaTime);
                    transform.rotation = Quaternion.LookRotation(currentDirection);
                }
            }
        }
    }

    void Update()
    {
        if (isActive)
        {
            activeTime += Time.deltaTime;
            CheckCollision();
        }
    }

    void CheckCollision()
    {
        // 使用SphereCast提高检测精度
        RaycastHit hit;
        float checkDistance = Mathf.Clamp(rb.velocity.magnitude * Time.deltaTime, 0.5f, 2f);

        if (Physics.SphereCast(transform.position, 0.5f, currentDirection, out hit,
            checkDistance, collisionMask))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        Idamageble target = hit.collider.GetComponent<Idamageble>();
        if (target != null)
        {
            target.TaskHit(damage, hit);
        }
        Debug.Log("Hit: " + hit.collider.name);
        DestroyMissile();
    }

    void DestroyMissile()
    {
        Destroy(gameObject);
    }

    // 调试显示碰撞检测范围
    void OnDrawGizmosSelected()
    {
        if (isActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + currentDirection * detectionDistance);
        }
    }
}