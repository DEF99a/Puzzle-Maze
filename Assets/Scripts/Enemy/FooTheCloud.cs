using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FooTheCloud : EnemyController
{
    [SerializeField] private GameObject trapItemPf;
    
    private float timer;

    protected override bool SpecialAction()
    {
        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);

        var per = actionRate / 100;
        var rd = Random.Range(0, 1f);
        //shouldTurn = false;
        //Debug.Log("actionRate:" + per + " rd:" + rd);
        if (rd <= per && timer < Time.time)
        {
            //Debug.Log("location.tileType:" + location.tileType);
            if (location.tileType == TileType.CanMove)
            {
                timer = Time.time + 1f;
                //var go = ObjectPool.instance.GetGameObject(trapItemPf, location.ToVector3(), Quaternion.identity);
                //go.GetComponent<TrapItem>().Setup();
                state = DynamicEntityState.Action;
                return true;
            }
        }
        return false;
    }

    protected override void Action()
    {
        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);

        var go = ObjectPool.instance.GetGameObject(trapItemPf, location.ToVector3(), Quaternion.identity, false);
        go.GetComponent<TrapItem>().Setup(false);
        AudioManager.instance.PlaySound(SoundName.skill_foo);
    }
}
