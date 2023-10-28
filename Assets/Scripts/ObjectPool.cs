using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectAmount
{
    public GameObject pf;
    public int amount;
}

[System.Serializable]
public class ObjectCategory
{
    public string name;
    public List<ObjectAmount> objectAmounts;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private List<ObjectCategory> objectCates;

    private Dictionary<int, List<GameObject>> cache;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        cache = new Dictionary<int, List<GameObject>>();

        for (int i = 0; i < objectCates.Count; i++)
        {
            var objectCate = objectCates[i];

            for(int j  = 0; j < objectCate.objectAmounts.Count; j++)
            {
                var objectAmount = objectCate.objectAmounts[j];

                var pfId = objectAmount.pf.GetInstanceID();
                cache.Add(pfId, new List<GameObject>());

                int count = objectAmount.amount;
#if UNITY_EDITOR
                count = 1;
#endif

                for (int k = 0; k < count; k++)
                {
                    var go = Instantiate(objectAmount.pf, transform);
                    go.SetActive(false);
                    cache[pfId].Add(go);
                }
            } 
        }
    }

    public GameObject GetGameObject(GameObject pf, Vector3 position, Quaternion quaternion, bool enable = true)
    {
        var pfId = pf.GetInstanceID();
        if (!cache.ContainsKey(pfId))
            cache.Add(pfId, new List<GameObject>());                
        var list = cache[pf.GetInstanceID()];

        GameObject go = null;
        for (int k = 0; k < list.Count; k++)
        {
            go = list[k];
            if (go != null && !go.activeSelf)
            {
                var tf = go.transform;
                tf.position = position;
                tf.rotation = quaternion;
                go.SetActive(enable);
                return go;
            }
        }

        go = Instantiate(pf, position, quaternion);
        go.transform.SetParent(transform);
        go.SetActive(enable);
        cache[pfId].Add(go);

        //Debug.Log("GetGameObject:" + pf.gameObject.name);

        return go;
    }

    public void Disable(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        go.transform.parent = transform;
    }

    public void DisableAll()
    {
        foreach(var pair in cache)
        {
            foreach(var go in pair.Value)
            {
                Disable(go);
            }    
        }    
    }
}
