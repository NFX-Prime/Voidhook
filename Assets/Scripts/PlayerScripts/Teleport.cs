using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform targetDest;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Movement>() != null)
        {
            other.transform.position = targetDest.position;
        }

    }
}
