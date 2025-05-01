using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    
    #region Variables
    [Header("-----------UI Elements-----------")]
    public GameObject Over;
    public GameObject Retry;
    
    [Header("-----------Others-----------")]
    public float splashDuration = 4.0f;

    [Header("----------Private--------")]
    private CanvasGroup overCanvasGroup;
    private CanvasGroup retryCanvasGroup;
    private bool isOpeningScreen = false;
    private MainSystem mainSystem;
    
    
    #endregion
    #region Behaviours Methods
    public void Start()
    {
        mainSystem = GetComponent<MainSystem>();
        overCanvasGroup = Over.GetComponent<CanvasGroup>();
        retryCanvasGroup = Retry.GetComponent<CanvasGroup>();

        Over.SetActive(false);
        Retry.SetActive(false);

        // if(!isOpeningScreen) StartCoroutine(ShowOpeningScreen());
    }
    public void Update()
    {
        if (mainSystem.gameState == 2)
        {
            Over.SetActive(true);
            Retry.SetActive(false);
            StartCoroutine(ShowGameoverScreen());
        }
        else if (mainSystem.gameState == 1)
        {
            Over.SetActive(false);
            Retry.SetActive(true);
            StartCoroutine(ShowRetryScreen());
        }
        else if (mainSystem.gameState == 0)
        {
            Over.SetActive(false);
            Retry.SetActive(false);
        }
    }
    #endregion

    #region Methods
    private IEnumerator ShowGameoverScreen()
    {   
        isOpeningScreen = true;
        // Fade in the opening screen
        yield return StartCoroutine(FadeCanvasGroup(overCanvasGroup, 0f, 1f, splashDuration));
        // yield return new WaitForSeconds(splashDuration);
        
        // // Fade out the opening screen and black background
        // yield return StartCoroutine(FadeCanvasGroup(overCanvasGroup, 1f, 0f, splashDuration));
        // Over.SetActive(false);
        // mainSystem.gameState = 0;
        // yield return StartCoroutine(ShowMainMenu());
    }

    private IEnumerator ShowRetryScreen()
    {   
        isOpeningScreen = true;
        // Fade in the opening screen
        yield return StartCoroutine(FadeCanvasGroup(retryCanvasGroup, 0f, 1f, splashDuration));
        yield return new WaitForSeconds(splashDuration);
        
        // Fade out the opening screen and black background
        yield return StartCoroutine(FadeCanvasGroup(retryCanvasGroup, 1f, 0f, splashDuration));
        Retry.SetActive(false);
        mainSystem.gameState = 0;
        // yield return StartCoroutine(ShowMainMenu());
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
    
    #endregion
}
