using Unity.VisualScripting;
using UnityEngine;

public class TargetSwap : MonoBehaviour {
    public BossAI bossSwitch;

    public bool targetted = false;

    private void OnTriggerEnter(Collider target)
    {
        targetted = false;
        bossSwitch.SwitchTarget();
    }
}

