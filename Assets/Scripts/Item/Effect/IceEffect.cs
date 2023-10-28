using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEffect : MonoBehaviour
{
    [HideInInspector] public SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.AnimationState.Complete -= HandleAnimationOnComplete;
        skeletonAnimation.AnimationState.Complete += HandleAnimationOnComplete;
        skeletonAnimation.loop = false;
    }

    private void HandleAnimationOnComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "1_start")
            skeletonAnimation.AnimationName = "2_loop";
        else if (trackEntry.Animation.Name == "3_crack")
            ObjectPool.instance.Disable(gameObject);
    }

    public void PlayAnim(int i)
    {
        if (i == 1)
            skeletonAnimation.AnimationName = "1_start";
        else if (i == 2)
            skeletonAnimation.AnimationName = "2_loop";
        else
            skeletonAnimation.AnimationName = "3_crack";
    }
}
