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
    public RectTransform optionArea;
    public GameObject background;
    public GameObject mainMenu;
    public GameObject optionMenu;
    public GameObject openingScreen;
    public GameObject BlackBackground;
    public GameObject card;
    public GameObject cardReader;
    public HandPoseDetector detector;
    // public GameObject easterEgg; //Easter Egg for opening screen
    [Header("----------Easter Egg Trigger--------")]
    public float chanceToShow = 0.2f;
    public List<Sprite> easterEggSprites;
    [Header("-----------Others-----------")]
    public float splashDuration = 2.0f;
    [Header("----------Private--------")]
    private CanvasGroup BGCanvasGroup;
    private CanvasGroup mainMenuCanvasGroup;
    private CanvasGroup openingCanvasGroup;
    private CanvasGroup easterEggCanvasGroup;
    private CanvasGroup blackBackgroundCanvasGroup;
    public bool easterEggerIsTriggered = false;
    private bool isOpeningScreen = false;
    #endregion
    #region Behaviours Methods
    public void Start()
    {
        openingCanvasGroup = openingScreen.GetComponent<CanvasGroup>();
        mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();
        BGCanvasGroup = background.GetComponent<CanvasGroup>();
        blackBackgroundCanvasGroup = BlackBackground.GetComponent<CanvasGroup>();

        background.SetActive(false);
        openingScreen.SetActive(true);
        BlackBackground.SetActive(true);
        

        if(!isOpeningScreen) StartCoroutine(ShowOpeningScreen());
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
            // if (Random.value <= chanceToShow)
            // {
            //     TriggerEasterEgg();
            // }
            // StartCoroutine(ShowEsterEgg());
                
        }
        // CloseOptionMenu();
        isEnter();
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
        // yield return StartCoroutine(ShowMainMenu());

        if(hintText != null)
        {
            hintText.SetActive(true);
            Debug.Log("Click right hand button to open the bag");
        }
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

    public void CloseOptionMenu()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left-click
        {
            Vector2 mousePosition = Input.mousePosition;

            // Check if the mouse is outside the darker image
            if (!RectTransformUtility.RectangleContainsScreenPoint(optionArea, mousePosition))
            {
                Debug.Log("Clicked outside the darker area, returning to main menu.");
                mainMenu.SetActive(true);
                optionMenu.SetActive(false);

            }
        }
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