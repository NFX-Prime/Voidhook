using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.ComponentModel.Design.Serialization;
public class FishUI : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;
    private VisualElement root;

    public VisualElement fishPanel;
    // Update is called once per frame
    public RawImage backgroundRawImage;
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = uiDoc.rootVisualElement;

        VisualElement fishPanel = root.Q<VisualElement>("FishPanel");
        
    }
    void Update()
    {
        Rect uvRect = backgroundRawImage.uvRect;
        uvRect.x += scrollSpeedX * Time.deltaTime;
        uvRect.y += scrollSpeedY * Time.deltaTime;
        backgroundRawImage.uvRect = uvRect;
        }
}
