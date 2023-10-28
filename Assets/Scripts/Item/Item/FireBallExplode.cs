using Assets.Scripts.Bullet;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FireBallExplode : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    //[HideInInspector] public BombEffectParam bombEffectParam;

    public void Init(BombEffectParam bombEffectParam)
    {
        //this.bombEffectParam = bombEffectParam;

        skeletonAnimation.loop = false;
        skeletonAnimation.AnimationName = "animation";
        skeletonAnimation.AnimationState.Complete -= OnCompleteAnim;
        skeletonAnimation.AnimationState.Complete += OnCompleteAnim;
        //skeletonAnimation.skeleton.SetColor(Color.red);
        //StartCoroutine(RunToEnd());
    }

    private void OnCompleteAnim(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "animation")
        {
            ObjectPool.instance.Disable(gameObject);
            skeletonAnimation.AnimationName = "idle";
        }
    }

    //IEnumerator RunToEnd()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    ObjectPool.instance.Disable(gameObject);
    //}

    private void LateUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y / 1000;
        transform.position = pos;
    }
}
