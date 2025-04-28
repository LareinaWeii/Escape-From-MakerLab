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

    private MainSystem mainSystem;
    private bool isFinished = false;

    void Start()
    {
        mainSystem = GameObject.Find("Game Manager").GetComponent<MainSystem>();
    }

    void Update()
    {
        if (isFinished)
        {
            mainSystem.gamePass[0] = 1;
            mainSystem.ReturnToMainScene();
        }    
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision entered with: " + collision.gameObject.name);
        if (IsCorrectBolt(collision.gameObject))
        {
            Debug.Log("Right bolt!");
            if (boltHole != null)
            {
                StartCoroutine(PlaceCorrectBolt(collision.gameObject));
            }
        }
        else Debug.Log("Wrong bolt!");
    }


    bool IsCorrectBolt(GameObject bolt)
    {
        if (sampleBolt != null) return bolt.name == sampleBolt.name;
        else return false;
    }

    private IEnumerator PlaceCorrectBolt(GameObject bolt)
    {
        Destroy(bolt);
        Destroy(boltHole);

        if (realBoltToActivate != null)
        {
            realBoltToActivate.SetActive(true);
        }

        yield return new WaitForSeconds(5f);
        isFinished = true;
    }
}
