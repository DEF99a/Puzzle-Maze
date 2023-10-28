using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItem : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private SkeletonDataAsset trapSkeletonDataAsset;
    [SerializeField] private SkeletonDataAsset explodeDataAsset;

    private BoxCollider2D box2D;
    private MeshRenderer meshRenderer;

    private bool isDie;
    public bool fromBallBox { get; private set; }

    private void Awake()
    {
        box2D = GetComponent<BoxCollider2D>();
        meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
    }

    public void Setup(bool _fromBallBox)
    {
        isDie = false;

        box2D.enabled = false;

        meshRenderer.sortingLayerName = "UI";
        meshRenderer.sortingOrder = 0;

        skeletonAnimation.skeletonDataAsset = trapSkeletonDataAsset;

        skeletonAnimation.Initialize(true);
        skeletonAnimation.loop = false;
        skeletonAnimation.AnimationName = "start";
        skeletonAnimation.AnimationState.Complete -= AnimComplete;
        skeletonAnimation.AnimationState.Complete += AnimComplete;

        fromBallBox = _fromBallBox;

        gameObject.SetActive(true);
    }

    private void AnimComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "start")
        {
            meshRenderer.sortingLayerName = "Player";
            meshRenderer.sortingOrder = -2;

            box2D.enabled = true;
            skeletonAnimation.loop = true;
            skeletonAnimation.AnimationName = "loop";

            StartCoroutine(TimeLifeIE());
        }
        else if (trackEntry.Animation.Name == "animation")
        {
            Debug.Log("Hide");
            ObjectPool.instance.Disable(gameObject);
        }
    }

    IEnumerator TimeLifeIE()
    {
        Debug.Log("TimeLifeIE");
        float time = 0f;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            time += 1f;
            if (time >= 4) AudioManager.instance.PlaySound(SoundName.bomb_bip);
            if (time >= 5) break;
        }
        Debug.Log("TimeLifeIE 2");
        AudioManager.instance.StopSound(SoundName.bomb_bip);
        AudioManager.instance.PlaySound(SoundName.bomb_explode);
        Explode();
    }

    private void LateUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y / 1000;
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDie) return;

        if (collision.CompareTag("Player") || (fromBallBox && collision.CompareTag("Enemy")))
        {
            Explode();
        }
    }

    private void Explode()
    {
        isDie = true;

        skeletonAnimation.skeletonDataAsset = explodeDataAsset;
        skeletonAnimation.Initialize(true);
        skeletonAnimation.loop = false;
        skeletonAnimation.AnimationName = "animation";
        skeletonAnimation.AnimationState.Complete -= AnimComplete;
        skeletonAnimation.AnimationState.Complete += AnimComplete;
    }
}
