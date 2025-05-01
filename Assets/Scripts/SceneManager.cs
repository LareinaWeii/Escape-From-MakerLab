using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;


public class SceneSwitcher : MonoBehaviour
{
    // TODO Careful that SceneManager.cs's class name is SceneSwitcher, not SceneManager
    public GameObject card;
    public GameObject cardReader;
    public bool isOpeningScreenPassed = false;
    private Vector3 initialCardPosition; // Variable to store the initial position of the card
    [Tooltip("Index of the scene to load when pressing space")]
    public int sceneToLoadIndex = 1; // Default to Scene2 (index 1)

    void Start()
    {
        // Record the initial position of the card
        if (card != null)
        {
            initialCardPosition = card.transform.position;
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed. Reloading scene.");
            ReloadScene();
        }

        isEnter();
    }

    private void isEnter()
    {
        // Check if the card and cardReader are colliding
        if (card != null && cardReader != null)
        {
            Collider cardCollider = card.GetComponent<Collider>();
            Collider cardReaderCollider = cardReader.GetComponent<Collider>();

            if ((cardCollider != null && cardReaderCollider != null && cardCollider.bounds.Intersects(cardReaderCollider.bounds)) ||
                (Input.GetKeyDown(KeyCode.Tab)))
            {
                Debug.Log("Card collided with CardReader!");
                isOpeningScreenPassed = true;
                SceneManager.LoadScene(sceneToLoadIndex);
            }
        }
    }
    public void ReloadScene()
    {
        Debug.Log("Reloading scene...");

        // Reset the card's position
        if (card != null)
        {
            card.transform.position = initialCardPosition;
        }
    } 

}