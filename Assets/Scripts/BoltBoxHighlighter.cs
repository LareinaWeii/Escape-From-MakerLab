using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltBoxHighlighter : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Highlight Settings")]
    public float highlightDistance = 0.9f;
    public Color highlightColor = Color.white;

    private List<Material> boltBoxMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private bool isHighlighted = false;

    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            if (mat != null && !boltBoxMaterials.Contains(mat))
            {
                boltBoxMaterials.Add(mat);
                originalColors.Add(mat.color);
            }
        }

        if (boltBoxMaterials.Count == 0)
        {
            Debug.LogError("BoltBoxHighlighter: not found any materials in children.");
        }
    }

    void Update()
    {
        if (player == null || boltBoxMaterials.Count == 0) return;

        float distance = Vector3.Distance(player.position, transform.position);
        Debug.Log("BoltBox Distance to player: " + distance);

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
        for (int i = 0; i < boltBoxMaterials.Count; i++)
        {
            if (highlight)
            {
                boltBoxMaterials[i].color = Color.Lerp(originalColors[i], highlightColor, 0.2f);
            }
            else
            {
                boltBoxMaterials[i].color = originalColors[i];
            }
        }

        isHighlighted = highlight;
    }

    public bool IsCurrentlyHighlighted()
    {
        return isHighlighted;
    }
}
