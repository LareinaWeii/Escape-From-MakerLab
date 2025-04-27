using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    public Transform bolts;


    private class BoltInitialState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private Dictionary<Transform, BoltInitialState> boltInitialStates = new Dictionary<Transform, BoltInitialState>();

    void Start()
    {
        if (bolts == null)
        {
            Debug.LogError("No bolts!");
            return;
        }

        // 收集所有子物体的初始状态
        foreach (Transform bolt in bolts)
        {
            BoltInitialState state = new BoltInitialState
            {
                position = bolt.position,
                rotation = bolt.rotation,
                scale = bolt.localScale
            };
            boltInitialStates.Add(bolt, state);
        }
    }

    public void Restart()
    {
        // string currentSceneName = SceneManager.GetActiveScene().name;
        // SceneManager.LoadScene(currentSceneName);

        foreach (var pair in boltInitialStates)
        {
            Transform bolt = pair.Key;
            BoltInitialState state = pair.Value;

            bolt.position = state.position;
            bolt.rotation = state.rotation;
            bolt.localScale = state.scale;
        }
    }
}
