using Assets.Scripts.Bullet;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario2Bomb : BoxBase
{
    [SerializeField] private GameObject explodePf;

    private MeshRenderer meshRenderer;
    private bool firstIsObtacle = false;

    public void FirstInit(ItemParam itemParam)
    {
        base.Init();
        diedEnemyAmount = 0;

        meshRenderer = skeletonAnimation.gameObject.GetComponent<MeshRenderer>();

        skeletonAnimation.loop = true;
        skeletonAnimation.AnimationName = "idle";
        skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

        isDie = false;
        state = DynamicEntityState.Idle;
        moveStepCount = 0;
        diedEnemyAmount = 0;
        firstIsObtacle = false;

        var location = MyGraph.instance.GetLocationFromPos(transform.position);
        if (location.tileType == TileType.Obtacle || EntityHelper.IsBox(location))
        {
            firstIsObtacle = true;
            meshRenderer.sortingOrder = 1;

            state = DynamicEntityState.Walk;
            movePointTf.parent = null;
            movePointTf.position = transform.position;

            this.itemParam = itemParam;
            direction = itemParam.direct;
            currentMoveSpeed = itemParam.speed;
        }
        else firstIsObtacle = false;

        StartCoroutine(ChangeAnimIE());

        CanKickFly = true;
        EnemyCanKickFly = true;
    }

    IEnumerator ChangeAnimIE()
    {
        float time = 0f;
        float firstTime = 3f;
        float endTime = 5f;
        while (true)
        {
            if (time <= firstTime)
            {
                if (state == DynamicEntityState.Idle)
                {
                    if (skeletonAnimation.AnimationName != "idle")
                        skeletonAnimation.AnimationName = "idle";
                }
                else if (state == DynamicEntityState.Walk)
                {
                    if (skeletonAnimation.AnimationName != "bounce")
                        skeletonAnimation.AnimationName = "bounce";
                }
            }
            else
            {
                if (state == DynamicEntityState.Idle)
                {
                    if (skeletonAnimation.AnimationName != "idle_warning")
                        skeletonAnimation.AnimationName = "idle_warning";
                }
                else if (state == DynamicEntityState.Walk)
                {
                    if (skeletonAnimation.AnimationName != "bounce_warning")
                        skeletonAnimation.AnimationName = "bounce_warning";
                }
            }

            yield return new WaitForSeconds(0.02f);
            time += 0.02f;
            if (time >= 4)
            {
                AudioManager.instance.PlaySound(SoundName.bomb_bip);
            }
            if (time >= endTime) break;
        }
        //explode
        Debug.Log("end boom:" + skeletonAnimation.loop);
        skeletonAnimation.loop = false;
        skeletonAnimation.AnimationName = "boom";
        AudioManager.instance.StopSound(SoundName.bomb_bip);
        AudioManager.instance.PlaySound(SoundName.bomb_explode);
    }


    public override void Setup(ItemParam itemParam)
    {
        isDie = false;

        this.itemParam = itemParam;

        movePointTf.parent = null;
        movePointTf.position = transform.position;

        direction = itemParam.direct;
        state = DynamicEntityState.Walk;
        currentMoveSpeed = itemParam.speed;
        if (currentMoveSpeed == 0) Die();
        //currentCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
    }

    protected override void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "boom")
        {
            Die();
        }
    }

    protected override void LateUpdate()
    {
        var pos = transform.position;
        pos.z = (pos.y + 0.1f) / 1000;
        transform.position = pos;
    }

    protected override void Update()
    {
        if (isDie) return;

        Move();
        UpdateCellMap();
    }

    protected override void UpdateCellMap()
    {
        if (isDie || GameManager.instance.IsGameOver || firstIsObtacle) return;

        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        if (currentCell != cell)
        {
            MyGraph.instance.UpdateLocation(currentCell, null);
            currentCell = cell;
        }
        MyGraph.instance.UpdateLocation(cell, this);
    }

    protected override void Move()
    {
        if (isDie) return;
        if (state != DynamicEntityState.Walk) return;

        if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        {
            var location = MyGraph.instance.GetLocationFromPos(transform.position);
            if (location.tileType == TileType.CanMove)
            {
                if (firstIsObtacle)
                {
                    firstIsObtacle = false;
                    state = DynamicEntityState.Idle;
                    moveStepCount = 0;
                    meshRenderer.sortingOrder = 0;
                    return;
                }
            }
            var tmpPos = transform.position + directPos[direction];
            CheckLocation(tmpPos);
        }
    }

    protected override void CheckLocation(Vector3 tmpPos)
    {
        var location = MyGraph.instance.GetLocationFromPos(tmpPos);
        if (location == null) return;

        if (location.tileType == TileType.CanMove)
        {
            movePointTf.position = tmpPos;
            moveStepCount++;
        }
        else if (location.tileType == TileType.Dynamic)
        {
            if (firstIsObtacle)
            {
                movePointTf.position = tmpPos;
                moveStepCount++;
            }
            else
            {
                if (EntityHelper.IsBox(location))
                {
                    if (location.entityBase.IsMoving())
                    {
                        movePointTf.position = tmpPos;
                        moveStepCount++;
                    }
                    else if (moveStepCount == 0)
                    {
                        Die();
                    }
                }
                else
                {
                    movePointTf.position = tmpPos;
                    moveStepCount++;
                }
            }
            //Debug.Log(gameObject.name + " location.itemBase.entityType:" + location.itemBase.entityType + " moving:" + location.itemBase.IsMoving());
        }
        else if (location.tileType == TileType.Obtacle)
        {
            if (firstIsObtacle)
            {
                movePointTf.position = tmpPos;
                moveStepCount++;
            }
            else
            {
                if (moveStepCount == 0)
                    Die();
            }
        }
        //Die();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
        if (state == DynamicEntityState.Walk)
        {
            if (!firstIsObtacle && ColliderHelper.HasElastic(col))
            {
                Elastic();
                AudioManager.instance.PlaySound(SoundName.block_bounce);
            }
        }
    }

    private void Elastic()
    {
        direction = opDirects[direction];
        //if (direction == DynamicEntityDirection.Left)
        //    direction = DynamicEntityDirection.Right;
        //else if (direction == DynamicEntityDirection.Right)
        //    direction = DynamicEntityDirection.Left;
        //else if (direction == DynamicEntityDirection.Up)
        //    direction = DynamicEntityDirection.Down;
        //else if (direction == DynamicEntityDirection.Down)
        //    direction = DynamicEntityDirection.Up;
    }

    protected override void Die()
    {
        if (isDie) return;
        isDie = true;

        MyGraph.instance.UpdateLocation(currentCell, null);
        GenBombEffects();
        ObjectPool.instance.Disable(gameObject);
    }

    private void GenBombEffects()
    {
        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        var minX = cell.x - 2;
        var maxX = cell.x + 2;
        var minY = cell.y - 2;
        var maxY = cell.y + 2;
        Mario2BombExplodeParam bombEffectParam = new Mario2BombExplodeParam()
        {
            direct = Vector3.zero,
            mario2BombExplode = Mario2BombExplodeType.Center,
            onFirstObtacle = firstIsObtacle
        };
        Vector3 pos = cell;
        var bgo = ObjectPool.instance.GetGameObject(explodePf, pos + new Vector3(0.5f, 0.5f), Quaternion.identity);
        bgo.GetComponent<Mario2BombExplode>().Init(bombEffectParam);

        for (int x = cell.x - 1; x >= minX; x--)
        {
            pos = new Vector3(x, cell.y);
            if (x == minX) bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Head;
            else bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Body;
            bombEffectParam.direct = Vector3.left;

            if (!Gen()) break;
        }

        for (int x = cell.x + 1; x <= maxX; x++)
        {
            pos = new Vector3(x, cell.y);
            if (x == maxX) bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Head;
            else bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Body;
            bombEffectParam.direct = Vector3.right;

            if (!Gen()) break;
        }

        for (int y = cell.y + 1; y <= maxY; y++)
        {
            pos = new Vector3(cell.x, y);
            if (y == maxY) bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Head;
            else bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Body;
            bombEffectParam.direct = Vector3.up;

            if (!Gen()) break;
        }

        for (int y = cell.y - 1; y >= minY; y--)
        {
            pos = new Vector3(cell.x, y);
            if (y == minY) bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Head;
            else bombEffectParam.mario2BombExplode = Mario2BombExplodeType.Body;

            bombEffectParam.direct = Vector3.down;

            if (!Gen()) break;
        }

        bool Gen()
        {
            MyGraph.instance.GetCellCanPutMario2BombFromPos(pos, out bool allow);
            if (allow || firstIsObtacle)
            {
                bgo = ObjectPool.instance.GetGameObject(explodePf, pos + new Vector3(0.5f, 0.5f), Quaternion.identity);
                bgo.GetComponent<Mario2BombExplode>().Init(bombEffectParam);
            }
            else if (!firstIsObtacle) return false;
            return true;
        }
    }
}
