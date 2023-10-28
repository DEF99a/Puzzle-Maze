using Assets.Scripts.Bullet;
using Assets.Scripts.Define;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : EntityBase
{
    //public SkeletonAnimation skeletonAnimation;
    //public SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject fireBallExplodePf;

    protected int moveStepCount;
    protected ItemParam itemParam;
    public ItemParam GetItemParam => itemParam;

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "animation")
            ObjectPool.instance.Disable(gameObject);
    }

    public virtual void Setup(ItemParam itemParam)
    {
        isDie = false;

        this.itemParam = itemParam;

        movePointTf.parent = null;
        movePointTf.position = transform.position;

        direction = itemParam.direct;
        state = DynamicEntityState.Walk;
        currentMoveSpeed = itemParam.speed;
        //currentCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
    }

    protected override void Update()
    {
        if (isDie) return;

        Move();
        UpdateCellMap();
    }

    protected override void Move()
    {
        if (isDie) return;
        if (state != DynamicEntityState.Walk) return;

        if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        {
            var tmpPos = transform.position + directPos[direction];
            CheckLocation(tmpPos);
        }
    }

    private void CheckLocation(Vector3 tmpPos)
    {
        var location = MyGraph.instance.GetLocationFromPos(tmpPos);
        if (location == null) return;

        if (location.tileType == TileType.CanMove)
        {
            movePointTf.position = tmpPos;
        }
        else
        {
            Die();
        }
    }

    protected override void Die()
    {
        base.Die();

        state = DynamicEntityState.Idle;

        //skeletonAnimation.loop = false;
        //skeletonAnimation.AnimationName = "animation";

        MyGraph.instance.UpdateLocation(currentCell, null);

        GenBombEffects();
    }

    private void GenBombEffects()
    {
        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        var minX = cell.x - 1;
        var maxX = cell.x + 1;
        var minY = cell.y - 1;
        var maxY = cell.y + 1;
        BombEffectParam bombEffectParam = new BombEffectParam() { characterBase = GetItemParam.characterBase };
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                var pos = new Vector3(x, y);
                MyGraph.instance.GetCellCanPutBombFromPos(pos, out bool allow);
                if (allow)
                {
                    var bgo = ObjectPool.instance.GetGameObject(fireBallExplodePf, pos + new Vector3(0.5f, 0.5f), Quaternion.identity);
                    bgo.GetComponent<FireBallExplode>().Init(bombEffectParam);
                }
            }
        }
        ObjectPool.instance.Disable(gameObject);
    }
}
