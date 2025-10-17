using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionHandler : MonoBehaviour
{
    // Get player and the layers to block
    [Header("References")]
    public Transform player;          
    // SET EACH OBJECT TO THE OBSTRUCTION LAYER TO PASS THROUGH! ITS IN ALL CAPS LIKE THIS!
    public LayerMask obstructionMask; 

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadedAlpha = 0.3f;
    public float fadeSpeed = 5f;

    // List of objects that will be passed through.
    private List<Renderer> currentObstructions = new List<Renderer>();

    // Using a Coroutine systemd that allows it to fade over a brief period. Like an animation of sorts.
    private Dictionary<Renderer, Coroutine> fadeCoroutines = new Dictionary<Renderer, Coroutine>();

    void Update()
    {
        if (player == null) return;

        // Cast a ray from the camera to the player. This ray is going to be used to see if something is hitting the ray.
        Vector3 direction = player.position - transform.position;
        float distance = Vector3.Distance(transform.position, player.position);

        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, obstructionMask);

        // Collect renderers hit by the ray
        List<Renderer> newObstructions = new List<Renderer>();
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                newObstructions.Add(rend);

                // If it's new, fade it out
                if (!currentObstructions.Contains(rend))
                {
                    StartFade(rend, fadedAlpha);
                }
            }
        }

        // Restore any old obstructions no longer blocking
        foreach (Renderer oldRend in currentObstructions)
        {
            if (!newObstructions.Contains(oldRend))
            {
                StartFade(oldRend, 1f);
            }
        }

        currentObstructions = newObstructions;
    }

    /// <summary>
    /// Function that starts fading the object.
    /// </summary>
    /// <param name="rend"></param>
    /// <param name="targetAlpha"></param>
    private void StartFade(Renderer rend, float targetAlpha)
    {
        if (fadeCoroutines.ContainsKey(rend))
        {
            StopCoroutine(fadeCoroutines[rend]);
            fadeCoroutines.Remove(rend);
        }

        fadeCoroutines[rend] = StartCoroutine(FadeMaterial(rend, targetAlpha));
    }

    private System.Collections.IEnumerator FadeMaterial(Renderer rend, float targetAlpha)
    {
        Material mat = rend.material;
        Color color = mat.color;
        float startAlpha = color.a;

        // Enable transparency mode
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        while (Mathf.Abs(color.a - targetAlpha) > 0.01f)
        {
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
            mat.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        mat.color = color;

        // If fully opaque, reset to normal rendering mode
        if (Mathf.Approximately(targetAlpha, 1f))
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = -1;
        }

        fadeCoroutines.Remove(rend);
    }
}
