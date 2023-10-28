using Assets.Scripts.Bullet;
using Assets.Scripts.Define;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioBullet : MonoBehaviour
{
    [SerializeField] private GameObject fireBallExplodePf;
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    private bool isDie;

    protected ItemParam itemParam;
    private DynamicEntityDirection direction;
    private float currentMoveSpeed;

    public ItemParam GetItemParam => itemParam;

    private MeshRenderer meshRenderer;

    public virtual void Setup(ItemParam itemParam)
    {
        isDie = false;

        this.itemParam = itemParam;

        direction = itemParam.direct;
        currentMoveSpeed = itemParam.speed;
        //currentCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
        if (direction == DynamicEntityDirection.Up) meshRenderer.sortingOrder = -1;
        else meshRenderer.sortingOrder = 0;
    }

    private void Update()
    {
        if (isDie) return;

        transform.position += EntityBase.directPos[direction] * currentMoveSpeed * Time.deltaTime;
    }

    void Die()
    {
        if (isDie) return;
        isDie = true;
        GenBombEffects();
    }

    private void GenBombEffects()
    {
        BombEffectParam bombEffectParam = new BombEffectParam() { characterBase = GetItemParam.characterBase };
        var bgo = ObjectPool.instance.GetGameObject(fireBallExplodePf, transform.position, Quaternion.identity);
        bgo.GetComponent<FireBallExplode>().Init(bombEffectParam);
        ObjectPool.instance.Disable(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("target:" + collision.gameObject.name + " collision:" + collision.tag);
        if (collision.CompareTag("Player")) return;
        else if (collision.CompareTag("BongBong"))
        {
            PlayerController.instance.ProcessEatItem(collision.gameObject, 1);
        }
        AudioManager.instance.PlaySound(SoundName.skill_mario_hit);
        Die();
    }
}
