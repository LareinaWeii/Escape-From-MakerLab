using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]

public class Enemy : LivingEntilty
{
    NavMeshAgent agent;
    public Transform target;
    GunController gunController;
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(UpdatePath(0.1f , new Vector3()));
        gunController = GetComponent<GunController>();
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))//鼠标左键按下
        //{
        //    gunController.Shoot();
        //}
        StartCoroutine(AutoFireRoutine());
    }

    IEnumerator AutoFireRoutine()
    {
        while (true)
        {
            gunController.Shoot();
            yield return new WaitForSeconds(1f); // 1秒间隔
        }
    }

    IEnumerator UpdatePath(float refreshRate , Vector3 targetPosition)
    {
        while(targetPosition != null)
        {
            if (target == null) yield break;
            targetPosition = new Vector3(target.position.x, 0, target.position.z);
            if(!dead)
                agent.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
