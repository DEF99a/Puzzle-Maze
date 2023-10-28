using Assets.Scripts.Common;
using Assets.Scripts.Hellper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weepinbell : EnemyController
{
    [SerializeField] private float speedShootFireBall = 10f;
    [SerializeField] private GameObject fireBallPf;

    protected override bool SpecialAction()
    {
        //shouldTurn = false;
        var per = actionRate / 100;
        var rd = Random.Range(0, 1f);
        //Debug.Log("actionRate:" + per + " rd:" + rd);
        if (rd <= per && PlayerController.instance.IsLive() && !PlayerController.instance.InGuard)
        {
            Location tmpLocal;
            int k = 0;
            bool canShoot = false;
            do
            {
                tmpLocal = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * k);
                if (EntityHelper.IsPlayer(tmpLocal) || EntityHelper.IsBox(tmpLocal)
                    || EntityHelper.IsPlayer(tmpLocal))
                {
                    if (EntityHelper.IsPlayer(tmpLocal))
                        canShoot = true;
                    break;
                }
                k++;
            } while (k < Config.LimitWhile && tmpLocal.tileType != TileType.Obtacle);

            if (canShoot && k > 2)
            {
                Debug.Log("k  = " + k);
                var go = ObjectPool.instance.GetGameObject(fireBallPf,
                    MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]).ToVector3(), Quaternion.identity);
                go.GetComponent<FireBall>().Setup(new Assets.Scripts.Bullet.ItemParam()
                {
                    direct = direction,
                    speed = speedShootFireBall,
                    characterBase = this,
                    characterId = this.EntityId
                });
                AudioManager.instance.PlaySound(SoundName.skill_weipinbell);
                //shouldTurn = true;
                return true;
            }
        }
        return false;
    }
}
