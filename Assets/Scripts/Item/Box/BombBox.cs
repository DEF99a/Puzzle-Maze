using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBox : BoxBase
{
    [SerializeField] private GameObject bombEffectPf;

    protected override void CheckLocation(Vector3 tmpPos)
    {
        var location = MyGraph.instance.GetLocationFromPos(tmpPos);
        if (location == null) return;
        if (MapManager.instance.IsInOpenDoor(tmpPos))
        {
            if (moveStepCount > 0)
            {
                moveStepCount = 0;
                state = DynamicEntityState.Idle;
            }
            else if (CanBreak || (moveStepCount > 0))
                Die();
            return;
        }

        if (location.tileType == TileType.CanMove)
        {
            movePointTf.position = tmpPos;
            moveStepCount++;
        }
        else if (location.tileType == TileType.Dynamic)
        {
            //Debug.Log(gameObject.name + " location.itemBase.entityType:" + location.itemBase.entityType + " moving:" + location.itemBase.IsMoving());
            if (EntityHelper.IsBox(location) || EntityHelper.IsBoxKey(location))
            {
                if (location.entityBase.IsMoving())
                {
                    movePointTf.position = tmpPos;
                    moveStepCount++;
                }
                else
                {
                    if (CanBreak || (moveStepCount > 0))
                        Die();
                }
            }
            else
            {
                movePointTf.position = tmpPos;
                moveStepCount++;
            }
        }
        else if (location.tileType == TileType.Obtacle)
        {
            if (CanBreak || (moveStepCount > 0))
                Die();
        }
        if (!isDie && state == DynamicEntityState.Idle)
            AudioManager.instance.PlaySound(SoundName.block_collision);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDie) return;

        if (col.CompareTag("RedImpostorCollider"))
        {
            Die();
        }
        else if (state == DynamicEntityState.Walk)
        {
            if (ColliderHelper.HasElastic(col))
            {
                direction = opDirects[direction];
                AudioManager.instance.PlaySound(SoundName.block_bounce);
            }
        }
        else if (col.CompareTag("BombEffect"))
        {
            itemParam = new ItemParam()
            {
                characterBase = col.GetComponent<BombEffect>().bombEffectParam.characterBase
            };

            Die();
        }
    }

    // explode
    protected override void Die()
    {
        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }

        if (isDie) return;
        if (GetItemParam.characterBase?.entityType == EntityType.Player)
            GameManager.instance.UpdateGameCoin(transform.position, Config.CoinRemoveBomBox, CoinFrom.RemoveBox);

        base.Die();
        GenBombEffects();
    }

    private void GenBombEffects()
    {
        AudioManager.instance.PlaySound(SoundName.bomb_explode);

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
                var pos = new Vector3(x + 0.5f, y + 0.5f);
                MyGraph.instance.GetCellCanPutBombFromPos(pos, out bool allow);
                //Debug.Log("pos:" + pos + " allow:" + allow);

                if (allow)
                {
                    var bgo = ObjectPool.instance.GetGameObject(bombEffectPf, pos, Quaternion.identity);
                    bgo.GetComponent<BombEffect>().Init(bombEffectParam);
                }
            }
        }
        ObjectPool.instance.Disable(gameObject);
    }
}
