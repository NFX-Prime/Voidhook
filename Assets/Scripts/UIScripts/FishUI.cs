using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.ComponentModel.Design.Serialization;
public class FishUI : MonoBehaviour
{
    [SerializeField] 
    UIDocument uiDoc;
    private VisualElement root;
    public VisualElement fishPanel;
    Rect uvRect;

    public RawImage backgroundRawImage;
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;

    void Start()
    {
        root = uiDoc.rootVisualElement;

        VisualElement fishPanel = root.Q<VisualElement>("FishPanel");
        
    }
    void Update()
    {
        uvRect = backgroundRawImage.uvRect;
        uvRect.x += scrollSpeedX * Time.deltaTime;
        uvRect.y += scrollSpeedY * Time.deltaTime;
        backgroundRawImage.uvRect = uvRect;

    }
}
