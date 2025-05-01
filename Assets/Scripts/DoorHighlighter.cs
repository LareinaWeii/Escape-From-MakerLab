using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHighlighter : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Highlight Settings")]
    public float highlightDistance = 0.9f;
    public Color highlightColor = Color.white;

    private Material DoorMaterials;
    private Color originalColors;
    private bool isHighlighted = false;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material mat = renderer.material;

        if (mat != null)
        {
            DoorMaterials = mat;
            originalColors = mat.color;
        }

        if (DoorMaterials == null)
        {
            Debug.LogError("DoorHighlighter: not found any materials.");
        }
    }

    void Update()
    {
        // Debug.Log("DoorHighlighter: Update called");
        if (player == null || DoorMaterials != null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        Debug.Log("Door Distance to player: " + distance);

        if (distance <= highlightDistance && !isHighlighted)
        {
            Highlight(true);
        }
        else if (distance > highlightDistance && isHighlighted)
        {
            Highlight(false);
        }
    }

    void Highlight(bool highlight)
    {
        if (highlight)
        {
            DoorMaterials.color = Color.Lerp(originalColors, highlightColor, 0.2f);
        }
        else
        {
            DoorMaterials.color = originalColors;
        }


        isHighlighted = highlight;
    }

    public bool IsCurrentlyHighlighted()
    {
        return isHighlighted;
    }
}
