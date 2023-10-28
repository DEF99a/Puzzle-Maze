using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Songoku : PlayerController
{
    protected override void Action()
    {
        base.Action();
        SongokuHookMovement.instance.gameObject.SetActive(true);
        AudioManager.instance.PlaySound(SoundName.skill_goku_loop);

        Vector3 dir = directPos[direction];

        var boneGunTip = skeletonAnimation.skeleton.FindBone("gun_tip");
        var gunTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip.WorldX, boneGunTip.WorldY, 0f));
        SongokuHookMovement.instance.Show(gunTipPos, dir);
    }

    protected override IEnumerator DieIE()
    {
        yield return null;

        state = DynamicEntityState.Die;
        skeletonAnimation.loop = false;

        SongokuHookMovement.instance.Hide(true);
    }
}
