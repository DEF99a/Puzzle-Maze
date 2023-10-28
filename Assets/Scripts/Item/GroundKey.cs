using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundKey : BoxBase
{
    [SerializeField] private Sprite unlock;
    [SerializeField] private Sprite lockSp;
    public override void BoxInit(BoxType boxType, SkeletonDataAsset skeletonDataAsset, Sprite sprite, bool _canBorn, bool _canBreak, bool _canBeKicked,
        bool _canBurn, bool _enemyCanFly, int _moveAmount)
    {
        base.BoxInit(boxType, skeletonDataAsset, sprite, _canBorn, _canBreak, _canBeKicked, _canBurn, _enemyCanFly, _moveAmount);
        spriteRenderer.sprite = lockSp;
    }

    protected override void Update()
    {
    }

    //[SerializeField] private Sprite unlock;

    //private bool hasBox;

    //void OnTriggerEnter2D(Collider2D col)
    //{
    //    if (hasBox) return;

    //    Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
    //    if (col.CompareTag("Box"))
    //    {
    //        var bb = col.GetComponent<BoxBase>();
    //        if(bb is BoxKey || bb is BoxKey1)
    //        {
    //            MapManager.instance.KeyMatchAmount++;
    //            hasBox = true;
    //            if(MapManager.instance.KeyMatchAmount == MapManager.instance.TotalKey)
    //            {
    //                GameManager.instance.GameOver(true, GameEndReason.WinPuzzle);
    //                StartCoroutine(ChangeStage());
    //            }    
    //        }    
    //    }
    //}

    //void OnTriggerExit2D(Collider2D col)
    //{
    //    //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
    //    if (!hasBox) return;

    //    if (col.CompareTag("Box"))
    //    {
    //        var bb = col.GetComponent<BoxBase>();
    //        if (bb is BoxKey || bb is BoxKey1)
    //        {
    //            hasBox = false;
    //            MapManager.instance.KeyMatchAmount--;
    //        }
    //    }
    //}    
}
