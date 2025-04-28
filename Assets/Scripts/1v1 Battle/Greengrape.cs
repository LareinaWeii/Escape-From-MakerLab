using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greengrape : MonoBehaviour
{
    float speed;
    float moveDistance;
    public LayerMask collisionMask;
    private float damage = 1;//×Óµ¯ÉËº¦

    private void Start()
    {
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        moveDistance = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDistance);
        CheckCollision(moveDistance);
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, moveDistance , collisionMask , QueryTriggerInteraction.Collide ))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        Idamageble idamageble = hit.collider.GetComponent<Idamageble>();
        if(idamageble != null)
        {
            idamageble.TaskHit(damage , hit);
        }
        Destroy(gameObject);
    }
}
