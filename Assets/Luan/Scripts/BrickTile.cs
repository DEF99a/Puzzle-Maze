using System;
using UnityEngine;

[System.Serializable]
public class BrickTile
{
    public PositionTypeTile PositionPositionTypeTile
    {
        get => positionPositionTypeTile;
        set => positionPositionTypeTile = value;
    }

    public bool BotRight => _botRight;
    public bool Botleft => _botLeft;

    public bool TopRight => _topRight;
    public bool TopLeft => _topLeft;
    public bool Right => _right;
    public bool Left => _left;
    public bool Top => _top;
    public bool Bot => _bot;

    [SerializeField] private bool _botRight;
    [SerializeField] private bool _botLeft;
    [SerializeField] private bool _topLeft;
    [SerializeField] private bool _topRight;
    [SerializeField] private bool _left;
    [SerializeField] private bool _right;
    [SerializeField] private bool _top;
    [SerializeField] private bool _bot;

    [SerializeField] private PositionTypeTile positionPositionTypeTile;
    [SerializeField] private Vector2 _position;
    [SerializeField] private int _x;
    [SerializeField] private int _y;
    [NonSerialized] private bool hasChange;
    public int X => _x;
    public int Y => _y;

    [NonSerialized] public Action<BrickTile> cbUpdateNewPostionType;

    public void UpdateNewPositionType()
    {
        if (hasChange)
        {
            cbUpdateNewPostionType?.Invoke(this);
            hasChange = false;
        }
    }

    public Vector2 Pos => _position;

    public void SetPosition(Vector3 position)
    {
        _position = position;
    }

    public void SetPositionType(int[,] maps, int x, int y, int xMax, int yMax)
    {
        _x = x;
        _y = y;

        TileData.SetPositionType(maps, x, y, xMax, yMax, out PositionTypeTile xx, out bool botRight, out bool botLeft,
            out bool topRight, out bool topLeft, out bool right, out bool left, out bool top, out bool bot);

        positionPositionTypeTile = xx;
        _botRight = botRight;
        _botLeft = botLeft;
        _topLeft = topLeft;
        _topRight = topRight;
        _left = left;
        _right = right;
        _top = top;
        _bot = bot;
    }

    public void UpdatePositionType(int[,] maps, int x, int y, int xMax, int yMax)
    {
        /* _x = x;
        _y = y;*/

        TileData.SetPositionType(maps, x, y, xMax, yMax, out PositionTypeTile xx, out bool botRight, out bool botLeft,
            out bool topRight, out bool topLeft, out bool right, out bool left, out bool top, out bool bot);


        if (positionPositionTypeTile != xx || _botRight != botRight ||
            _botLeft != botLeft || _topLeft != topLeft ||
            _topRight != topRight || _right != right || _left != left || _top != top || _bot != bot)
        {
            hasChange = true;
        }

        positionPositionTypeTile = xx;
        _botRight = botRight;
        _botLeft = botLeft;
        _topLeft = topLeft;
        _topRight = topRight;
        _left = left;
        _right = right;
        _top = top;
        _bot = bot;
        UpdateNewPositionType();
    }
}