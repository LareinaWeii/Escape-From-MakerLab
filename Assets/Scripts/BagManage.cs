//TODO: only show on object once and use pose detecor to detect the object
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class BagManage : MonoBehaviour
{
    #region Variables
    public GameObject plateform;
    public GameObject bag;
    public GameObject player;
    public HandPoseDetector detector;
    public List<GameObject> objects; // Reference to the object1
    public bool isBagOpen = false; // Flag to check if the bag is open
    private Vector3 diffOfPlateformAndPlayer; // Distance between the platform and the player
    private Dictionary<GameObject, Vector3> lastKnownPositions = new Dictionary<GameObject, Vector3>(); // Store last positions of objects on the platform
    #endregion

    #region Behaviours Methods
    // Start is called before the first frame update
    void Start()
    {
        // diffOfPlateformAndPlayer = plateform.transform.position - player.transform.position; // Calculate the distance between the platform and the player
        //Get all objects in bag
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.CompareTag("InBag"))
            {
                lastKnownPositions[obj] = obj.transform.position - plateform.transform.position; // Store the last known position of the object
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        CheckObjInOutBag();
    }
    #endregion

    #region Public Methods
    public void OpenBag()
    {
        //TODO: check if the bag cannot open
        // Define the region around the platform to check for collisions
        // Vector3 platformCenter = plateform.transform.position;
        // Vector3 platformSize = plateform.GetComponent<Collider>().bounds.size; // Get the platform's size
        // float checkHeight = 1.0f; // Height above the platform to check for collisions
        // Vector3 checkRegion = new Vector3(platformSize.x, checkHeight, platformSize.z);

        // // Check for collisions in the region around the platform
        // Collider[] colliders = Physics.OverlapBox(platformCenter, checkRegion / 2, Quaternion.identity);
        // foreach (Collider collider in colliders)
        // {
        //     if (collider.gameObject != plateform && collider.gameObject.activeSelf)
        //     {
        //         //TODO show the information
        //         Debug.Log("Collision detected with: " + collider.gameObject.name);
        //         Debug.Log("Cannot open the bag because the platform is obstructed.");
        //         return; // Exit the method if there is a collision
        //     }
        // }
        isBagOpen = true;

        // Activate the platform
        // plateform.transform.position = player.transform.position + diffOfPlateformAndPlayer; // Set the platform position to the player's position
        plateform.SetActive(true);

        // Move objects to their last known positions on the platform
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.CompareTag("InBag"))
            {
                obj.transform.position = plateform.transform.position + lastKnownPositions[obj];
                obj.SetActive(true); // Activate the object
            }
        }

        Debug.Log("Bag opened successfully.");
    }

    public void CloseBag()
    {
        // CheckObjInOutBag(); // Check if objects are in or out of the bag
        // Deactivate the platform and objects
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.CompareTag("InBag"))
            {
                lastKnownPositions[obj] = obj.transform.position - plateform.transform.position; // Store the last known position of the object
                obj.SetActive(false); // Deactivate the object
            }
        }
        plateform.SetActive(false); // Deactivate the platform
        isBagOpen = false; // Set the bag as closed
    }
    #endregion

    #region Private Methods
    private void CheckObjInOutBag()
    {
        Collider plateformCollider = plateform.GetComponent<Collider>();
        // Update in bag or not tags
        foreach (GameObject obj in objects)
        {
            if(obj != null && obj.activeSelf)
            {
                Collider objCollider = obj.GetComponent<Collider>();
                if(objCollider != null && plateformCollider != null && objCollider.bounds.Intersects(plateformCollider.bounds))
                {
                    obj.tag = "InBag"; // Set the tag to "InBag" if the object is on the platform
                }
                else
                {
                    obj.tag = "Fetchable"; // Set the tag to "Fetchable" if the object is not on the platform
                }
            }
        }

    }
    
    #endregion
}