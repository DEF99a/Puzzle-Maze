using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koffing : EnemyController
{
    [SerializeField] private GameObject pushItemPf;

    protected override bool SpecialAction()
    {
        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);

        var per = actionRate / 100;
        var rd = Random.Range(0, 1f);
        //Debug.Log("actionRate:" + per + " rd:" + rd);
        if (rd <= per)
        {
            //Debug.Log("location.tileType:" + location.tileType);
            if (location.tileType == TileType.CanMove)
            {
                state = Assets.Scripts.Define.DynamicEntityState.Action;
                //var go = ObjectPool.instance.GetGameObject(pushItemPf, location.ToVector3(), Quaternion.identity);
                //go.GetComponent<SlowItem>().Init();
                return true;
            }
        }
        return false;
    }

    protected override void Action()
    {
        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
        if(location.tileType == TileType.CanMove)
        {
            var go = ObjectPool.instance.GetGameObject(pushItemPf, location.ToVector3(), Quaternion.identity);
            go.GetComponent<SlowItem>().Init();

            AudioManager.instance.PlaySound(SoundName.skill_koffling);
        }      

        shouldTurnDirect = false;
        Idle();
    }
}
