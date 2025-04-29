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
    public HandPoseDetector detector;
    public List<GameObject> objects; // Reference to the object1
    private Dictionary<GameObject, Vector3> lastKnownPositions = new Dictionary<GameObject, Vector3>(); // Store last positions of objects on the platform
    #endregion

    #region Behaviours Methods
    // Start is called before the first frame update
    void Start()
    {
        //Get all objects in bag
        
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                lastKnownPositions[obj] = obj.transform.position - plateform.transform.position; // Store the last known position of the object
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        // CheckObjInOutBag();
    }
    #endregion

    #region Public Methods
    public void OpenBag()
    {
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

        // Activate the platform
        plateform.SetActive(true);

        // Move objects to their last known positions on the platform
        foreach (GameObject obj in objects)
        {
            Debug.Log(obj.name);
            if (obj != null)
            {
                // obj.transform.position = plateform.transform.position + lastKnownPositions[obj];
                obj.SetActive(true); // Activate the object
            }
        }

        Debug.Log("Bag opened successfully.");
    }

    public void CloseBag()
    {

        plateform.SetActive(false);
        foreach (GameObject obj in objects)
        {
            if (obj != null && IsObjectOnPlate(obj))
            {
                lastKnownPositions[obj] = obj.transform.position - plateform.transform.position; // Store the last known position of the object
            }
            obj.SetActive(false); // Deactivate the object
        }
        plateform.SetActive(false); // Deactivate the platform
        Debug.Log("Bag closed");
    }
    public void SwitchBagObj()
    {
        // Check if the detector has detected a pose
        HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
        if (detectedPose != null && detectedPose.name == "Point")
        {
            Debug.Log("Detected pose: Point");

            // Find the currently active object in the bag
            GameObject currentActiveObject = objects.Find(obj => obj.activeSelf);

            // Deactivate the currently active object
            if (currentActiveObject != null)
            {
                currentActiveObject.SetActive(false);
                Debug.Log("Deactivated object: " + currentActiveObject.name);
            }

            // Activate the first object in the list
            if (objects.Count > 0 && objects[0] != null)
            {
                objects[0].SetActive(true);
                Debug.Log("Activated object: " + objects[0].name);
            }
        }
    }
    #endregion

    #region Private Methods
    private void CheckObjInOutBag()
    {
        // Define the region around the platform to check for objects
        Vector3 platformCenter = plateform.transform.position;
        Vector3 platformSize = plateform.GetComponent<Collider>().bounds.size; // Get the platform's size
        // float checkHeight = 1.0f; // Height above the platform to check for objects
        Vector3 checkRegion = new Vector3(platformSize.x, platformSize.y, platformSize.z);

        // Check for objects in the region around the platform
        Collider[] colliders = Physics.OverlapBox(platformCenter, checkRegion / 2, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            GameObject obj = collider.gameObject;

            // Check if the object is in the bag list, has the "Fetchable" tag, and is not already in bagObjects
            if (objects.Contains(obj) && obj.CompareTag("Fetchable") && !objects.Contains(obj))
            {
                // Add the object to the bagObjects list
                objects.Add(obj);
                Debug.Log("Object added to bagObjects: " + obj.name);
            }
        }

        // Remove objects from bagObjects if they are no longer on the plate
        objects.RemoveAll(obj => !IsObjectOnPlate(obj));
    }

    // Helper function to check if an object is on the plate
    private bool IsObjectOnPlate(GameObject obj)
    {
        if (obj == null) return false;

        // Get the bounds of the plateform
        Bounds plateBounds = plateform.GetComponent<Collider>().bounds;

        // Check if the object's position is within the bounds of the plateform
        return plateBounds.Contains(obj.transform.position);
    }
    
    #endregion
}