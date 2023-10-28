using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedJoystick : Joystick
{
    [SerializeField] private Image upBtn;
    [SerializeField] private Image downBtn;
    [SerializeField] private Image leftBtn;
    [SerializeField] private Image rightBtn;

    Dictionary<DynamicEntityDirection, Image> pairs;

    private void Awake()
    {
        pairs = new Dictionary<DynamicEntityDirection, Image>();

        upBtn = transform.Find("Up").GetChild(0).GetComponent<Image>();
        downBtn = transform.Find("Down").GetChild(0).GetComponent<Image>();
        leftBtn = transform.Find("Left").GetChild(0).GetComponent<Image>();
        rightBtn = transform.Find("Right").GetChild(0).GetComponent<Image>();

        pairs.Add(DynamicEntityDirection.Up, upBtn);
        pairs.Add(DynamicEntityDirection.Down, downBtn);
        pairs.Add(DynamicEntityDirection.Left, leftBtn);
        pairs.Add(DynamicEntityDirection.Right, rightBtn);
        Debug.Log("fffffffffff");

        foreach (var p in pairs)
        {
            var color = p.Value.color;
            color.a = 0.5f;
            p.Value.color = color;
        }
    }

    private bool isTouching;

    public override void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
        base.OnPointerDown(eventData);
        //Debug.Log("downnnnnnn");
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;

        base.OnPointerUp(eventData);

        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
            PlayerController.instance.SetDirectionButton(Assets.Scripts.Define.DynamicEntityDirection.None);

        foreach (var p in pairs)
        {
            var color = p.Value.color;
            color.a = 0.5f;
            p.Value.color = color;
        }
    }

    private void Update()
    {
        if (isTouching)
        {
            var direct = new Vector2(Horizontal, Vertical);
            //Debug.Log("direct:" + direct.magnitude);

            if (direct.magnitude < 0.1f) return;

            var normalizedPoint = direct.normalized;
            //Debug.Log("Horizontal:" + Horizontal);
            //Debug.Log("Vertical:" + Vertical);

            var angle = GeneralHelper.GetAngleFromVectorFloat(normalizedPoint);
            if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
            {
                //Debug.Log("direct 2:" + direct.magnitude);
                var dir = PlayerController.instance.MoveByJoyStick(angle);
                EnableBtn(dir);
            }
        }
    }

    private void EnableBtn(DynamicEntityDirection dynamicEntityDirection)
    {
        foreach (var p in pairs)
        {
            var color = p.Value.color;
            if (p.Key == dynamicEntityDirection)
            {
                color.a = 1f;
            }
            else
            {
                color.a = 0.5f;
            }
            p.Value.color = color;
        }
    }
}