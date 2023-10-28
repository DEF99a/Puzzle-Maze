using Assets.Scripts.Define;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario : PlayerController
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
                ProcessAnim();
                //Debug.Log("Mario end shoot");
            }
        }
    }

    protected override void Action()
    {
        base.Action();
        AudioManager.instance.PlaySound(SoundName.skill_mario);

        //Debug.Log("Mario shoot");
        var boneGunTip = skeletonAnimation.skeleton.FindBone("gun_tip");
        var gunTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip.WorldX, boneGunTip.WorldY, 0f));
        var go = ObjectPool.instance.GetGameObject(bulletPf, gunTipPos, Quaternion.identity);

        go.GetComponent<MarioBullet>().Setup(new Assets.Scripts.Bullet.ItemParam()
        {
            direct = direction,
            speed = 7,
            characterBase = this,
            characterId = this.EntityId
        });
    }
}
