using Assets.Scripts.Define;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Browser : EnemyController
{
    [SerializeField] private float speedShootFireBall = 10f;
    [SerializeField] private GameObject fireBallPf;

    protected override bool SpecialAction()
    {
        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
        var location2 = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * 2);
        //shouldTurn = false;
        if (location.tileType == TileType.CanMove && location2.tileType == TileType.CanMove)
        {
            var per = actionRate / 100;
            var rd = Random.Range(0, 1f);
            //Debug.Log("actionRate:" + per + " rd:" + rd);
            if (rd <= per)
            {
                //shouldTurn = true;

                state = DynamicEntityState.Action;

               
                return true;
            }
        }

        return false;
    }

    protected override void Action()
    {
        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
        if (location.tileType == TileType.CanMove)
        {
            var go = ObjectPool.instance.GetGameObject(fireBallPf, location.ToVector3(), Quaternion.identity);
            go.GetComponent<FireBall>().Setup(new Assets.Scripts.Bullet.ItemParam()
            {
                direct = direction,
                speed = speedShootFireBall,
                characterBase = this,
                characterId = this.EntityId
            });
            AudioManager.instance.PlaySound(SoundName.skill_bowser);
        }

        shouldTurnDirect = true;
        Idle();
    }
}
