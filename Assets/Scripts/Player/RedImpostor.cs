using Assets.Scripts.Bullet;
using Assets.Scripts.Define;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedImpostor : PlayerController
{
    [SerializeField] private GameObject bulletPf;

    protected override void HandleCompleteAnim(TrackEntry trackEntry)
    {
        base.HandleCompleteAnim(trackEntry);
        if (trackEntry.Animation.Name.Equals(this.actionAnimName))
        {
            this.actionAnimName = "";
            if (!isDie)
            {
                //bool isShoot = false;
                //var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
                //if (location.tileType == TileType.Dynamic)
                //{
                //    if (location.entityBase is BoxBase box)
                //    {
                //        box.Setup(new ItemParam()
                //        {
                //            characterId = this.EntityId,
                //            characterBase = this,
                //            direct = direction,
                //            speed = 0
                //        });
                //        isShoot = true;
                //    }
                //}
                //if (!isShoot)
                //{
                //    var enemies = MapManager.instance.enemies;
                //    var cellOnFront = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]).ToVector3Int();

                //    foreach (var en in enemies)
                //    {
                //        if (en.IsLive() && en.gameObject.activeInHierarchy)
                //        {
                //            var enCell = MyGraph.instance.GetCellFromPosition(en.transform.position, out _);
                //            if(cellOnFront == enCell)
                //            {
                //                en.Killed();
                //            }
                //        }
                //    }
                //}

                Idle(0);
                skeletonAnimation.loop = true;
                ProcessAnim();
                //Debug.Log("Mario end shoot");
            }
        }
    }

    protected override void Action()
    {
        base.Action();
        //Debug.Log("Mario shoot");
        AudioManager.instance.PlaySound(SoundName.skill_impostor);

        var boneGunTip = skeletonAnimation.skeleton.FindBone("gun_tip");
        var gunTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip.WorldX, boneGunTip.WorldY, 0f));
        var go = ObjectPool.instance.GetGameObject(bulletPf, gunTipPos, Quaternion.identity);

        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);

        go.GetComponent<RedImpostorKnife>().Setup(direction, location.ToVector3());
    }
}
