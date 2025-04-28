using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    Rigidbody rig;
    Vector3 velocity;

    [Header("Aim Assist Settings")]
    [SerializeField] float snapAngle = 60f;    // 吸附生效的角度阈值
    [SerializeField] float maxSnapDistance = 12f; // 最大吸附距离
    [SerializeField] float snapStrength = 1.2f;    // 吸附强度（0-1）

    [Header("Target Reference")]
    [SerializeField] public Transform enemy;

    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }



    private void FixedUpdate()
    {
        // rig.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        Vector3 move = velocity * Time.fixedDeltaTime;
        move.y = 0f;
        rig.MovePosition(new Vector3(transform.position.x + move.x, transform.position.y, transform.position.z + move.z));
        
    }

    public void Move(Vector3 moveVelocity)
    {
        velocity = moveVelocity;
    }


    public void LookAt(Vector3 point)
    {
        if (enemy == null) return;

        Vector3 finalPoint = point;

        if (ShouldApplyAimAssist())
        {
            // 获取水平面方向向量（强制Y=0）
            Vector3 rawDirection = FlattenVector(point - transform.position).normalized;
            Vector3 enemyDirection = FlattenVector(enemy.position - transform.position).normalized;

            // 计算修正后的方向
            Vector3 correctedDirection = CalculateCorrectedDirection(rawDirection, enemyDirection);

            // 保持原始瞄准距离
            float aimDistance = Vector3.Distance(transform.position, point);
            finalPoint = transform.position + correctedDirection * aimDistance;
        }

        // 保持水平旋转
        finalPoint.y = transform.position.y;
        transform.LookAt(finalPoint);

        // 调试可视化
        Debug.DrawLine(transform.position, point, Color.green);        // 原始输入
        Debug.DrawLine(transform.position, finalPoint, Color.yellow);  // 修正后方向
        Debug.DrawLine(transform.position, enemy.position, Color.red);  // 敌人位置
    }

    //public void LookAt(Vector3 point)
    //{
    //    // Vector3 vector3 = new Vector3(point.x, transform.position.y, point.z);
    //    // transform.LookAt(vector3);
    //    Vector3 finalPoint = ShouldApplyAimAssist() ?
    //                CalculateAssistedPoint(point) :
    //                point;

    //    Vector3 lookTarget = new Vector3(finalPoint.x, transform.position.y, finalPoint.z);
    //    transform.LookAt(lookTarget);
    //}
    bool ShouldApplyAimAssist()
    {
        if (enemy == null) return false;

        // 计算与敌人的水平距离
        Vector3 toEnemy = enemy.position - transform.position;
        toEnemy.y = 0;
        return toEnemy.magnitude <= maxSnapDistance;
    }

    Vector3 FlattenVector(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }

    Vector3 CalculateCorrectedDirection(Vector3 rawDir, Vector3 enemyDir)
    {
        float angle = Vector3.Angle(rawDir, enemyDir);
        float distance = Vector3.Distance(transform.position, enemy.position);

        // 超出有效范围时返回原始方向
        if (angle > snapAngle || distance > maxSnapDistance)
            return rawDir;

        // 动态吸附强度计算
        float angleFactor = 1 - Mathf.Pow(angle / snapAngle, 0.3f);
        float distanceFactor = Mathf.Pow(1 - (distance / maxSnapDistance), 1.5f);
        float strength = snapStrength * angleFactor * distanceFactor;

        // 方向修正算法（向敌人方向弯曲）
        return Vector3.RotateTowards(
            rawDir,
            enemyDir,
            strength * Mathf.Deg2Rad * angle, // 根据角度差动态调整弧度
            0
        ).normalized;
    }

}
