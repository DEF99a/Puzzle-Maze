using Assets.Scripts.Bullet;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mario2BombExplodeType
{
    Center,
    Head,
    Body
}

public class Mario2BombExplodeParam
{
    public Mario2BombExplodeType mario2BombExplode;
    public Vector3 direct;
    public bool onFirstObtacle;
}

public class Mario2BombExplode : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimationCore;
    [SerializeField] private SkeletonAnimation skeletonAnimationBodyAndHead;
    SkeletonAnimation skeletonAnimation = null;

    public void Init(Mario2BombExplodeParam param)
    {
        //this.bombEffectParam = bombEffectParam;
       
        skeletonAnimationCore.AnimationState.Complete -= OnCompleteAnim;
        skeletonAnimationBodyAndHead.AnimationState.Complete -= OnCompleteAnim;

        if (param.mario2BombExplode == Mario2BombExplodeType.Center)
        {
            skeletonAnimationCore.gameObject.SetActive(true);
            skeletonAnimationBodyAndHead.gameObject.SetActive(false);

            skeletonAnimationCore.loop = true;
            skeletonAnimationCore.AnimationName = "animation";
            skeletonAnimation = skeletonAnimationCore;
        }
        else if(param.mario2BombExplode == Mario2BombExplodeType.Head)
        {
            skeletonAnimationCore.gameObject.SetActive(false);
            skeletonAnimationBodyAndHead.gameObject.SetActive(true);

            skeletonAnimationBodyAndHead.loop = true;
            skeletonAnimationBodyAndHead.AnimationName = "animation_end";
            skeletonAnimation = skeletonAnimationBodyAndHead;
        }
        else if (param.mario2BombExplode == Mario2BombExplodeType.Body)
        {
            skeletonAnimationCore.gameObject.SetActive(false);
            skeletonAnimationBodyAndHead.gameObject.SetActive(true);

            skeletonAnimationBodyAndHead.loop = true;
            skeletonAnimationBodyAndHead.AnimationName = "animation";
            skeletonAnimation = skeletonAnimationBodyAndHead;
        }
        if (param.mario2BombExplode == Mario2BombExplodeType.Head || param.mario2BombExplode == Mario2BombExplodeType.Body)
        {
            if (param.direct == Vector3.up)
                transform.rotation = Quaternion.AngleAxis(-90, Vector3.forward);
            else if (param.direct == Vector3.down)
                transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
            else if (param.direct == Vector3.right)
                transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        }
        var mesh = skeletonAnimation.transform.GetComponent<MeshRenderer>();
        if (param.onFirstObtacle) mesh.sortingOrder = 1;
        else mesh.sortingOrder = -1;

        skeletonAnimation.AnimationState.Complete += OnCompleteAnim;
    }

    private void OnCompleteAnim(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "animation" || trackEntry.Animation.Name == "animation_end")
        {
            skeletonAnimationCore.Skeleton.SetBonesToSetupPose();
            skeletonAnimationBodyAndHead.Skeleton.SetBonesToSetupPose();

            ObjectPool.instance.Disable(gameObject);
        }
    }

    private void LateUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y / 1000;
        transform.position = pos;
    }
}
