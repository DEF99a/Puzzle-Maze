using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    [SerializeField] private List<RectTransform> bgRectTransforms;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private float timeSpeed = 0.05f;

    private float timer;
    private Vector3 offset;

    private void Awake()
    {
        offset = bgRectTransforms[2].localPosition - bgRectTransforms[1].localPosition;
    }

    void Update()
    {
        if(timer < Time.time)
        {
            timer = Time.time + timeSpeed;
            RectTransform bgFirst = null;
            for (int k = 0; k < bgRectTransforms.Count; k++)
            {
                bgRectTransforms[k].localPosition += new Vector3(-1, 1);

                Camera cam = Camera.main;
                float height = 2f * cam.orthographicSize;
                float width = height * cam.aspect;

                var loc = RectTransformUtility.WorldToScreenPoint(Camera.main, bgRectTransforms[k].GetChild(0).position);
                var sc = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector3(-width * 0.5f, height * 0.5f));
                //var inRect = RectTransformUtility.RectangleContainsScreenPoint(canvasRect, loc);
                if (loc.x < sc.x && loc.y > sc.y && k == 0)
                {
                    bgFirst = bgRectTransforms[k];
                }
            }
            if(bgFirst != null)
            {
                bgFirst.localPosition = bgRectTransforms[2].localPosition + offset;

                bgRectTransforms[0] = bgRectTransforms[1];
                bgRectTransforms[1] = bgRectTransforms[2];
                bgRectTransforms[2] = bgFirst;
            }    
        }       
    }
}
