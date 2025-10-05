using UnityEngine;

public class PlayerShadow : MonoBehaviour
{

    // Prefab of a flat circle or some sort of shadow for the player to see. (INPUT IT IN THE INSPECTOR! - James)
    public GameObject shadowPrefab;
    private GameObject shadowInstance;

    public float shadowYOffset = 0.01f;
    // Maximum raycast length. Change as needed.
    public float maxShadowDistance = 20f;

    // Scale variable to change size of shadow based on distance from ground (DOESNT WORK RN)
    //private float scale = 0f; 

    // Setting ray up
    Ray ray;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Instantiating shadow underneath player
        shadowInstance = Instantiate(shadowPrefab);
        
    }

    // Update is called once per frame
    void Update()
    {
        // Casting ray straight below player
        ray = new Ray(transform.position, Vector3.down);

        // Checking to see if ray hit something below
        if (Physics.Raycast(ray, out RaycastHit hit, maxShadowDistance))
        {
            // Activate the shadow
            shadowInstance.SetActive(true);

            // Set position of shadow onto the ground. (Should be on the ground at all times)
            shadowInstance.transform.position = hit.point + Vector3.up * shadowYOffset;

            // Rotate shadow for whatever grounds
            shadowInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Scaling shadow based on height (DOESN"T WORK RN FORGET ABOUT THIS!!!)
            //scale = Mathf.Clamp(hit.distance / 5f, 0.3f, 1f);
            //shadowInstance.transform.localScale = new Vector3(scale, 1, scale);


        }
        // If no ground detected...
        else
        {
            // Simply put the shadow away.
            shadowInstance.SetActive(false);
        }


    }
}
