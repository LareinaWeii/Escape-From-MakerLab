using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.PhysicalHands;

public class FlipColor : MonoBehaviour
{
    [Header("References")]
    private List<GameObject> cubes; // List of cubes to change color
    private List<Material> originalMaterials;// Store original materials of cubes
    public PhysicalHandsManager physicalHandsManager; // Leap Motion service provider
    public HandPoseDetector detector;
    public GameObject leftHandWist;
    public GameObject rightHandWist;
    private GameObject targetCube; // The cube that will be changed
    private bool noCubeIsSelected = true; // Flag to check if a cube is selected
    private bool finished = false; // Flag to check if the cube is finished changing color

    [Header("Grid Settings")]
    public int gridWidth = 5; // Set this to your grid width
    public int gridHeight = 5; // Set this to your grid height

    // Start is called before the first frame update
    void Start()
    {
        // Get all cubes
        cubes = new List<GameObject>();
        foreach (Transform child in transform) // Iterate through all direct children
        {
            cubes.Add(child.gameObject);
        }

        // Get all materials of the cubes
        originalMaterials = new List<Material>();
        foreach (GameObject cube in cubes)
        {
            if (cube != null)
            {
                Renderer cubeRenderer = cube.GetComponent<Renderer>();
                if (cubeRenderer != null)
                {
                    // Store a NEW INSTANCE of the material to avoid shared references
                    originalMaterials.Add(new Material(cubeRenderer.material));
                }
            }
        }

    }
    void Update()
    {
        noCubeIsSelected = true; // Reset the flag at the start of each frame
        foreach (GameObject cube in cubes)
        {
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            // Debug.Log("Cube origin material color: " + cubeMaterial.color);
            if (IsCubeSelected(cube)) // Check if the cube is active
            {
                if (cubeRenderer.material.color != Color.white)
                {
                    // Debug.Log("A Cube is selected");
                    cubeRenderer.material = new Material(cubeRenderer.material); // Create a new instance of the material
                    cubeRenderer.material.color = Color.white; //Highlight the cube
                }
                noCubeIsSelected = false; // Set the flag to true
                HandPoseScriptableObject detectedPose = detector.GetCurrentlyDetectedPose();
                if (detectedPose != null && detectedPose.name == "Point")
                {
                    targetCube = cube; // Set the target cube to the currently selected cube
                    Debug.Log("Target Cube: " + targetCube.name);
                }
            }
            else
            {
                // Reset the cube's color to its original color
                int cubeIndex = cubes.IndexOf(cube);
                if (cubeIndex >= 0 && cubeIndex < originalMaterials.Count)
                {
                    cubeRenderer.material = originalMaterials[cubeIndex];
                    // Debug.Log("Cube original material: " + originalMaterials[cubeIndex]);
                }   
            }
        }

        finished = true; // Reset the flag at the start of each frame
        foreach(GameObject cube in cubes)
        {
            Color cube0Color= cubes[0].GetComponent<Renderer>().material.color;
            if (cube.GetComponent<Renderer>().material.color != cube0Color)
            {
                finished = false; // Set the flag to false
            }
        }

        
        if(finished)
        {
            Debug.Log("Finished changing color!");
        }
        else
        {
            Debug.Log("Not finished changing color!");
        }

        // if(CheckFinished())
        // {
        //     finished = true; // Set the flag to true
        //     Debug.Log("Finished changing color!");
        // }

    }
    // Update is called once per frame

    private bool CheckFinished()
    {
        foreach(GameObject cube in cubes)
        {
            Debug.Log("Cube color: " + cube.GetComponent<Renderer>().material.name + " cubes[0] color: " + cubes[0].GetComponent<Renderer>().material.name);
            Color cube0Color= cubes[0].GetComponent<Renderer>().material.color;
            if (cube.GetComponent<Renderer>().material.color != cube0Color)
            {
                return false; // Not all cubes have the same color
            }
        }
        return true; // All cubes have the same color
    }


    private bool IsCubeSelected(GameObject cube)
    {
        Vector3 leftHandWorldPos = leftHandWist.transform.position;
        Vector3 rightHandWorldPos = rightHandWist.transform.position;
        Bounds cubeBounds = cube.GetComponent<Renderer>().bounds;
        float cubeMinY = cubeBounds.min.y; // Lowest height of the cube
        float cubeMaxY = cubeBounds.max.y; // Highest height of the cube
        float cubeMinX = cubeBounds.min.x; // Lowest height of the cube
        float cubeMaxX = cubeBounds.max.x; // Highest height of the cube

        // Check if one hand's pos is within the cube's pos range
        if (rightHandWorldPos.y >= cubeMinY && rightHandWorldPos.y <= cubeMaxY
        && rightHandWorldPos.x >= cubeMinX && rightHandWorldPos.x <= cubeMaxX) 
        // ||(leftHandWorldPos.y >= cubeMinY && leftHandWorldPos.y <= cubeMaxY 
        // && leftHandWorldPos.x >= cubeMinX && leftHandWorldPos.x <= cubeMaxX))
        {
            // Debug.Log("A hand is within the cube's height range");
            return true;
        }
        return false;    
    }

    public void ChangeMaterial(Material targetMaterial)
    {
        if (targetCube == null)
        {
            Debug.Log("No target cube selected!");
            return;
        }

        Debug.Log("Target Cube: " + targetCube.name);

        Renderer cubeRenderer = targetCube.GetComponent<Renderer>();
        if (cubeRenderer != null)
        {  
            int cubeIndex = cubes.IndexOf(targetCube);
            
            if(cubeIndex >= 0 && cubeIndex < originalMaterials.Count)
            {
                Material originalMaterial = originalMaterials[cubeIndex];
                Material newMaterial = new Material(targetMaterial);
                int row = cubeIndex / gridWidth; // Calculate the row index
                int col = cubeIndex % gridWidth; // Calculate the column index
                Flip(row, col, originalMaterial, newMaterial); // Call the FloodFill function to change the color

                // cubeRenderer.material = newMaterial;
                // originalMaterials[cubeIndex] = newMaterial;
                // cubeRenderer.material.color = originalMaterials[cubeIndex].color;  
                // targetCube = null; // Reset the target cube after changing the material
                // Debug.Log("Material changed to: " + targetMaterial.name);
            }
        }
        else
        {
            Debug.LogError("No Renderer component found on the GameObject!");
            return;
        }
    }
    private void Flip(int row, int col, Material originalMaterial, Material newMaterial)
    {   
        // Check if the original material is the same as the new material
        if (originalMaterial == newMaterial)
            return;
        if (row < 0 || row >= gridHeight)
        {
            Debug.Log("Row out of bounds: " + row);
            return; // Out of bounds check
        }
        if (col < 0 || col >= gridWidth)
        {
            Debug.Log("Column out of bounds: " + col);
            return; // Out of bounds check
        }

        int cubeIndex = row * gridWidth + col; // Calculate the index of the cube in the list
        if(originalMaterials[cubeIndex].color != originalMaterial.color)
        {
            Debug.Log("Cube's original material is not the same as the original material: " + originalMaterials[cubeIndex] + " != " + originalMaterial);
            return; // Check if the cube's material is the same as the original material
        }
        
        // Flip the cube's color
        GameObject targetCube = cubes[cubeIndex]; // Get the cube at the calculated index
        Renderer cubeRenderer = targetCube.GetComponent<Renderer>();
        cubeRenderer.material = newMaterial;
        originalMaterials[cubeIndex] = newMaterial; // Update the original material to the new material
        Debug.Log("Cube flipped: " + targetCube.name);

        Flip(row, col-1, originalMaterial, newMaterial); // Check the cube to the left
        Flip(row, col+1, originalMaterial, newMaterial); // Check the cube to the right
        Flip(row-1, col, originalMaterial, newMaterial); // Check the cube above
        Flip(row+1, col, originalMaterial, newMaterial); // Check the cube below

    }
}
