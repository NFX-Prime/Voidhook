using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    public Camera mainCam;

    public GameObject followedObj;

    // CHANGE UP FOV SETTINGS TO BETTER SUIT OUR GAME!!
    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float hidingFOV = 35f;
    public float zoomSpeed = 5f;

    private float targetFOV;

    private void Start()
    {
        mainCam.transform.Rotate(new Vector3(45, 0, 0));
        mainCam.fieldOfView = normalFOV;
    }

    private void Update()
    {
        // Follow player
        mainCam.transform.position = new Vector3(followedObj.transform.position.x, followedObj.transform.position.y + 10, followedObj.transform.position.z - 10);

        // Determine FOV if player is hiding. (Yes, then change to hiding FOV. No, then change to normal)
        targetFOV = PlayerState.Instance.isHidden ? hidingFOV : normalFOV;

        // Change camera based on POV
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

}
