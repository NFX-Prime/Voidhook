using UnityEngine;

public class PlayerFall : MonoBehaviour
{

    public Transform targetDest;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = targetDest.position;
        }

    }
}
