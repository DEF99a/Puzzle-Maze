using Assets.Scripts.Define;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario2 : PlayerController
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
                Idle(0);
                skeletonAnimation.loop = true;
            }
        }
    }

    protected override void Action()
    {
        base.Action();
        AudioManager.instance.PlaySound(SoundName.skill_bomberman);

        //var boneGunTip = skeletonAnimation.skeleton.FindBone("gun_tip");
        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        var pos = cell + new Vector3(0.5f, 0.5f);
        pos += directPos[direction];

        //var gunTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip.WorldX, boneGunTip.WorldY, 0f));
        var go = ObjectPool.instance.GetGameObject(bulletPf, pos, Quaternion.identity);
        go.GetComponent<Mario2Bomb>().FirstInit(new Assets.Scripts.Bullet.ItemParam()
        {
            characterBase = this,
            characterId = this.EntityId,
            direct = direction,
            speed = shootSpeed * 0.5f
            //speed = 0.5f
        });
    }
}
