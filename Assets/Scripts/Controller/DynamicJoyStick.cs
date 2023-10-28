using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicJoyStick : MonoBehaviour
{
    public static DynamicJoyStick Instance;

    [SerializeField] private Transform bgStick;
    [SerializeField] private Transform smallStick;

    Vector3 startPosition, firstPositionMove;
    float stickRadius;

    void Start()
    {
        Instance = this;

        stickRadius = bgStick.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        firstPositionMove = bgStick.transform.position;
    }

    void Update()
    {
        if (!GameManager.instance.IsPlaying || !PlayerController.instance.IsLive())
        {
            bgStick.transform.position = firstPositionMove;
            smallStick.transform.position = firstPositionMove;
            return;
        }
        ProcessMouseInput();
    }

    void ProcessMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var touchPos = GetWorldPosition(Input.mousePosition);
            // Debug.Log("WTF:" + touchPos);

            startPosition = touchPos;
            bgStick.transform.position = startPosition;
            smallStick.transform.position = startPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            var touchPos = GetWorldPosition(Input.mousePosition);

            Vector3 distance = touchPos - startPosition;
            Vector3 direction = distance.normalized;
            float stickDistance = Vector3.Distance(touchPos, startPosition);
            // if (stickDistance <= 0.1f)
            //     return;

            if (stickDistance < stickRadius)
            {
                smallStick.transform.position = startPosition + direction * stickDistance;
            }
            else
            {
                smallStick.transform.position = startPosition + direction * stickRadius;
            }

            var tan = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            PlayerController.instance.MoveByJoyStick(tan);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            bgStick.transform.position = firstPositionMove;
            smallStick.transform.position = firstPositionMove;
            PlayerController.instance.SetDirectionButton(Assets.Scripts.Define.DynamicEntityDirection.None);
        }
    }

    //void ProcessTouch()
    //{
    //    int i = 0;

    //    while (i < Input.touchCount)
    //    {
    //        // Debug.Log("touch:" + i);

    //        Touch t = Input.GetTouch(i);
    //        var touchPos = GetWorldPosition(t.position);

    //        if (t.phase == TouchPhase.Began)
    //        {
    //            if (t.position.x > Screen.width / 2)
    //            {
    //                playerShooting.Shoot();
    //                shootFingerId = t.fingerId;
    //                btnShoot.transform.position = touchPos;
    //            }
    //            else
    //            {
    //                moveFingerId = t.fingerId;
    //                Debug.Log(t.fingerId);

    //                startPosition = touchPos;
    //                bgStick.transform.position = startPosition;
    //                smallStick.transform.position = startPosition;
    //            }
    //        }
    //        else if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) && moveFingerId == t.fingerId)
    //        {
    //            Vector3 distance = touchPos - startPosition;
    //            Vector3 direction = distance.normalized;

    //            float stickDistance = Vector3.Distance(touchPos, startPosition);
    //            if (stickDistance <= 0.1f)
    //                return;

    //            if (stickDistance < stickRadius)
    //            {
    //                smallStick.transform.position = startPosition + direction * stickDistance;
    //            }
    //            else
    //            {
    //                smallStick.transform.position = startPosition + direction * stickRadius;
    //            }

    //            var tan = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //            playerMovement.MoveByJoyStick(tan);
    //        }
    //        else if (t.phase == TouchPhase.Ended)
    //        {
    //            if (t.fingerId == moveFingerId)
    //            {
    //                bgStick.transform.position = firstPositionMove;
    //                smallStick.transform.position = firstPositionMove;
    //                playerMovement.MoveStop();
    //                moveFingerId = 99;
    //            }
    //            else
    //            {
    //                btnShoot.transform.position = firstPositionShoot;
    //                shootFingerId = 100;
    //            }
    //        }
    //        i++;
    //    }
    //}

    Vector3 GetWorldPosition(Vector3 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 100));
    }
}
