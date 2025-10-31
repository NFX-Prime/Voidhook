using UnityEngine;

public class Bobber : MonoBehaviour
{
    private bool isFishing = false;
    private PondFishing pond;

    public void OnEnterPond(PondFishing pond)
    {
        if (isFishing) return;
        isFishing = true;
        this.pond = pond;

        pond.StartFishing(this);
    }

    public void OnFishHooked()
    {
        Debug.Log("Fish hooked!");
        StartReelPhase();
    }

    void StartReelPhase()
    {
        // Starts the reeling UI and waits for completion
        ReelInWheel.Instance.StartReeling(OnFishCaught);
    }

    void OnFishCaught()
    {
        Debug.Log("Fish caught!");
        GameManager.Instance.AddFish();
        Destroy(gameObject);
    }

    public void OnFishLost()
    {
        Debug.Log("Fish got away!");
        Destroy(gameObject);
    }
}