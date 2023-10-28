using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinosaur : PlayerController
{
    protected override void Action()
    {
        base.Action();

        Vector3 dir = Vector3.left;
        if (direction == DynamicEntityDirection.Down)        
            dir = Vector3.down;        
        else if(direction == DynamicEntityDirection.Up)
            dir = Vector3.up;
        else if(direction == DynamicEntityDirection.Right)
            dir = Vector3.right;
        AudioManager.instance.PlaySound(SoundName.skill_dino);

        var boneGunTip = skeletonAnimation.skeleton.FindBone("gun_tip");
        var gunTipPos = skeletonAnimation.transform.TransformPoint(new Vector3(boneGunTip.WorldX, boneGunTip.WorldY, 0f));
        DinosaurHookMovement.instance.Show(gunTipPos, dir);
    }

    protected override IEnumerator DieIE()
    {
        yield return null;

        state = DynamicEntityState.Die;
        skeletonAnimation.loop = false;

        DinosaurHookMovement.instance.Hide();
    }
}
