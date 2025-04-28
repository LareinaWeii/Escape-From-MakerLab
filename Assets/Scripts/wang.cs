using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wang : MonoBehaviour
{
    [Header("Settings")]
    public float descendSpeed = 1.0f;

    private MainSystem mainSystem;
    private bool isDescending = false;
    private float targetY;

    // Start is called before the first frame update
    void Start()
    {
        mainSystem = GameObject.Find("Game Manager").GetComponent<MainSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        targetY = transform.position.y - 5f; 
        if (Mathf.Abs(transform.position.y - targetY) < 0.01f) return;

        if (mainSystem.gamePass[0] == 1 && mainSystem.gamePass[1] == 1 && 
            mainSystem.mainScene.activeSelf && !isDescending)
        {
            isDescending = true;
        }

        // Test
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isDescending = true;
        }

        if (isDescending)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), Time.deltaTime * descendSpeed);
        }
    }
}
