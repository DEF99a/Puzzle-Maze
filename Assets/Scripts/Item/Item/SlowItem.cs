using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowItem : MonoBehaviour
{
    public void Init()
    {
        StartCoroutine(RunToEnd());
    }

    IEnumerator RunToEnd()
    {
        yield return new WaitForSeconds(5f);
        ObjectPool.instance.Disable(gameObject);
    }

    private void LateUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y / 1000;
        transform.position = pos;
    }
}
