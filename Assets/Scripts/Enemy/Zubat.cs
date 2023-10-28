using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zubat : EnemyController
{
    protected override void CanMove(Vector3 tmpPos)
    {
        var cell = MyGraph.instance.GetCellEnemyCanMove2(tmpPos, out bool canMove);
        if (canMove)
        {
            var enemies = MapManager.instance.enemies;
            foreach (var enemy in enemies)
            {
                if (enemy.GetInstanceID() != this.GetInstanceID())
                {
                    var otherCell = MyGraph.instance.GetCellEnemyCanMove2(enemy.movePointTf.position, out _);
                    if( (cell.x == otherCell.x && Mathf.Abs(cell.y - otherCell.y) <= 0)
                        || (cell.y == otherCell.y && Mathf.Abs(cell.x - otherCell.x) <= 0))
                    {
                        direction = NextDirect(reverseSide: false, out bool hasCellEmpty);
                        Idle(hasCellEmpty ? -1 : 0);
                        return;
                    }
                }
            }

            var location = MyGraph.instance.GetLocationFromPos(tmpPos);
            if (EntityHelper.IsPlayer(location))
                chasePlayerTimer = Time.time + Random.Range(3, 5);

            movePointTf.position = tmpPos;
            state = DynamicEntityState.Walk;
        }
    }

    protected override void UpdateCellMap()
    {
        if (isDie || GameManager.instance.IsGameOver) return;

        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out bool canMove);
        if (canMove)
        {
            //Debug.Log("currentCell 1:" + currentCell);
            if (currentCell != cell)
            {
                var location = MyGraph.instance.GetLocationFromPos(currentCell);
                if (location.entityBase == this || location.tileType == TileType.CanMove)
                {
                    MyGraph.instance.UpdateLocation(currentCell, null);
                    //Debug.Log("currentCell 2:" + currentCell);
                }                
                currentCell = cell;
            }

            MyGraph.instance.UpdateLocation(cell, this);
        }
    }

    //protected override DynamicEntityDirection NextDirect(bool reverseSide, out bool hasCellEmpty)
    //{
        //hasCellEmpty = false;
        //return base.NextDirect(reverseSide, out hasCellEmpty);
        //Up,Right,Down,Left
        //float[] directChange = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };
        //if (direction == DynamicEntityDirection.Up)
        //{
        //    directChange = new float[] { 0f, 0.4f, 0.2f, 0.4f };
        //}
        //else if (direction == DynamicEntityDirection.Down)
        //{
        //    directChange = new float[] { 0.2f, 0.4f, 0, 0.4f };
        //}
        //else if (direction == DynamicEntityDirection.Right)
        //{
        //    directChange = new float[] { 0.4f, 0, 0.4f, 0.2f };
        //}
        //else if (direction == DynamicEntityDirection.Left)
        //{
        //    directChange = new float[] { 0.4f, 0.2f, 0.4f, 0 };
        //}
        //var newDirect = (DynamicEntityDirection)RandomElement(directChange);
        //return newDirect;
    //}

}
