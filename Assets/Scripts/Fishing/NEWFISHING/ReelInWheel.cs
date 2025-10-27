using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;




public class ReelInWheel : MonoBehaviour
{
    public static ReelInWheel Instance;

    // Assign the UI Image here
    public Image wheelImage; 

    // How fast it fills
    public float fillSpeed = 0.4f;

    // How fast it drains when not pressing
    public float drainSpeed = 0.2f; 
    private System.Action onComplete;
    private bool active = false;
    private float fill = 0f;

    void Awake()
    {
        // UI hidden when not reeling
        Instance = this;
        //gameObject.SetActive(false); 
    }

    /// <summary>
    /// Function that is used in PondFishing to complete the fishing and add fish to count (IN PROGRESS)
    /// </summary>
    /// <param name="onFinished"></param>
    public void StartReeling(System.Action onFinished)
    {
        onComplete = onFinished;
        fill = 0f;
        wheelImage.fillAmount = 0f;
        active = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!active) return;

        // Player is reeling (Holding Left Mouse Button)
        bool isReeling = Mouse.current.leftButton.isPressed;

        if (isReeling)
        {
            fill += fillSpeed * Time.deltaTime;
        }
        else
        {
            fill -= drainSpeed * Time.deltaTime;
        }

        fill = Mathf.Clamp01(fill);
        wheelImage.fillAmount = fill;

        if (fill >= 1f)
        {
            active = false;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
