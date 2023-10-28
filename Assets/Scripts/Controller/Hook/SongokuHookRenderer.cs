using Assets.Scripts.Bullet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongokuHookRenderer : MonoBehaviour
{
    [SerializeField] private SongokuHookMovement hookMovement;

    private BoxCollider2D box2D;
    public LineRenderer LineRenderer { get; private set; }

    private Vector3 direct;
    private Vector3 startPos;

    private void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
        box2D = GetComponent<BoxCollider2D>();

        box2D.size = Vector3.zero;
        box2D.enabled = false;
        gameObject.SetActive(false);
    }

    public void Setup(Vector3 dir, Vector3 sp)
    {
        direct = dir;
        startPos = sp;
        LineRenderer.startWidth = 0.5f;
        LineRenderer.endWidth = 0.5f;
    }

    public void Render(Vector3 endPosition, bool enable)
    {
        if (enable)
        {
            if (!LineRenderer.enabled) LineRenderer.enabled = true;
            LineRenderer.positionCount = 2;

        }
        else
        {
            LineRenderer.positionCount = 0;
            if (LineRenderer.enabled) LineRenderer.enabled = false;
        }

        if (LineRenderer.enabled)
        {
            Vector3 temp = startPos;
            temp.z = PlayerController.instance.transform.position.z + 0.01f;
            LineRenderer.SetPosition(0, temp);

            temp = endPosition;
            temp.z = PlayerController.instance.transform.position.z + 0.01f;
            endPosition = temp;
            LineRenderer.SetPosition(1, endPosition);
        }
        AddColliderToLine(endPosition, enable);
    }

    private void AddColliderToLine(Vector3 endPos, bool enable)
    {
        if (enable)
        {
            float lineLength = Vector3.Distance(startPos, endPos) + 0.5f;
            if (direct == Vector3.left || direct == Vector3.right)
                box2D.size = new Vector3(lineLength, 0.4f, 1f);
            else box2D.size = new Vector3(0.4f, lineLength, 1f);

            Vector3 midPoint = (startPos + endPos) / 2;
            transform.position = midPoint; // setting position of collider object

            if (gameObject.activeSelf == false)
            {
                box2D.enabled = true;
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (gameObject.activeSelf)
            {
                box2D.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        if (collision.CompareTag("BoxBound"))
        {
            var box = collision.transform.parent.GetComponent<BoxBase>();
            if (box != null)
            {
                box.Setup(new ItemParam()
                {
                    characterId = PlayerController.instance.EntityId,
                    characterBase = PlayerController.instance,
                    direct = EntityBase.posDirects[direct],
                    speed = 0
                });
            }
            hookMovement.Hide(false);
            PlayerController.instance.EndAction();
        }
        else if (collision.CompareTag("Box"))
        {
            var box = collision.transform.GetComponent<BoxBase>();
            if (box != null)
            {
                box.Setup(new ItemParam()
                {
                    characterId = PlayerController.instance.EntityId,
                    characterBase = PlayerController.instance,
                    direct = EntityBase.posDirects[direct],
                    speed = 0
                });
            }
            hookMovement.Hide(false);
            PlayerController.instance.EndAction();
        }
        else if (collision.CompareTag("BongBong"))
        {
            PlayerController.instance.ProcessEatItem(collision.gameObject, 1);
        }
    }

    public void Hide()
    {
        LineRenderer.positionCount = 0;
        LineRenderer.enabled = false;

        box2D.size = Vector3.zero;
        box2D.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
