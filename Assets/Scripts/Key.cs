using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private MainSystem mainSystem;

    void Start()
    {
        mainSystem = GameObject.Find("Game Manager").GetComponent<MainSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Key Collision Detected");
        if (transform.parent.parent.name == "gold key")
        {
            if (mainSystem.gameState == 0)  mainSystem.gameState = 1;
            Debug.Log("Gold Key");
        }
        else if (transform.parent.parent.name == "silver key")
        {
            if (mainSystem.gameState == 0)  mainSystem.gameState = 1;
            Debug.Log("Silver Key");
        }
        else if (transform.parent.parent.name == "regular key")
        {
            if (mainSystem.gameState == 0)  mainSystem.gameState = 2;
            Debug.Log("Regular Key");
        }
    }
}
