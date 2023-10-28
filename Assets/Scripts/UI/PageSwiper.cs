using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform parent;
    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages = 1;
    public int currentPage = 1;
    public int offsetWidth = 20;

    float width;
    bool cantMoveToLeft, cantMoveToRight;

    Vector3 firstLocation;
    // Start is called before the first frame update
    private void Awake()
    {
        width = Screen.width;
        firstLocation = parent.transform.localPosition;
        Debug.Log("firstLocation:" + firstLocation);
    }


    public void Show()
    {
        parent.transform.localPosition = firstLocation - new Vector3(width * (currentPage - 1), 0, 0);
        Debug.Log("parent.transform.localPosition:" + parent.transform.localPosition);

        panelLocation = parent.transform.localPosition;
        CheckMoveDirect();

        gameObject.SetActive(true);
    }

    //private void OnDisable()
    //{
    //    currentPage = 1;
    //}


    public void OnDrag(PointerEventData data)
    {
        Debug.Log("OnDrag");
        float difference = data.pressPosition.x - data.position.x;
        if (cantMoveToRight && difference > 0) return;
        if (cantMoveToLeft && difference < 0) return;
        parent.transform.localPosition = panelLocation - new Vector3(difference, 0, 0);
        Debug.Log("dif 22:" + difference + " panelLocation 22:" + panelLocation);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / width;
        if (cantMoveToRight && percentage > 0) return;
        if (cantMoveToLeft && percentage < 0) return;
        //Debug.Log("percentage:" + percentage);
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            MovePage(percentage);
        }
        else
        {
            StartCoroutine(SmoothMove(parent.transform.localPosition, panelLocation, easing));
        }
    }

    public void MovePage(float percentage)
    {
        Vector3 newLocation = panelLocation;
        if (percentage > 0 && currentPage < totalPages)
        {
            currentPage++;
            //var width = Screen.width;
            newLocation += new Vector3(-(width + offsetWidth), 0, 0);
        }
        else if (percentage < 0 && currentPage > 1)
        {
            currentPage--;
            //var width = Screen.width;
            newLocation += new Vector3((width + offsetWidth), 0, 0);
        }
        PanelMap.instance.OnPageChange(currentPage);

        StartCoroutine(SmoothMove(parent.transform.localPosition, newLocation, easing));
        panelLocation = newLocation;
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            parent.transform.localPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        CheckMoveDirect();
    }

    void CheckMoveDirect()
    {
        cantMoveToLeft = cantMoveToRight = false;

        if (currentPage == totalPages)
        {
            cantMoveToRight = true;
            cantMoveToLeft = false;
        }

        if (currentPage == 1)
        {
            cantMoveToLeft = true;
            cantMoveToRight = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
    }
}

