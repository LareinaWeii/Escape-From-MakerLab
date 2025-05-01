using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour
{
    public GameObject player;
    public GameObject mainCam;

    [Header("Leap Motion")]
    public LeapServiceProvider leapServiceProvider;
    public HandPoseDetector PointDetector;

    [Header("Game Scene")]
    public GameObject mainScene;
    public GameObject BoltGame;
    public GameObject Sentry1v1Game;
    public GameObject FlipColorCubeGame;
    public GameObject[] miniGamesLeapPos;
    public List<int> gamePass = new List<int>();

    [Header("Game State")]
    public GameObject FinalChoice;
    public int gameState = 0; // 0: In process, 1: Retry, 2: Over
    

    [Header("HighLighter")]
    public BoltBoxHighlighter boltBoxHighlighter;
    public ColorfulCabiantHighlighter colorfulCabiantHighlighter;
    public SentryHighlighter sentryHighlighter;
    public DoorHighlighter doorHighlighter;

    enum SceneID
    {
        MainScene = 0,
        BoltGameScene = 1,
        SentryGameScene = 2,
        FlipColorScene = 3
    }

    private SceneID activeSceneID = 0;
    private Transform mainCameraTransform = null;
    private Transform leapMotionTransform = null;
    private Vector3 camera_to_leapmotion_offset = new Vector3(0, 0, 0);
    private Vector3 initialLeapScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 lastPlayerPosition = new Vector3(0, 0, 0);
    private Vector3 lastPlayerRotation = new Vector3(0, 0, 0);


    // Start is called before the first frame update
    void Start()
    {
        gamePass = new List<int> { 0, 0, 0, 0 };
        initialLeapScale = leapServiceProvider.transform.localScale;
        lastPlayerPosition = player.transform.position;
        lastPlayerRotation = player.transform.rotation.eulerAngles;

        Transform[] childTransforms = player.GetComponentsInChildren<Transform>();
        foreach (Transform child in childTransforms)
        {
            if (child.name == "Main Camera") mainCameraTransform = child;
            else if (child.name == "Leap Motion") leapMotionTransform = child;
        }
        if (mainCameraTransform != null && leapMotionTransform != null)
        {
            camera_to_leapmotion_offset = mainCameraTransform.position - leapMotionTransform.position;
            Debug.Log("camera_to_leapmotion_offset: " + camera_to_leapmotion_offset);
        }
        else Debug.LogError("Main Camera or Leap Motion not found in player's children!");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DetectClick())
        {
            if (boltBoxHighlighter.IsCurrentlyHighlighted() && gamePass[0] == 0)
            {
                Debug.Log("Click detected on BolyGame!");
                activeSceneID = SceneID.BoltGameScene;
                gamePass[0] = 2;
                LoadMiniGameScene(BoltGame, SceneID.BoltGameScene);
            }
            else if (sentryHighlighter.IsCurrentlyHighlighted() && gamePass[1] == 0)
            {
                Debug.Log("Click detected on Sentry1v1Game!");
                activeSceneID = SceneID.SentryGameScene;
                gamePass[1] = 2;
                LoadMiniGameScene(Sentry1v1Game, SceneID.SentryGameScene);
            }
            else if (colorfulCabiantHighlighter.IsCurrentlyHighlighted() && gamePass[2] == 0)
            {
                Debug.Log("Click detected on FlipColorCubeGame!");
                activeSceneID = SceneID.FlipColorScene;
                gamePass[2] = 2;
                LoadMiniGameScene(FlipColorCubeGame, SceneID.FlipColorScene);
            }
        }

        bool passAllGames = CheckAllPass();
        Debug.Log("passAllGames: " + passAllGames);
        if (passAllGames)
        {
            Debug.Log("All games passed!");
            if ((DetectClick() && doorHighlighter.IsCurrentlyHighlighted()) || Input.GetKeyDown(KeyCode.E))
            {
                FinalChoice.SetActive(true);
            }
        }
    }

    bool DetectClick()
    {
        HandPoseScriptableObject detectedPose = PointDetector.GetCurrentlyDetectedPose();
        if (detectedPose != null)
        {
            // Debug.Log("Detected pose: " + detectedPose.name + " Enter MiniGame!");
            return true;
        }
        return false;
    }

    void LoadMiniGameScene(GameObject miniGame, SceneID sceneID)
    {
        lastPlayerPosition = player.transform.position;
        lastPlayerRotation = player.transform.rotation.eulerAngles;
        mainCam.SetActive(false);
        miniGame.SetActive(true);
        mainScene.SetActive(false);
        RePosLeapmotion(sceneID);
    }

    public void ReturnToMainScene()
    {
        switch (activeSceneID)
        {
            case SceneID.MainScene:
                Debug.Log("Main Scene Now!");
                break;
            case SceneID.BoltGameScene:
                Debug.Log("Back from Bolt Game Scene!");
                BoltGame.SetActive(false);
                break;
            case SceneID.SentryGameScene:
                Debug.Log("Back from Sentry Game Scene!");
                Sentry1v1Game.SetActive(false);
                break;
            case SceneID.FlipColorScene:
                Debug.Log("Back from Flip Color Game Scene!");
                FlipColorCubeGame.SetActive(false);
                break;
            default:
                Debug.Log("Unknown Scene ID!");
                break;
        }

        mainScene.SetActive(true);
        mainCam.SetActive(true);
        leapServiceProvider.transform.localScale = initialLeapScale;
        if (!player.activeSelf) player.SetActive(true);
        RePosLeapmotion(SceneID.MainScene);
        activeSceneID = 0;
    }

    void RePosLeapmotion(SceneID sceneID)
    {
        switch (sceneID)
        {
            case SceneID.MainScene:
                player.transform.position = lastPlayerPosition;
                player.transform.rotation = Quaternion.Euler(lastPlayerRotation);
                break;
            case SceneID.BoltGameScene:
                player.SetActive(false);
                break;
            case SceneID.SentryGameScene:
                Vector3 newPosition = miniGamesLeapPos[1].transform.position + camera_to_leapmotion_offset;
                player.transform.position = newPosition;
                player.transform.rotation = miniGamesLeapPos[1].transform.rotation;
                ScaleLeapServiceProvider(1.8f);
                break;
            case SceneID.FlipColorScene:
                Vector3 newPosition2 = miniGamesLeapPos[0].transform.position + camera_to_leapmotion_offset;
                player.transform.position = newPosition2;
                player.transform.rotation = miniGamesLeapPos[0].transform.rotation;
                ScaleLeapServiceProvider(4f);
                break;
            default:
                Debug.Log("Unknown Scene ID!");
                break;
        }
    }

    private void ScaleLeapServiceProvider(float scaleMultiplier)
    {
        if (leapServiceProvider != null)
        {
            leapServiceProvider.transform.localScale = initialLeapScale * scaleMultiplier;
            Debug.Log("LeapServiceProvider scaled to: " + leapServiceProvider.transform.localScale);
        }
        else Debug.LogError("LeapServiceProvider is not assigned!");
    }

    private bool CheckAllPass()
    {
        Debug.Log("CheckAllPass: gamePass[0]: " + gamePass[0] + " gamePass[1]: " + gamePass[1] + " gamePass[2]: " + gamePass[2]);
        if (gamePass[0] == 1 && gamePass[1] == 1 && gamePass[2] == 1)
        {
            return true;
        }
        Debug.Log("Passed Not all!");
        return false;
    }

}
