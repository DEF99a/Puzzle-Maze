using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    const float Z = -10;

    //private GameObject player;

    [HideInInspector] public Vector3 min;
    [HideInInspector] public Vector3 max;

    private Vector3 offset;

    private void Awake()
    {
        instance = this;
    }
           
    public void Init(Vector3 startPos, Vector3 _min, Vector3 _max)
    {
        this.min = _min;
        this.max = _max;

        //var widthTarget = (max.x - min.x);
        //var heightTarget = (max.y - min.y);

        //Camera.main.orthographicSize = widthTarget / (2 * Camera.main.aspect);

        //if (Camera.main.orthographicSize * 2 > heightTarget)
        //{
        //    Camera.main.orthographicSize = heightTarget / 2;
        //}
        //if (Camera.main.orthographicSize >= 6) Camera.main.orthographicSize = 6;

        //Debug.Log("startPos:" + startPos + "min:" + min + " max:" + max);

        //var position = player.transform.position;
        transform.position = Limit(startPos);
        //Debug.Log("Cam:" + transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerController.instance == null || GameManager.instance.IsStop) return;
        var position = PlayerController.instance.transform.position - offset;
        var pos = Limit(position);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 2);
    }

    private Vector3 Limit(Vector3 position)
    {
        float orthSize = Camera.main.orthographicSize;
        var aspect = Camera.main.aspect;

        //Debug.Log("this.min.x:" + this.min.x + " aspect:" + Math.Round(aspect, 2) + " a:" +
        //    (this.min.x + orthSize * aspect) + " b:" + (this.max.x - orthSize * aspect));

        if (position.x < (this.min.x + orthSize * aspect))
        {
            position.x = this.min.x + orthSize * aspect;
        }
        else if (position.x > (this.max.x - orthSize * aspect))
        {
            position.x = this.max.x - orthSize * aspect;
        }
        if (position.y < (this.min.y + orthSize))
        {
            position.y = this.min.y + orthSize;
        }
        else if (position.y > (this.max.y - orthSize))
        {
            position.y = this.max.y - orthSize;
        }
        position.z = Z;

        return position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(min, 0.1f);
        Gizmos.DrawSphere(max, 0.1f);
    }

    public bool IsInCamera(Vector3 position)
    {
        float halfHeight = Camera.main.orthographicSize;
        var aspect = Camera.main.aspect;

        if (position.x >= (transform.position.x - halfHeight * aspect) && position.x <= (transform.position.x + halfHeight * aspect)
            && position.y >= (transform.position.y - halfHeight) && position.y <= (transform.position.y + halfHeight))
        {
            return true;
        }
        return false;
    }
}
