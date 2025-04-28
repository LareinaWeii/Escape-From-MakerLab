using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryHighlighter : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Highlight Settings")]
    public float highlightDistance = 0.9f;
    public Color highlightColor = Color.white;

    private List<Material> SentryMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();
    private bool isHighlighted = false;

    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            if (mat != null && !SentryMaterials.Contains(mat))
            {
                SentryMaterials.Add(mat);
                originalColors.Add(mat.color);
            }
        }

        if (SentryMaterials.Count == 0)
        {
            Debug.LogError("SentryHighlighter: not found any materials in children.");
        }
    }

    void Update()
    {
        if (player == null || SentryMaterials.Count == 0) return;

        float distance = Vector3.Distance(player.position, transform.position);
        // Debug.Log("Sentry Distance to player: " + distance);

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
        for (int i = 0; i < SentryMaterials.Count; i++)
        {
            if (highlight)
            {
                SentryMaterials[i].color = Color.Lerp(originalColors[i], highlightColor, 0.2f);
            }
            else
            {
                SentryMaterials[i].color = originalColors[i];
            }
        }

        isHighlighted = highlight;
    }

    public bool IsCurrentlyHighlighted()
    {
        return isHighlighted;
    }
}
