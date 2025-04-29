using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;


public class SceneSwitcher : MonoBehaviour
{
    public GameObject card;
    public GameObject cardReader;
    [Tooltip("Index of the scene to load when pressing space")]
    public int sceneToLoadIndex = 1; // Default to Scene2 (index 1)
    
    void Update()
    {
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
                SceneManager.LoadScene(sceneToLoadIndex);
            }
        }
    }

}