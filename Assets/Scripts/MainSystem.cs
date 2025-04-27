using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour
{
    [Header("Game Scene")]
    public GameObject mainScene;
    public GameObject BoltGame;
    public GameObject FlipColorCubeGame;
    

    [Header("HighLighter")]
    public BoltBoxHighlighter boltBoxHighlighter;
    public ColorfulCabiantHighlighter colorfulCabiantHighlighter;

    enum SceneID
    {
        MainScene = 0,
        BoltGameScene = 1,
        FlipColorScene = 2
    }

    private SceneID activeSceneID = 0;


    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        if (boltBoxHighlighter == null) return;

        if (DetectClick())
        {
            if (boltBoxHighlighter.IsCurrentlyHighlighted())
            {
                Debug.Log("Click detected on BolyGame!");
                activeSceneID = SceneID.BoltGameScene;
                LoadMiniGameScene(BoltGame);
            }
            else if (colorfulCabiantHighlighter.IsCurrentlyHighlighted())
            {
                Debug.Log("Click detected on FlipColorCubeGame!");
                activeSceneID = SceneID.FlipColorScene;
                LoadMiniGameScene(FlipColorCubeGame);
            }
            else
            {
                Debug.Log("Click detected, but no game highlighted!");
            }
        }
        
    }

    bool DetectClick()
    {
        // TODO
        return Input.GetKeyDown(KeyCode.Space);
    }

    void LoadMiniGameScene(GameObject miniGame)
    {
        mainScene.SetActive(false);
        miniGame.SetActive(true);
    }

    public void ReturnToMainScene()
    {
        switch (activeSceneID)
        {
            case SceneID.MainScene:
                Debug.Log("Main Scene Now!");
                break;
            case SceneID.BoltGameScene:
                Debug.Log("Go to Bolt Game Scene!");
                BoltGame.SetActive(false);
                break;
            case SceneID.FlipColorScene:
                Debug.Log("Go to Flip Color Game Scene!");
                FlipColorCubeGame.SetActive(false);
                break;
            default:
                Debug.Log("Unknown Scene ID!");
                break;
        }

        mainScene.SetActive(true);
        activeSceneID = 0;
    }

}
