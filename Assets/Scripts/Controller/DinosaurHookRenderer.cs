using Assets.Scripts.Bullet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurHookRenderer : MonoBehaviour
{
    static Dictionary<Vector3, Vector2> colliderOffsetByDirects = new Dictionary<Vector3, Vector2>()
    {
        {Vector3.up, new Vector2(0, 0.2f)}, {Vector3.down, new Vector2(0, -0.2f)},
        {Vector3.right, new Vector2(0.2f, 0)}, {Vector3.left, new Vector2(-0.2f, 0)},
    };

    [SerializeField] private DinosaurHookMovement hookMovement;

    private BoxCollider2D box2D;
    private LineRenderer lineRenderer;

    private Vector3 direct;
    private Vector3 startPos;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        box2D = GetComponent<BoxCollider2D>();

        box2D.size = Vector3.zero;
        box2D.enabled = false;
        gameObject.SetActive(false);
    }

    public void Setup(Vector3 dir, Vector3 sp)
    {
        direct = dir;
        startPos = sp;
    }

    public void Render(Vector3 endPosition, bool enable)
    {
        if (enable)
        {
            if (!lineRenderer.enabled) lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;

        }
        else
        {
            lineRenderer.positionCount = 0;
            if (lineRenderer.enabled) lineRenderer.enabled = false;
        }

        if (lineRenderer.enabled)
        {
            Vector3 temp = startPos;
            temp.z = PlayerController.instance.transform.position.z + 0.01f;
            lineRenderer.SetPosition(0, temp);

            temp = endPosition;
            temp.z = PlayerController.instance.transform.position.z + 0.01f;
            endPosition = temp;
            lineRenderer.SetPosition(1, endPosition);
        }
        AddColliderToLine(endPosition, enable);
    }

    private void AddColliderToLine(Vector3 endPos, bool enable)
    {
        if (enable)
        {
            float lineLength = Vector3.Distance(startPos, endPos); // length of line

            if (direct == Vector3.left || direct == Vector3.right)
                box2D.size = new Vector3(lineLength, 0.1f, 1f);
            else box2D.size = new Vector3(0.1f, lineLength, 1f);

            box2D.offset = colliderOffsetByDirects[direct];

            Vector3 midPoint = (startPos + endPos) / 2;
            box2D.transform.position = midPoint;

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
        //Debug.Log("target:" + collision.gameObject.name + " collision:" + collision.tag);
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
                    speed = 7
                });
            }
            hookMovement.Reverse();
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
                    speed = 7
                });
            }
            hookMovement.Reverse();
        }
        else if (collision.CompareTag("Enemy"))
        {
            hookMovement.Reverse();
        }
        else if (collision.CompareTag("BongBong"))
        {
            PlayerController.instance.ProcessEatItem(collision.gameObject, 1);
        }
    }

    public void Hide()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;

        box2D.size = Vector3.zero;
        box2D.enabled = false;

        gameObject.SetActive(false);
    }
}
