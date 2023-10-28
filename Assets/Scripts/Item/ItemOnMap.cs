using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;

public class ItemOnMap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SkeletonAnimation anim;

    [SerializeField] private Text valueTxt;
    [HideInInspector] public EntityType entityType;

    public void Init(EntityType entityType)
    {
        this.entityType = entityType;

        string name = entityType.ToString();
        SkeletonDataAsset skeletonDataAsset;

        Debug.Log("entityttttt:" + entityType);

        if (name.Contains("ItemCoinRewardEndGameX"))
        {
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemCoinRewardEndGameX);
        }
        else if (name.Contains("ItemCoin"))
        {
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemCoin100);
        }
        else if (name.Contains("ItemTimer"))
        {
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(EntityType.ItemTimer);
        }
        else
        {
            skeletonDataAsset = EntityAnimData.instance.GetAnimData(entityType);
        }

        if (skeletonDataAsset != null)
        {
            spriteRenderer.gameObject.SetActive(false);

            anim.ClearState();

            anim.skeletonDataAsset = skeletonDataAsset;
            anim.Initialize(true);
            anim.gameObject.SetActive(true);

            if (name.Contains("ItemCoin"))
            {
                anim.timeScale = 0.2f;
            }

            var animName = anim.skeleton.Data.FindAnimation("idle");
            if (animName != null)
                anim.AnimationName = "idle";
            else anim.AnimationName = "animation";
        }
        else
        {
            anim.gameObject.SetActive(false);
            spriteRenderer.gameObject.SetActive(true);
        }

        if (name.Contains("ItemCoinRewardEndGameX"))
        {
            spriteRenderer.sprite = EntitySprites.instance.GetSprite(EntityType.ItemCoinRewardEndGameX);
            valueTxt.text = name.Replace("ItemCoinRewardEndGameX", "");
        }
        else if (name.Contains("ItemCoin"))
        {
            spriteRenderer.sprite = EntitySprites.instance.GetSprite(EntityType.ItemCoin);
            int value = int.Parse(name.Replace("ItemCoin", ""));
            valueTxt.text = (value / 10).ToString();
        }
        else if (name.Contains("ItemTimer"))
        {
            spriteRenderer.sprite = EntitySprites.instance.GetSprite(EntityType.ItemTimer);
            valueTxt.text = name.Replace("ItemTimer", "");
        }
        else
        {
            spriteRenderer.sprite = EntitySprites.instance.GetSprite(entityType);
            valueTxt.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //col.CompareTag("Enemy") ||
        if (col.CompareTag("Player")
            || col.CompareTag("Box") || col.CompareTag("SongokuHook") || col.CompareTag("Hook") || col.CompareTag("RedImpostorCollider"))
        {
            if (col.CompareTag("Box"))
            {
                var boxBase = col.GetComponent<BoxBase>();
                Debug.Log("boxBase.IsMoving():" + boxBase.IsMoving() + " boxBase.GetItemParam.characterBase:" + (boxBase.GetItemParam.characterBase != null));
                if (boxBase != null)
                {
                    if (boxBase.IsMoving())
                    {
                        var characterBase = boxBase.GetItemParam.characterBase;
                        if (characterBase != null && characterBase.gameObject.activeInHierarchy
                            && characterBase.EntityId > 0 && boxBase.GetItemParam.characterId == characterBase.EntityId)
                        {
                            characterBase.ProcessEatItem(this.gameObject, 2);
                            ObjectPool.instance.Disable(gameObject);
                        }
                    }
                    else if (boxBase is Mario2Bomb)
                    {
                        var characterBase = boxBase.GetItemParam.characterBase;
                        if (characterBase == null && PlayerController.instance.gameObject.activeInHierarchy)
                        {
                            PlayerController.instance.ProcessEatItem(this.gameObject, 2);
                            ObjectPool.instance.Disable(gameObject);
                        }
                    }
                }
            }
            else if (col.CompareTag("SongokuHook") || col.CompareTag("Hook") || col.CompareTag("RedImpostorCollider"))
            {
                if (PlayerController.instance.IsLive())
                {
                    PlayerController.instance.ProcessEatItem(this.gameObject, 2);
                    ObjectPool.instance.Disable(gameObject);
                }
            }
            else
            {
                ObjectPool.instance.Disable(gameObject);
            }
        }
    }
}
