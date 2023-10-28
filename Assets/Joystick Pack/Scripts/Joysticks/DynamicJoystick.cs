using Assets.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicJoystick : Joystick
{
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private Image focus;
    [SerializeField] private int haha;

    private bool isTouching;
    private Vector3 firstAnchor;

    protected override void Start()
    {
        MoveThreshold = moveThreshold;
        base.Start();
        firstAnchor = background.anchoredPosition;
        //background.gameObject.SetActive(false);
        focus = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        focus.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;

        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;

        //background.gameObject.SetActive(false);
        background.anchoredPosition = firstAnchor;

        base.OnPointerUp(eventData);

        focus.gameObject.SetActive(false);

        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
            PlayerController.instance.SetDirectionButton(Assets.Scripts.Define.DynamicEntityDirection.None);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }

    private void Update()
    {
        if (isTouching)
        {
            var direct = new Vector2(Horizontal, Vertical);
            if (direct.magnitude < 0.1f) return;

            var normalizedPoint = direct.normalized;
            //Debug.Log("Horizontal:" + Horizontal);
            //Debug.Log("Vertical:" + Vertical);

            var angle = GeneralHelper.GetAngleFromVectorFloat(normalizedPoint);
            if (!focus.gameObject.activeSelf) focus.gameObject.SetActive(true);

            focus.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
                PlayerController.instance.MoveByJoyStick(angle);
        }
    }
}