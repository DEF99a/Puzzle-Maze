using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    public static int worldId;

    public static Dictionary<DynamicEntityDirection, Vector3> directPos = new Dictionary<DynamicEntityDirection, Vector3>()
    {
        {DynamicEntityDirection.Down, Vector3.down }, {DynamicEntityDirection.Up, Vector3.up},
        {DynamicEntityDirection.Left, Vector3.left }, {DynamicEntityDirection.Right, Vector3.right},
        {DynamicEntityDirection.None, Vector3.zero },
    };
    protected static Dictionary<DynamicEntityDirection, DynamicEntityDirection> opDirects = new Dictionary<DynamicEntityDirection, DynamicEntityDirection>()
    {
        {DynamicEntityDirection.Down, DynamicEntityDirection.Up }, {DynamicEntityDirection.Up, DynamicEntityDirection.Down},
        {DynamicEntityDirection.Left, DynamicEntityDirection.Right }, {DynamicEntityDirection.Right, DynamicEntityDirection.Left},
        {DynamicEntityDirection.None, DynamicEntityDirection.None },
    };

    public static Dictionary<Vector3, DynamicEntityDirection> posDirects = new Dictionary<Vector3, DynamicEntityDirection>()
    {
        {Vector3.up, DynamicEntityDirection.Up }, {Vector3.down, DynamicEntityDirection.Down},
        {Vector3.left, DynamicEntityDirection.Left }, {Vector3.right, DynamicEntityDirection.Right},
        {Vector3.zero, DynamicEntityDirection.None }
    };

    public Transform movePointTf;
    protected Vector3 oldMovePointPos;

    public EntityType entityType;

    [HideInInspector] public int EntityId;
    [HideInInspector] public DynamicEntityDirection direction;

    protected DynamicEntityState state;

    protected float currentMoveSpeed;
    protected Vector3Int currentCell;
    protected bool isDie;

    [HideInInspector] public bool HasEnemyAbove = false;

    public virtual bool IsLive() { return isDie == false; }

    protected virtual void Awake()
    {
    }

    protected virtual void OnDisable()
    {
        EntityId = 0;
    }

    public virtual void Init()
    {
        EntityId = ++worldId;
        //Debug.Log("EntityId:" + EntityId);

        var pos = transform.position;
        if (pos.x != 0) pos.x = (int)pos.x + (pos.x < 0 ? -0.5f : 0.5f);
        if (pos.y != 0) pos.y = (int)pos.y + (pos.y < 0 ? -0.5f : 0.5f);
        transform.position = pos;
        movePointTf.transform.position = pos;

        currentCell = GeneralHelper.DefaultVector3Int;
    }

    protected virtual void Update()
    {
        //if (isDie || GameManager.instance.IsGameOver)
        if(isDie)
        {
            state = DynamicEntityState.Idle;
            return;
        }

        Move();
        UpdateCellMap();
    }

    protected virtual void LateUpdate()
    {
        var pos = transform.position;
        pos.z = pos.y / 1000;
        transform.position = pos;
    }

    private void FixedUpdate()
    {
        //if (isDie || GameManager.instance.IsGameOver || state == DynamicEntityState.Ice) return;
        if (isDie || state == DynamicEntityState.Ice) return;
        transform.position = Vector3.MoveTowards(transform.position, movePointTf.position, currentMoveSpeed * Time.fixedDeltaTime);
    }

    protected virtual void UpdateCellMap()
    {
        //if (isDie || GameManager.instance.IsGameOver) return;
        if (isDie) return;

        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        if (currentCell != cell)
        {
            MyGraph.instance.UpdateLocation(currentCell, null);
            currentCell = cell;
        }
        MyGraph.instance.UpdateLocation(cell, this);
    }

    protected virtual void Die()
    {
        isDie = true;
        movePointTf.parent = transform;
        movePointTf.position = transform.position;
    }

    public void ResetMovePoint()
    {
        movePointTf.parent = transform;
        movePointTf.position = transform.position;
        state = DynamicEntityState.Idle;
    }

    protected virtual void Move()
    {
    }

    protected virtual void MoveLeft()
    {
    }

    protected virtual void MoveDown()
    {
    }

    protected virtual void MoveRight()
    {
    }

    protected virtual void MoveUp()
    {
    }

    protected virtual void MoveStop()
    {
        state = DynamicEntityState.Idle;
    }

    public bool IsMoving()
    {
        return state == DynamicEntityState.Walk;
    }
}
