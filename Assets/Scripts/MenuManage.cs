using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Leap;
public class MenuManage : MonoBehaviour
{
    #region Variables
    [Header("-----------UI Elements-----------")]
    public GameObject hintText; // Reference to the hint text UI element

    [Header("----------Menu Screen--------")] 

    public GameObject background;
    public GameObject mainMenu;
    public GameObject openingScreen;
    public GameObject BlackBackground;
    public GameObject card;
    public GameObject cardReader;
    public GameObject TutScreen1;
    public GameObject TutScreen2;
    public HandPoseDetector detector;
    // public GameObject easterEgg; //Easter Egg for opening screen
    [Header("----------Easter Egg Trigger--------")]
    public float chanceToShow = 0.2f;
    public List<Sprite> easterEggSprites;
    [Header("-----------Others-----------")]
    public float splashDuration = 2.0f;
    public float fadeDuration = 1.0f;
    public float TutSplashDuration = 5.0f;
    [Header("----------Private--------")]
    private CanvasGroup BGCanvasGroup;
    private CanvasGroup mainMenuCanvasGroup;
    private CanvasGroup openingCanvasGroup;
    private CanvasGroup easterEggCanvasGroup;
    private CanvasGroup blackBackgroundCanvasGroup;
    private CanvasGroup TutScreen1CanvasGroup;
    private CanvasGroup TutScreen2CanvasGroup;
    public bool easterEggerIsTriggered = false;
    private BagManage BagManageScript;
    [Header("----------Flags--------")]
    public bool isOpeningScreen = true;
    private bool isTut1OpenedFlag = false;// control tut1 open once(no use, controled by opening scene, opening scene open once, tut1 screen open once)
    private bool isTut2OpenedFlag = false;// control tut2 open once
    public bool isTutScreenOpenedFlag = false;


    #endregion
    #region Behaviours Methods
    public void Start()
    {
        isTutScreenOpenedFlag = false;
        BagManageScript = GameObject.Find("BagManager").GetComponent<BagManage>();

        openingCanvasGroup = openingScreen.GetComponent<CanvasGroup>();
        mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();
        BGCanvasGroup = background.GetComponent<CanvasGroup>();
        blackBackgroundCanvasGroup = BlackBackground.GetComponent<CanvasGroup>();
        TutScreen1CanvasGroup = TutScreen1.GetComponent<CanvasGroup>();
        TutScreen2CanvasGroup = TutScreen2.GetComponent<CanvasGroup>();

        background.SetActive(false);
        openingScreen.SetActive(true);
        BlackBackground.SetActive(true);
        TutScreen1.SetActive(false);
        TutScreen2.SetActive(false);

        // if(!isOpeningScreen) StartCoroutine(ShowOpeningScreen());
        StartCoroutine(ShowOpeningScreen());
        
    }
    public void Update()
    {
        //Skip the opening screen if the user presses the Escape key
        if (Input.GetKeyDown(KeyCode.Space) && isOpeningScreen)
        {
            Debug.Log("Space key pressed. Skipping opening screen.");
            openingScreen.SetActive(false);
            BlackBackground.SetActive(false);
        }
        if(openingScreen.activeSelf & Input.GetMouseButtonDown(0) & !easterEggerIsTriggered)
        {
            HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
            if(detectedPose != null)
            {
                TriggerEasterEgg();
            }                
        }
        isEnter();

        if(!isOpeningScreen && !isTut2OpenedFlag && BagManageScript.isBagOpen)
        {
            StartCoroutine(ShowTutScreen2());
            isTut2OpenedFlag = true;
        }
        
    }
    #endregion

    #region Methods
    private IEnumerator ShowOpeningScreen()
    {   
        isOpeningScreen = true;
        // Fade in the opening screen
        yield return StartCoroutine(FadeCanvasGroup(openingCanvasGroup, 0f, 1f, splashDuration));
        yield return new WaitForSeconds(splashDuration);
        if(easterEggerIsTriggered)
        {
            yield return new WaitForSeconds(splashDuration);
        }
        // Fade out the opening screen and black background
        yield return StartCoroutine(FadeCanvasGroup(openingCanvasGroup, 1f, 0f, splashDuration));
        yield return StartCoroutine(FadeCanvasGroup(blackBackgroundCanvasGroup, 1f, 0f, splashDuration));
        openingScreen.SetActive(false);
        BlackBackground.SetActive(false);
        isOpeningScreen = false;
        // yield return StartCoroutine(ShowMainMenu());
        TutScreen1.SetActive(true);

        StartCoroutine(ShowTutScreen1());

    }

    private IEnumerator ShowTutScreen1()
    {   
        Debug.Log("ShowTutScreen1() called");
        isTutScreenOpenedFlag = true;

        // Fade in the opening screen
        yield return StartCoroutine(FadeCanvasGroup(TutScreen1CanvasGroup, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(TutSplashDuration);
        
        // Fade out the opening screen and black background
        yield return StartCoroutine(FadeCanvasGroup(TutScreen1CanvasGroup, 1f, 0f, fadeDuration));
        TutScreen1.SetActive(false);
        isTutScreenOpenedFlag = false;
    }
    
    private IEnumerator ShowTutScreen2()
    {   
        Debug.Log("ShowTutScreen2() called");
        isTutScreenOpenedFlag = true;
        TutScreen2.SetActive(true);
        // Fade in the opening screen
        yield return StartCoroutine(FadeCanvasGroup(TutScreen2CanvasGroup, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(TutSplashDuration);
        
        // Fade out the opening screen and black background
        yield return StartCoroutine(FadeCanvasGroup(TutScreen2CanvasGroup, 1f, 0f, fadeDuration));
        TutScreen2.SetActive(false);
        isTutScreenOpenedFlag = false;
    }

    private void TriggerEasterEgg()
    {
        if(easterEggSprites != null)
        {   
            int i = 0;
            List<Image> openingImages = new List<Image>(openingScreen.GetComponentsInChildren<Image>());

            foreach (Image image in openingImages)
            {
            
                image.sprite = easterEggSprites[i];
                i += 1;
            }
            easterEggerIsTriggered = true;
            Debug.Log("Easter Egg Triggered!");
            
        }
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Ensure it reaches the exact target value
    }    
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    private void isEnter()
    {
        // Check if the card and cardReader are colliding
        if (card != null && cardReader != null)
        {
            Collider cardCollider = card.GetComponent<Collider>();
            Collider cardReaderCollider = cardReader.GetComponent<Collider>();

            if (cardCollider != null && cardReaderCollider != null && cardCollider.bounds.Intersects(cardReaderCollider.bounds))
            {
                Debug.Log("Card collided with CardReader!");
                StartCoroutine(FadeCanvasGroup(blackBackgroundCanvasGroup, 0f, 1f, splashDuration));
            }
        }
    }
    #endregion
}