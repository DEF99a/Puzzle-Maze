using Assets.Scripts.Bullet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurHookMovement : MonoBehaviour
{
    public static DinosaurHookMovement instance;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private DinosaurHookRenderer hookRenderer;

    private SpriteRenderer spriteRenderer;

    private Vector3 direct;
    private Vector3 offset, originPosition;
    private bool startMove;
    private bool forward;
    private Vector3 originLocalScale;
    private bool isDie;

    private void Awake()
    {
        instance = this;

        originLocalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void Show(Vector3 startPosition, Vector3 dir)
    {
        hookRenderer.Setup(dir, startPosition);

        originPosition = startPosition;
        transform.position = startPosition;

        direct = dir;
        offset = Vector3.zero;
        startMove = true;
        forward = true;
        isDie = false;

        if (dir != Vector3.down)
            spriteRenderer.sortingOrder = -1;
        else spriteRenderer.sortingOrder = 1;

        if (dir == Vector3.up)
            transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
        else if (dir == Vector3.down)
            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        else if (dir == Vector3.left)
            transform.localScale = originLocalScale;
        else if (dir == Vector3.right)
        {
            var temp = originLocalScale;
            temp.x = -temp.x;
            transform.localScale = temp;
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!startMove) return;

        offset += moveSpeed * Time.deltaTime * direct;
        transform.position += moveSpeed * Time.deltaTime * direct;
        if (forward && offset.magnitude >= maxDistance)
        {
            direct *= -1;
            forward = false;
        }

        hookRenderer.Render(transform.position, true);

        if (!forward && Vector3.Distance(transform.position, originPosition) <= 0.1f)
        {
            Hide();

            transform.rotation = Quaternion.identity;
            transform.localScale = originLocalScale;

            PlayerController.instance.EndAction();
        }
    }

    public void Reverse()
    {
        if (isDie == true) return;
        isDie = true;
        direct *= -1;
        forward = false;
    }

    public void Hide()
    {
        startMove = false;

        hookRenderer.Hide();

        gameObject.SetActive(false);
    }
}
