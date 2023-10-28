using UnityEngine;
using UnityEngine.UI;

public class SrollBg : MonoBehaviour
{
    public GameObject quadGameObject;
    private Image quadRenderer;

    [SerializeField] float scrollSpeedX = 0.5f;
    [SerializeField] float scrollSpeedY = 0.2f;

    void Start()
    {
        quadRenderer = quadGameObject.GetComponent<Image>();
    }

    void Update()
    {
        Vector2 textureOffset = new Vector2(Time.time*scrollSpeedX,Time.time*scrollSpeedY);
        quadRenderer.material.mainTextureOffset = textureOffset;
    }
}