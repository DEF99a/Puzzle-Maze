using Assets.Scripts.Define;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Balloon : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation itemAnim;
    [SerializeField] private Transform movePointTf;
    [SerializeField] private float speed = 1;
    [SerializeField] private Text valueTxt;

    public EntityType entityType;

    private bool isDie = true;

    public void Init(Vector3 position, EntityType entityType)
    {
        var pos = position;
        if (pos.x != 0) pos.x = (int)pos.x + (pos.x < 0 ? -0.5f : 0.5f);
        if (pos.y != 0) pos.y = (int)pos.y + (pos.y < 0 ? -0.5f : 0.5f);
        transform.position = pos;
        movePointTf.transform.position = pos;
        isDie = false;
        this.entityType = entityType;

        valueTxt.gameObject.SetActive(false);
        string name = entityType.ToString();
        SkeletonDataAsset skeletonDataAsset;
        string animationName = "idle";

        //Debug.Log("Balloon:" + name);
        if (name.Contains("ItemCoinRewardEndGameX"))
        {
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemCoinRewardEndGameX);
            animationName += " x" + name.Replace("ItemCoinRewardEndGameX", "");
            //Debug.Log("Balloon skeletonDataAsset 1:" + skeletonDataAsset);
        }
        else if (name.Contains("ItemCoin"))
        {
            //if (name == "ItemCoin100")
            //{
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemCoin100);
            animationName = "";
            //Debug.Log("Balloon skeletonDataAsset 2:" + skeletonDataAsset);
            int amount = int.Parse(name.Replace("ItemCoin", "")) / 10;
            valueTxt.text = amount.ToString();
            valueTxt.gameObject.SetActive(true);
            //}
            //else
            //{
            //    skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemCoin);
            //    animationName += " " + name.Replace("ItemCoin", "");
            //    Debug.Log("Balloon skeletonDataAsset 2:" + skeletonDataAsset);
            //}
        }
        else if (name.Contains("ItemTimer"))
        {
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemTimer);
            valueTxt.text = name.Replace("ItemTimer", "");
            valueTxt.gameObject.SetActive(true);
            //Debug.Log("Balloon skeletonDataAsset 3:" + skeletonDataAsset);
        }
        else
        {
            animationName = "";
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(entityType);
            //Debug.Log("Balloon skeletonDataAsset 4:" + skeletonDataAsset);
        }

        if (skeletonDataAsset != null)
        {
            itemAnim.ClearState();

            itemAnim.skeletonDataAsset = skeletonDataAsset;
            itemAnim.Initialize(true);

            if (animationName == "")
            {
                var anim = itemAnim.skeleton.Data.FindAnimation("idle");
                if (anim != null)
                    itemAnim.AnimationName = "idle";
                else itemAnim.AnimationName = "animation";
            }
            else itemAnim.AnimationName = animationName;
        }
    }

    void Update()
    {
        if (isDie) return;

        if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        {
            var tmpPos = transform.position + Vector3.up;
            movePointTf.position = tmpPos;
        }

        if (transform.position.y >= CameraController.instance.max.y + 1)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        //if (isDie || GameManager.instance.IsGameOver) return;
        if (isDie) return;
        transform.position = Vector3.MoveTowards(transform.position, movePointTf.position, speed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y / 1000;
        transform.position = pos;
    }

    protected virtual void Die()
    {
        if (isDie) return;

        isDie = true;
        movePointTf.parent = transform;
        movePointTf.position = transform.position;

        ObjectPool.instance.Disable(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //col.CompareTag("Enemy") ||
        if ( col.CompareTag("Player")
            || col.CompareTag("Box") || col.CompareTag("Hook") || col.CompareTag("MarioBullet") || col.CompareTag("SongokuHook")
            || col.CompareTag("RedImpostorCollider") )
        {
            //Debug.Log("col:" + col.tag);

            if (col.CompareTag("Box"))
            {
                var boxBase = col.GetComponent<BoxBase>();
                //Debug.Log("boxxx:" + (boxBase != null));

                if (boxBase != null && boxBase.IsMoving() && boxBase.GetItemParam.characterBase != null)
                {
                    var characterBase = boxBase.GetItemParam.characterBase;
                    if (characterBase != null && characterBase.gameObject.activeInHierarchy
                        && characterBase.EntityId > 0 && boxBase.GetItemParam.characterId == characterBase.EntityId)
                    {
                        //Debug.Log("box die");
                        characterBase.ProcessEatItem(this.gameObject, 1);
                        AudioManager.instance.PlaySound(SoundName.pick_ballon);
                        Die();
                        return;
                    }
                }
            }
            else if (col.CompareTag("RedImpostorCollider"))
            {
                //Debug.Log("RedImpostorCollider");
                PlayerController.instance.ProcessEatItem(this.gameObject, 1);
                AudioManager.instance.PlaySound(SoundName.pick_ballon);
                Die();
            }
            else {
                //Debug.Log("box die 2");
                AudioManager.instance.PlaySound(SoundName.pick_ballon);
                Die();
            }
        }
    }
}
