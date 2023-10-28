using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bound
{
    public Vector3 min;
    public Vector3 max;

    public bool InBound(Vector3 localPlace)
    {
        if(localPlace.x >= min.x && localPlace.x <= max.x
                && localPlace.y >= min.y && localPlace.y <= max.y)
        {
            return true;
        }
        return false;
    }
}


/// <summary>
/// Thuc ra chi co 2 loai map la Enemy( co be quai ) va All(khong be quai)
/// </summary>
public enum MapType
{
    None,
    Enemy,
    Puzzle,
    All
}

public class Level : MonoBehaviour
{
    public static Level instance;

    public int totalTime = 60;
    public int timeToGetStar = 20;

    public int sameTimeEnemyAmount = 2;
    public EnemyTypeAmount[] enemyTypeAmounts;

    [HideInInspector] public int totalEnemy;
    [HideInInspector] public List<Bound> bounds = new List<Bound>();

    [HideInInspector] public MapType mapType;

    private void Awake()
    {
        instance = this;
        bounds.Clear();

        totalEnemy = 0;
        for (int k = 0; k < enemyTypeAmounts.Length; k++)
        {
            totalEnemy += enemyTypeAmounts[k].amount;
        }
        if (totalEnemy > 0) mapType = MapType.Enemy;
        else mapType = MapType.All;
    }

    public Bound GetBound(Vector3 position)
    {
        if (bounds == null) bounds = new List<Bound>();
        //Debug.Log("bounds.Count:" + bounds.Count);

        if (bounds.Count == 0)
        {
            var boundsTf = transform.Find("Bounds");
            //Debug.Log("boundsTf:" + (boundsTf == null));
            if (boundsTf != null)
            {
                for (int k = 0; k < boundsTf.childCount; k++)
                {
                    var boundTf = boundsTf.GetChild(k);
                    var min = boundTf.Find("Min").position;
                    var max = boundTf.Find("Max").position;

                    int tmpMinX = min.x < 0 ? ((int)min.x - 1) : (int)min.x;
                    int tmpMinY = min.y < 0 ? ((int)min.y - 1) : (int)min.y;

                    int tmpMaxX = max.x < 0 ? (int)max.x : ((int)max.x + 1);
                    int tmpMaxY = max.y < 0 ? (int)max.y : ((int)max.y + 1);

                    var bound = new Bound()
                    {
                        min = new Vector3(tmpMinX, tmpMinY),
                        max = new Vector3(tmpMaxX, tmpMaxY)
                    };
                    bounds.Add(bound);
                    Debug.Log("minX:" + min.x + " bX:" + bound.min.x);

                }
            }
        }

        for (int k = 0; k < bounds.Count; k++)
        {
            var bound = bounds[k];

            if (position.x >= bound.min.x && position.x <= bound.max.x
                && position.y >= bound.min.y && position.y <= bound.max.y)
            {
                return bound;
            }
        }
        return null;
    }
}
