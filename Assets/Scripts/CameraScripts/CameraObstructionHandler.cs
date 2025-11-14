using UnityEngine;
using System.Collections.Generic;

public class CameraObstruction : MonoBehaviour
{
    public Transform player;
    public float raycastRadius = 0.3f; 
    // SET AS THE LONG LAYER "OBSTACLESTOFADEOUTWHNEBLOCKINGCAMERA"
    // Each object should be in this layer if they are to be turned transparent.
    public LayerMask obstructMask;     

    private List<Renderer> currentObstructions = new List<Renderer>();
    private List<Renderer> previousObstructions = new List<Renderer>();

    void Update()
    {
        HandleObstructions();
    }

    void HandleObstructions()
    {
        currentObstructions.Clear();

        // Raycast from camera to player
        Vector3 dir = player.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.position);

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position, raycastRadius, dir, dist, obstructMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
                currentObstructions.Add(rend);
        }

        // ---------------------------
        // Fade NEW obstructions (once)
        // ---------------------------
        foreach (Renderer r in currentObstructions)
        {
            if (!previousObstructions.Contains(r))
            {
                SetRendererOpacity(r, 0.2f); // fade amount
            }
        }

        // ---------------------------
        // Restore objects no longer blocked (once)
        // ---------------------------
        foreach (Renderer r in previousObstructions)
        {
            if (!currentObstructions.Contains(r))
            {
                SetRendererOpacity(r, 1f); // restore
            }
        }

        // Swap list references (much faster than copying)
        var temp = previousObstructions;
        previousObstructions = currentObstructions;
        currentObstructions = temp;
    }

    void SetRendererOpacity(Renderer rend, float alpha)
    {
        foreach (Material mat in rend.materials)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;

            // Ensure material supports transparency
            if (alpha < 1f)
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = 3000;
                Debug.Log("Alpha less than 1f. Transparent object on.");
            }
 
            else
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.renderQueue = -1;
                Debug.Log("Resetting object transparency");
            }
            
        }
    }
}