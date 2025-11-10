using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // Setting instance so we can use it in the movement scripts too and the enemy AI scripts.
    public static PlayerState Instance;

    // This tells enemies if the player is hidden
    public bool isHidden = false;

    private void Awake()
    {
        Instance = this;
    }
}