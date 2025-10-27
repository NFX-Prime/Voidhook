using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    public Camera mainCam;

    public GameObject followedObj;


    private void Start()
    {
        mainCam.transform.Rotate(new Vector3(45, 0, 0));
    }

    private void Update()
    {
        mainCam.transform.position = new Vector3(followedObj.transform.position.x, followedObj.transform.position.y + 10, followedObj.transform.position.z - 10);
        
    }

}
