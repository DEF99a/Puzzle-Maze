using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    CanMove,
    Obtacle,
    Dynamic
}

public class Location
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public Location Parent;

    public TileType tileType;
    public EntityBase entityBase;

    //public void AddEntity(EntityBase entityBase)
    //{
    //    if (entityBases == null) entityBases = new List<EntityBase>();
    //    entityBases.Add(entityBase);
    //}

    //public void RemoveEntity(EntityBase entityBase)
    //{
    //    if (entityBases == null) return;
    //    entityBases.Remove(entityBase);
    //}

    public Vector3 ToVector3()
    {
        return new Vector3(X + 0.5f, Y + 0.5f);
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(X, Y, 0);
    }

    public Location Clone()
    {
        return new Location() { X = this.X, Y = this.Y, tileType = this.tileType, entityBase = this.entityBase };
    }
}

public class MyGraph : MonoBehaviour
{
    public static MyGraph instance;

    [SerializeField] private bool showLog;

    public static List<String> enemieStrs = new List<string>()
        {
            "goomba"
        };

    private Dictionary<Vector3Int, Location> map = new Dictionary<Vector3Int, Location>();
    private Dictionary<Vector3Int, Location> cacheMap = new Dictionary<Vector3Int, Location>();
    private Dictionary<Vector3Int, Location> tmpMap = new Dictionary<Vector3Int, Location>();

    private Tilemap backgroundTileMap;
    private List<Tilemap> frontTileMaps;

    [HideInInspector] public List<Location> doors = new List<Location>();
    public List<Location> cacheDoors = new List<Location>();

    public List<Location> autoMoveLocations = new List<Location>();

    private void Awake()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        var bgTf = transform.Find("Background");
        backgroundTileMap = bgTf.GetComponent<Tilemap>();

        frontTileMaps = new List<Tilemap>();
        for (int k = 0; k < transform.childCount; k++)
        {
            var child = transform.GetChild(k);
            if (child.name.Contains("Front"))
            {
                frontTileMaps.Add(child.GetComponent<Tilemap>());
            }
        }
        UpdateMap();
    }

    private void UpdateMap()
    {
        foreach (var pos in backgroundTileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (backgroundTileMap.HasTile(localPlace))
            {
                var location = new Location()
                {
                    X = localPlace.x,
                    Y = localPlace.y,
                    tileType = TileType.CanMove
                };
                map.Add(localPlace, location);
                //Debug.Log("background:" + localPlace);
            }
        }

        foreach (var frontTileMap in frontTileMaps)
        {
            foreach (var pos in frontTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (frontTileMap.HasTile(localPlace))
                {
                    var tile = frontTileMap.GetTile(localPlace);
                    if (tile.name.Contains("ground lock"))
                    {
                        //Debug.Log("tile.nameee:" + tile.name);
                        continue;
                    }

                    if (!enemieStrs.Contains(tile.name) && !tile.name.Equals("player") && !tile.name.Contains("box"))
                    {
                        map[localPlace].tileType = TileType.Obtacle;
                    }
                    else if (map[localPlace].tileType == TileType.CanMove)
                    {
                        map[localPlace].tileType = TileType.Dynamic;
                    }
                    //if (tile.name.Contains("ground_set"))
                    //    Debug.Log("tile:" + tile.name);
                }
            }
        }


        cacheMap = map;
    }

    public void ResetCurrentPartMap(Bound bound)
    {
        var cacheDoorsVector3Int = doors.Select(a => a.ToVector3Int());

        foreach (var pos in backgroundTileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (backgroundTileMap.HasTile(localPlace) && bound.InBound(localPlace) && !cacheDoorsVector3Int.Contains(localPlace))
            {
                if (showLog)
                    Debug.Log("ResetCurrentPartMap:" + localPlace);
                if (map.ContainsKey(localPlace))
                {
                    map[localPlace].tileType = TileType.CanMove;
                    map[localPlace].entityBase = null;
                }
                else
                {
                    Debug.LogError("ResetCurrentPartMap error:" + localPlace);
                }
            }
        }
        foreach (var frontTileMap in frontTileMaps)
        {
            foreach (var pos in frontTileMap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (frontTileMap.HasTile(localPlace) && bound.InBound(localPlace) && !cacheDoorsVector3Int.Contains(localPlace))
                {
                    var tile = frontTileMap.GetTile(localPlace);
                    if (tile.name.Contains("ground lock"))
                    {
                        continue;
                    }

                    if (!enemieStrs.Contains(tile.name) && !tile.name.Equals("player") && !tile.name.Contains("box"))
                    {
                        if (showLog)
                            Debug.Log("ResetCurrentPartMap frontTileMap:" + localPlace);
                        map[localPlace].tileType = TileType.Obtacle;
                    }
                    else if (map[localPlace].tileType == TileType.CanMove)
                    {
                        map[localPlace].tileType = TileType.Dynamic;
                    }
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //var enemies = GameManager.instance.enemies;
        //foreach (var en in enemies)
        //{
        //    var minX = Mathf.FloorToInt(en.fieldOfView.bound.Min(a => a.x));
        //    var maxX = Mathf.CeilToInt(en.fieldOfView.bound.Max(a => a.x));
        //    var minY = Mathf.FloorToInt(en.fieldOfView.bound.Min(a => a.y));
        //    var maxY = Mathf.CeilToInt(en.fieldOfView.bound.Max(a => a.y));

        //    for (int k = minX; k < maxX; k++)
        //    {
        //        for (int l = minY; l < maxY; l++)
        //        {
        //            Gizmos.DrawLine(new Vector3(k, l), new Vector3(k, l + 1));
        //            Gizmos.DrawLine(new Vector3(k, l+1), new Vector3(k+1, l + 1));
        //            Gizmos.DrawLine(new Vector3(k+1, l+1), new Vector3(k+1, l));
        //            Gizmos.DrawLine(new Vector3(k+1, l), new Vector3(k, l));
        //        }
        //    }
        //}

        Gizmos.color = Color.blue;
        foreach (var en in map)
        {
            if (en.Value.tileType != TileType.CanMove) continue;

            int k = en.Key.x;
            int l = en.Key.y;

            Gizmos.DrawLine(new Vector3(k, l), new Vector3(k, l + 1));
            Gizmos.DrawLine(new Vector3(k, l + 1), new Vector3(k + 1, l + 1));
            Gizmos.DrawLine(new Vector3(k + 1, l + 1), new Vector3(k + 1, l));
            Gizmos.DrawLine(new Vector3(k + 1, l), new Vector3(k, l));
        }
    }

    public List<Location> Search(Vector3 startPos, Vector3 endPos, bool useCacheMap, bool through = false)
    {
        Vector3Int startVt = backgroundTileMap.WorldToCell(startPos);
        var targetVt = backgroundTileMap.WorldToCell(endPos);
        //Debug.Log("startVt:" + startVt + " targetVt:" + targetVt);

        //if (obtacleTileMap.HasTile(targetVt))
        //{
        //    var tmp = targetVt;
        //    int i = 1;
        //    do
        //    {
        //        var left = new Vector3Int(tmp.x - i, tmp.y, 0);
        //        var right = new Vector3Int(tmp.x + i, tmp.y, 0);
        //        var up = new Vector3Int(tmp.x, tmp.y + i, 0);
        //        var down = new Vector3Int(tmp.x, tmp.y - i, 0);
        //        if (CanMoveTo(left)) tmp = left;
        //        else if (CanMoveTo(right)) tmp = right;
        //        else if (CanMoveTo(up)) tmp = up;
        //        else if (CanMoveTo(down)) tmp = down;
        //        i++;
        //    }
        //    while (tmp == targetVt && i < 10000);
        //    targetVt = tmp;
        //}
        //bool CanMoveTo(Vector3Int vector3Int)
        //{
        //    if (backgroundTileMap.HasTile(vector3Int) && !obtacleTileMap.HasTile(vector3Int)) return true;
        //    return false;
        //}

        return AStar(startVt, targetVt, useCacheMap, through);
    }

    private List<Location> AStar(Vector3Int startVt, Vector3Int targetVt, bool useCacheMap, bool through)
    {
        var result = new List<Location>();

        Dictionary<Vector3Int, Location> _map = new Dictionary<Vector3Int, Location>();
        if (useCacheMap)
        {
            foreach (var dt in cacheMap)
            {
                _map.Add(dt.Key, dt.Value.Clone());
            }
        }
        else
        {
            foreach (var dt in map)
            {
                _map.Add(dt.Key, dt.Value.Clone());
            }
        }
        if (!_map.ContainsKey(startVt) || !_map.ContainsKey(targetVt)) return result;
        var start = _map[startVt];
        var target = _map[targetVt];

        Location current = null;
        var openList = new List<Location>();
        var closedList = new List<Location>();

        int g = 0;

        // start by adding the original position to the open list
        openList.Add(start);

        int cnt = 0;
        while (openList.Count > 0 && cnt < Config.LimitWhile)
        {
            cnt++;
            if (cnt == Config.LimitWhile - 1)
                Debug.LogError("huuuuuu");

            // get the square with the lowest F score
            var lowest = openList.Min(l => l.F);
            current = openList.First(l => l.F == lowest);

            // add the current square to the closed list
            closedList.Add(current);

            // remove it from the open list
            openList.Remove(current);

            // if we added the destination to the closed list, we've found a path
            if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                break;

            var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, _map, through);
            g++;

            foreach (var adjacentSquare in adjacentSquares)
            {
                // if this adjacent square is already in the closed list, ignore it
                if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y) != null)
                    continue;

                // if it's not in the open list...
                if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                        && l.Y == adjacentSquare.Y) == null)
                {
                    // compute its score, set the parent
                    adjacentSquare.G = g;
                    adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                    adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                    adjacentSquare.Parent = current;

                    // and add it to the open list
                    openList.Insert(0, adjacentSquare);
                }
                else
                {
                    // test if using the current G score makes the adjacent square's F score
                    // lower, if yes update the parent because it means it's a better path
                    if (g + adjacentSquare.H < adjacentSquare.F)
                    {
                        adjacentSquare.G = g;
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;
                    }
                }
            }
        }

        if (current != null
            && current.X == target.X && current.Y == target.Y)
        {
            result.Add(current);
            cnt = 0;
            while (current != null && cnt < Config.LimitWhile)
            {
                cnt++;
                if (cnt == Config.LimitWhile - 1)
                    Debug.LogError("huuuuuu");
                current = current.Parent;
                if (current != null)
                    result.Add(current);
            }
        }

        _map.Clear();

        result.Reverse();
        return result;
    }

    static List<Location> GetWalkableAdjacentSquares(int x, int y, Dictionary<Vector3Int, Location> map, bool through)
    {
        var proposedLocations = new List<Location>()
        {
            new Location { X = x, Y = y - 1 },
            new Location { X = x, Y = y + 1 },
            new Location { X = x - 1, Y = y },
            new Location { X = x + 1, Y = y },
        };

        //string str = "";
        var result = new List<Location>();
        for (int k = 0; k < proposedLocations.Count; k++)
        {
            var l = proposedLocations[k];
            var key = new Vector3Int(l.X, l.Y, 0);
            //if (x == 0 && y == 0)
            //    Debug.Log("key:" + key);
            if (map.ContainsKey(key))
            {
                //if (l.X == 0 && l.Y == 1)
                //{
                //    Debug.Log("entityType:" + map[key].entityBase.entityType + " wtf:" + map[key].tileType);
                //}
                //if (x == 0 && y == 0)
                //    Debug.Log("map[key].tileType:" + map[key].tileType + " EntityHelper.IsPlayer(map[key]):" + EntityHelper.IsPlayer(map[key]));
                if (through)
                {
                    result.Add(l);
                    continue;
                }
                if (map[key].tileType == TileType.CanMove)
                {
                    result.Add(l);
                    //str += string.Format("({0},{1})", l.X, l.Y);
                }
                else if (EntityHelper.IsPlayer(map[key]))
                {
                    result.Add(l);
                    //str += string.Format("({0},{1})", l.X, l.Y);
                }
            }
        }
        //if (x == 0 && y == 0)
        //    Debug.Log("x:" + x + " y:" + y + " hangxom:" + str);
        return result;
    }

    static int ComputeHScore(int x, int y, int targetX, int targetY)
    {
        return Math.Abs(targetX - x) + Math.Abs(targetY - y);
    }

    #region Get Cell
    public Vector3Int GetCellFromPosition(Vector3 pos, out bool canMove)
    {
        canMove = false;
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell)) canMove = (map[cell].tileType == TileType.CanMove);
        return cell;
    }

    public Vector3Int GetCellCanPutBombFromPos(Vector3 pos, out bool allow)
    {
        allow = false;
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell))
        {
            var location = map[cell];
            //if(cell == new Vector3Int(-3, -3, 0))
            //{
            //    Debug.Log("tile:" + location.tileType + " entity:" + location.entityBase?.name);
            //}
            if (location.tileType == TileType.CanMove || EntityHelper.IsPlayer(location)
                || EntityHelper.IsEnemy(location) || EntityHelper.IsEntityType(location, EntityType.GiftBox)
                || EntityHelper.IsEntityType(location, EntityType.BombBox))
                allow = true;
            else if(EntityHelper.IsBox(location))
            {
                if(location.entityBase is BoxBase box)
                {
                    if (box.CanBurn)
                        allow = true;
                }
            }
        }
        return cell;
    }

    public Vector3Int GetCellCanPutMario2BombFromPos(Vector3 pos, out bool allow)
    {
        allow = false;
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell))
        {
            var location = map[cell];
            if (location.tileType == TileType.CanMove || EntityHelper.IsPlayer(location)
                || EntityHelper.IsEnemy(location))
                allow = true;
        }
        return cell;
    }

    public Vector3Int GetCellEnemyCanMove(Vector3 pos, out bool allow)
    {
        allow = false;
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell))
        {
            var location = map[cell];
            if (location.tileType == TileType.CanMove || EntityHelper.IsPlayer(location) || EntityHelper.IsEnemy(location))
                allow = true;
        }
        return cell;
    }

    public Vector3Int GetCellEnemyCanMove2(Vector3 pos, out bool allow)
    {
        allow = false;
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell))
        {
            allow = true;
        }
        return cell;
    }

    public Vector3Int GetCellPlayerCanMove(Vector3 pos, out bool allow)
    {
        allow = false;
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell))
        {
            var location = map[cell];
            if (location.tileType == TileType.CanMove || EntityHelper.IsEnemy(location))
                allow = true;
        }
        return cell;
    }
    #endregion

    public Location GetLocationFromPos(Vector3 pos)
    {
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell)) return map[cell];
        else
        {
            return new Location()
            {
                X = cell.x,
                Y = cell.y,
                tileType = TileType.Obtacle
            };
        }
    }

    public void UpdateLocation(Vector3Int cell, EntityBase itemBase)
    {
        if (map.ContainsKey(cell))
        {
            map[cell].tileType = itemBase == null ? TileType.CanMove : TileType.Dynamic;
            map[cell].entityBase = itemBase;
            //Debug.Log("UpdateLocation:" + cell);

            if (map[cell].X != cell.x || map[cell].Y != cell.y)
            {
                Debug.LogWarning("map:" + map[cell] + " cell:" + cell + " entity:" + map[cell].entityBase + " tileType:" + map[cell].tileType);
            }
        }
    }

    public void UpdateDoorCloseMap(Vector3Int localPlace, EntityBase entityBase, string name)
    {
        if (map.ContainsKey(localPlace))
        {
            map[localPlace].entityBase = entityBase;
            doors.Add(map[localPlace]);
            //Debug.Log("dooor:" + localPlace + " entity:" + entityBase.name);
        }
    }

    public void UpdateCanMoveMap(Vector3 pos)
    {
        Vector3Int cell = backgroundTileMap.WorldToCell(pos);
        if (map.ContainsKey(cell))
        {
            map[cell].tileType = TileType.CanMove;
        }
    }

    public void CalculateAutoMoveLocation(Vector3Int localPlace)
    {
        map[localPlace].tileType = TileType.CanMove;
        autoMoveLocations.Add(map[localPlace]);
        //Debug.Log("locccccc:" + map[localPlace].ToVector3Int());
    }

    public void UpdateDoorOpenMap(Vector3Int localPlace, string name)
    {
        map[localPlace].tileType = TileType.CanMove;
    }

    public Vector3 GetRandomCellPos(Bound bound)
    {
        var cells = map.Where(m => m.Value.tileType == TileType.CanMove && bound.InBound(m.Key)).Select(m => m.Key);
        return cells.ElementAt(UnityEngine.Random.Range(0, cells.Count())) + new Vector3(0.5f, 0.5f);
    }
}
