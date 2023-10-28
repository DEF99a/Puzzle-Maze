using Assets.Scripts.Bullet;
using Assets.Scripts.Define;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedImpostorKnife : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private GameObject box2D;

    private Vector3 originLocalScale;

    private void Awake()
    {
        originLocalScale = transform.localScale;
        box2D.gameObject.SetActive(false);        
    }

    private void Start()
    {
        skeletonAnimation.AnimationState.Complete -= HandleCompleteAnim;
        skeletonAnimation.AnimationState.Complete += HandleCompleteAnim;
    }

    public virtual void Setup(DynamicEntityDirection direction, Vector3 locationPos)
    {
        //skeletonAnimation.skeleton.SetBonesToSetupPose();

        skeletonAnimation.loop = true;
        if (direction == DynamicEntityDirection.Up)
        {
            skeletonAnimation.AnimationName = "rear";
            //skeletonAnimation.AnimationState.SetAnimation(0, "rear", false);
        }
        else if (direction == DynamicEntityDirection.Down)
        {
            skeletonAnimation.AnimationName = "front";
            //skeletonAnimation.AnimationState.SetAnimation(0, "front", false);
        }
        else if (direction == DynamicEntityDirection.Left)
        {
            skeletonAnimation.AnimationName = "side";
            //skeletonAnimation.AnimationState.SetAnimation(0, "side", false);
            transform.localScale = originLocalScale;
        }
        else if (direction == DynamicEntityDirection.Right)
        {
            skeletonAnimation.AnimationName = "side";
            //skeletonAnimation.AnimationState.SetAnimation(0, "side", false);

            var temp = originLocalScale;
            temp.x = -temp.x;
            transform.localScale = temp;
        }
        box2D.transform.parent = null;
        box2D.transform.position = locationPos;
        box2D.gameObject.SetActive(true);
    }

    private void HandleCompleteAnim(TrackEntry trackEntry)
    {
        if(trackEntry.Animation.Name == "rear" || trackEntry.Animation.Name == "front" || trackEntry.Animation.Name == "side")
        {
            transform.rotation = Quaternion.identity;
            transform.localScale = originLocalScale;

            box2D.transform.parent = transform;

            //skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
            //skeletonAnimation.skeleton.SetBonesToSetupPose();

            ObjectPool.instance.Disable(gameObject);
            box2D.gameObject.SetActive(false);
        }        
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("target:" + collision.gameObject.name + " collision:" + collision.tag);
    //    if (collision.CompareTag("Player")) return;
    //    else if (collision.CompareTag("BongBong"))
    //    {
    //        PlayerController.instance.ProcessEatItem(collision.gameObject, 1);
    //    }
    //    Die();
    //}
}
