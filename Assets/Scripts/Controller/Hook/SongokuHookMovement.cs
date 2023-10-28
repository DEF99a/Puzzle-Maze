using Assets.Scripts.Bullet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongokuHookMovement : MonoBehaviour
{
    public static SongokuHookMovement instance;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private SongokuHookRenderer hookRenderer;

    private SpriteRenderer spriteRenderer;

    private Vector3 direct;
    private bool startMove;

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

        transform.position = startPosition;

        direct = dir;
        startMove = true;
        isDie = false;

        if (dir != Vector3.down) spriteRenderer.sortingOrder = -1;
        else spriteRenderer.sortingOrder = 1;

        if (dir == Vector3.up || dir == Vector3.down)
            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        transform.localScale = originLocalScale;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!startMove) return;

        transform.position += moveSpeed * Time.deltaTime * direct;

        hookRenderer.Render(transform.position, true);
    }

    public void Hide(bool force)
    {
        if (isDie == true) return;

        isDie = true;
        startMove = false;

        if(force)
        {
            hookRenderer.Hide();
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else StartCoroutine(HideIE());

        IEnumerator HideIE()
        {
            float lineWidth = hookRenderer.LineRenderer.startWidth;
            var lineWidthStep = lineWidth / 5;
            var ls = transform.localScale;
            var lsStep = ls.x / 5;
            while(true)
            {
                lineWidth -= lineWidthStep;
                hookRenderer.LineRenderer.startWidth = lineWidth;
                hookRenderer.LineRenderer.endWidth = lineWidth;
                ls.x -= lsStep;
                ls.y -= lsStep;
                transform.localScale = ls;
                if (lineWidth <= 0f) break;
                yield return new WaitForSeconds(0.015f);
            }

            hookRenderer.Hide();
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            AudioManager.instance.StopSound(SoundName.skill_goku_loop);
            AudioManager.instance.PlaySound(SoundName.skill_goku_end);
        }      
    }
}
