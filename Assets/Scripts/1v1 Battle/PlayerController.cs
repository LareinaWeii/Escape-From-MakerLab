using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    Rigidbody rig;
    Vector3 velocity;

    [Header("Aim Assist Settings")]
    [SerializeField] float snapAngle = 60f;    // ������Ч�ĽǶ���ֵ
    [SerializeField] float maxSnapDistance = 12f; // �����������
    [SerializeField] float snapStrength = 1.2f;    // ����ǿ�ȣ�0-1��

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
            // ��ȡˮƽ�淽��������ǿ��Y=0��
            Vector3 rawDirection = FlattenVector(point - transform.position).normalized;
            Vector3 enemyDirection = FlattenVector(enemy.position - transform.position).normalized;

            // ����������ķ���
            Vector3 correctedDirection = CalculateCorrectedDirection(rawDirection, enemyDirection);

            // ����ԭʼ��׼����
            float aimDistance = Vector3.Distance(transform.position, point);
            finalPoint = transform.position + correctedDirection * aimDistance;
        }

        // ����ˮƽ��ת
        finalPoint.y = transform.position.y;
        transform.LookAt(finalPoint);

        // ���Կ��ӻ�
        Debug.DrawLine(transform.position, point, Color.green);        // ԭʼ����
        Debug.DrawLine(transform.position, finalPoint, Color.yellow);  // ��������
        Debug.DrawLine(transform.position, enemy.position, Color.red);  // ����λ��
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

        // ��������˵�ˮƽ����
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

        // ������Ч��Χʱ����ԭʼ����
        if (angle > snapAngle || distance > maxSnapDistance)
            return rawDir;

        // ��̬����ǿ�ȼ���
        float angleFactor = 1 - Mathf.Pow(angle / snapAngle, 0.3f);
        float distanceFactor = Mathf.Pow(1 - (distance / maxSnapDistance), 1.5f);
        float strength = snapStrength * angleFactor * distanceFactor;

        // ���������㷨������˷���������
        return Vector3.RotateTowards(
            rawDir,
            enemyDir,
            strength * Mathf.Deg2Rad * angle, // ���ݽǶȲ̬��������
            0
        ).normalized;
    }

}
