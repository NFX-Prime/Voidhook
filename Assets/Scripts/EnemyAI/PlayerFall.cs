using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    public Transform player;
    private void OnCollisionEnter(Collision collision)
    {
        player.position = new Vector3(-33.7f, 5.41f, 37.88f);
    }
}
