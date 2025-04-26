using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoltDropChecker : MonoBehaviour
{
    [Header("Target Bolt")]
    public GameObject sampleBolt;


    [Header("Replace Bolt")]
    public GameObject realBoltToActivate;

    [Header("Hole")]
    public GameObject boltHole;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision entered with: " + collision.gameObject.name);
        if (IsCorrectBolt(collision.gameObject))
        {
            Debug.Log("Right bolt!");
            if (boltHole != null)
            {
                PlaceCorrectBolt(collision.gameObject);
            }
        }
        else
        {
            Debug.Log("Wrong bolt!");
        }
    }


    bool IsCorrectBolt(GameObject bolt)
    {
        if (sampleBolt != null) return bolt.name == sampleBolt.name;
        else return false;
    }

    void PlaceCorrectBolt(GameObject bolt)
    {
        Destroy(bolt);
        Destroy(boltHole);

        if (realBoltToActivate != null)
        {
            realBoltToActivate.SetActive(true);
        }
    }
}
