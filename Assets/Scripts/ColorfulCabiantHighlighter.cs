using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorfulCabiantHighlighter : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Highlight Settings")]
    public float highlightDistance = 1.0f;
    public Color highlightColor = Color.white;

    private List<Material> CabiantMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private bool isHighlighted = false;

    void Start()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform child in childTransforms)
        {
            if (child.gameObject.activeSelf && child != transform)
            {
                Renderer[] renderers = child.GetComponents<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    Material mat = renderer.material;
                    if (mat != null && !CabiantMaterials.Contains(mat))
                    {
                        CabiantMaterials.Add(mat);
                        originalColors.Add(mat.color);
                    }
                }
            }
        }

        if (CabiantMaterials.Count == 0)
        {
            Debug.LogError("ColorfulCabiantHighlighter: not found any materials in children.");
        }
    }

    void Update()
    {
        if (player == null || CabiantMaterials.Count == 0) return;

        float distance = Vector3.Distance(player.position, transform.position);
        // Debug.Log("Cabiant Distance to player: " + distance);

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
        for (int i = 0; i < CabiantMaterials.Count; i++)
        {
            if (highlight)
            {
                CabiantMaterials[i].color = Color.Lerp(originalColors[i], highlightColor, 0.3f);
            }
            else
            {
                CabiantMaterials[i].color = originalColors[i];
            }
        }

        isHighlighted = highlight;
    }

    public bool IsCurrentlyHighlighted()
    {
        return isHighlighted;
    }
}
