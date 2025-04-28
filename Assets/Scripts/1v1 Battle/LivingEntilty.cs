using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntilty : MonoBehaviour , Idamageble
{
    protected float health = 100;
    public float startingHealth = 100;
    protected bool dead = false;
    public void TaskHit(float damage, RaycastHit hit)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}