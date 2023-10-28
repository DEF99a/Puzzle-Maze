using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class MapObject : MonoBehaviour
{

    [SerializeField] private bool _autoLoad;
    [SerializeField] private Tilemap _loadTileWater;
    [SerializeField] private Tilemap _loadTileGroundBasic_1;
    [SerializeField] private Tilemap _loadTileGroundBasic_2;

    [Space]
    [SerializeField] private GameObject _prefabWater;
    [SerializeField] private GameObject _prefabGroundBasic1;
    [SerializeField] private GameObject _prefabGroundBasic2;
    [SerializeField] private Tilemap _frontWaterTilemap;
    [SerializeField] private Tilemap _frontGroundBasic1;
    [SerializeField] private Tilemap _frontGroundBasic2;
    
    [Space]
    [SerializeField] private List<WaterTile> WaterTiles;
    [SerializeField] private List<BrickTile> _brickTiles;
    [SerializeField] private List<GroundBasic2Tile> _brickTiles2;
    [Space]

    [SerializeField] private Transform _waterStore;
    [SerializeField] private Transform _groundBasic1Store;
    [SerializeField] private Transform _groundBasic2Store;

    private void OnValidate()
    {
        if (!_autoLoad)
        {

        LoadWaterTileData();
        LoadBrickTileData();
        LoadBrickTileData2();
        }
        else
        {
            LoadWaterTileData();
        //    LoadBrickTileData();
            LoadBrickTileData2();
        }
    }

    void Start()
    {
        GenTileObjects();
     //   LoadBrickTileData();
    }

    [ContextMenu("Load Water Tile Data")]
    void LoadWaterTileData()
    {
        WaterTiles.Clear();
        var tilemap = _frontWaterTilemap;
        if (_autoLoad)
        {
            tilemap = _loadTileWater;
            if (_loadTileWater == null)

            {
                var count = transform.parent.childCount;
                for (int i = 0; i < count; i++)
                {
                    if(transform.parent.GetChild(i).name.Equals("Front"))
                    {
                        tilemap = transform.parent.GetChild(i).GetComponent<Tilemap>();
                        _loadTileWater                                  = transform.parent.GetChild(i).GetComponent<Tilemap>();
                    }
                }
            }
        }


            int[,] maps = new int[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];



        var x = -1;
        var y = 0;
        var xMax = tilemap.cellBounds.size.x;
        var yMax = tilemap.cellBounds.size.y;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            x += 1;
            if (x >= xMax)
            {
                x = 0;
                y += 1;
                if (y >= yMax)
                {
                    y = 0;
                }
            }


            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (tilemap.HasTile(localPlace))
            {
                var tile = tilemap.GetTile(localPlace);
                //   Debug.Log(x + "-" + y);

                if (!_autoLoad)
                {
                    maps[x, y] = tile.name.Contains("water") ? 1 : 0;
                }
                else
                {
                    maps[x, y] = tile.name.Contains("water_set") ? 1 : 0;

                }
            }
        }

        x = -1;
        y = 0;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            x += 1;
            if (x >= xMax)
            {
                x = 0;
                y += 1;
                if (y >= yMax)
                {
                    y = 0;
                }
            }

            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (tilemap.HasTile(localPlace))
            {
                var tile = tilemap.GetTile(localPlace);

                if (!_autoLoad)
                {
                    if (tile.name.Contains("water"))
                    {
                        var position = localPlace + new Vector3(0.5f, 0.5f);
                        var water = new WaterTile();
                        water.SetPosition(position);
                        water.SetPositionType(maps, x, y, xMax, yMax);
                        WaterTiles.Add(water);
                    }
                }
                else
                {
                    if (tile.name.Contains("water_set"))
                    {
                        var position = localPlace + new Vector3(0.5f, 0.5f);
                        var water = new WaterTile();
                        water.SetPosition(position);
                        water.SetPositionType(maps, x, y, xMax, yMax);
                        WaterTiles.Add(water);
                    }
                }

            }
        }
    }

    [ContextMenu("Load Brick Tile Data 2")]
    void LoadBrickTileData2()
    {
        _brickTiles2.Clear();


        var tilemap = _frontGroundBasic2;
        if (_autoLoad)
        {
            tilemap = _loadTileGroundBasic_2;
            if (_loadTileGroundBasic_2 == null)

            {
                var count = transform.parent.childCount;
                for (int i = 0; i < count; i++)
                {
                    if (transform.parent.GetChild(i).name.Equals("Front"))
                    {
                        tilemap = transform.parent.GetChild(i).GetComponent<Tilemap>();
                        _loadTileGroundBasic_2 = transform.parent.GetChild(i).GetComponent<Tilemap>(); 
                    }
                }
            }
        }

        int[,] maps = new int[tilemap.cellBounds.size.x, tilemap.cellBounds.size.y];



        var x = -1;
        var y = 0;
        var xMax = tilemap.cellBounds.size.x;
        var yMax = tilemap.cellBounds.size.y;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            x += 1;
            if (x >= xMax)
            {
                x = 0;
                y += 1;
                if (y >= yMax)
                {
                    y = 0;
                }
            }


            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (tilemap.HasTile(localPlace))
            {
                var tile = tilemap.GetTile(localPlace);
                //   Debug.Log(x + "-" + y);

                if (!_autoLoad)
                { 
                    maps[x, y] = tile.name.Contains("brick2") ? 1 : 0;
                }
                else
                {
                    maps[x, y] = tile.name.Contains("ground_set_2") ? 1 : 0;
                }
            }
        }

        if (_autoLoad)
        {
            for (int i = xMax-1; i >=0 ; i--)
            {
                for (int j = yMax-1; j>=0 ; j--)
                {
                    if (maps[i, j] == 1)
                    {
                        var top = 1;
                        var bot = 0;

                        if (j == 0)
                        {
                            bot = 0;
                            top = maps[i, j + 1];
                        }
                        else if (j == yMax - 1)
                        {
                            bot = maps[i, j - 1];
                            top = 0;
                        }
                        else
                        {
                            bot = maps[i, j - 1];
                            top = maps[i, j + 1];
                        }

                        if (top  == 1 && bot == 0)
                        {
                            maps[i, j] = 0;
                        }
                    }
                }
            }
        }

        x = -1;
        y = 0;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            x += 1;
            if (x >= xMax)
            {
                x = 0;
                y += 1;
                if (y >= yMax)
                {
                    y = 0;
                }
            }

            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (tilemap.HasTile(localPlace))
            {
                var tileObject = tilemap.GetTile(localPlace);

                if (!_autoLoad)
                {
                    if (tileObject.name.Contains("brick2"))
                    {
                        var position = localPlace + new Vector3(0.5f, 0.5f);
                        var tile = new GroundBasic2Tile();
                        tile.SetPosition(position);
                        tile.SetPositionType(maps, x, y, xMax, yMax);
                        _brickTiles2.Add(tile);
                    }
                }
                else
                {
                    if (tileObject.name.Contains("ground_set_2") && maps[x,y]==1)
                    {
                        var position = localPlace + new Vector3(0.5f, 0.5f);
                        var tile = new GroundBasic2Tile();
                        tile.SetPosition(position);
                        tile.SetPositionType(maps, x, y, xMax, yMax);
                        _brickTiles2.Add(tile);
                    }
                }
            }
        }
    }
    

    [ContextMenu("Load Ground Basic 1 Tile Data")]
    void LoadBrickTileData()
    {
        _brickTiles.Clear();


        var tilemap = _frontGroundBasic1;
        if (_autoLoad)
        {
            tilemap = _loadTileGroundBasic_1;
            if (_loadTileGroundBasic_1 == null)

            {
                var count = transform.parent.childCount;
                for (int i = 0; i < count; i++)
                {
                    if (transform.parent.GetChild(i).name.Equals("Front"))
                    {
                        tilemap = transform.parent.GetChild(i).GetComponent<Tilemap>();
                        _loadTileGroundBasic_1 = transform.parent.GetChild(i).GetComponent<Tilemap>();
                    }
                }
            }
        }

        var x = -1;
        var y = 0;
        var xMax = tilemap.cellBounds.size.x;
        var yMax = tilemap.cellBounds.size.y;
        var mapsBrick = new int[xMax, yMax];
      var  mapsBrickTile = new BrickTile[xMax, yMax];
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            x += 1;
            if (x >= xMax)
            {
                x = 0;
                y += 1;
                if (y >= yMax)
                {
                    y = 0;
                }
            }

            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (tilemap.HasTile(localPlace))
            {
                var tile = tilemap.GetTile(localPlace);

                if (!_autoLoad)
                {
                    var brick = tile.name.Contains("brick") ? 1 : 0;
                    mapsBrick[x, y] = brick;
                }
                else
                {
                    var brick = tile.name.Contains("ground_set_2") ? 1 : 0;
                    mapsBrick[x, y] = brick;
                }

            }
        }

        //    if (Application.isEditor)
         x = -1;
        y = 0;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            x += 1;
            if (x >= xMax)
            {
                x = 0;
                y += 1;
                if (y >= yMax)
                {
                    y = 0;
                }
            }

            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (tilemap.HasTile(localPlace))
            {
                var tile = tilemap.GetTile(localPlace);
              

                if (!_autoLoad)
                {
                    if (tile.name.Contains("brick"))
                    {
                        var position = localPlace + new Vector3(0.5f, 0.5f);
                        var x1 = x;
                        var y1 = y;
                        var t = new BrickTile();
                        t.SetPosition(position);
                        t.SetPositionType(mapsBrick, x1, y1, xMax, yMax);
                        _brickTiles.Add(t);
                        mapsBrickTile[x1, y1] = t;
                    }
                }
                else
                {
                    if (tile.name.Contains("ground_set_2"))
                    {
                        var position = localPlace + new Vector3(0.5f, 0.5f);
                        var x1 = x;
                        var y1 = y;
                        var t = new BrickTile();
                        t.SetPosition(position);
                        t.SetPositionType(mapsBrick, x1, y1, xMax, yMax);
                        _brickTiles.Add(t);
                        mapsBrickTile[x1, y1] = t;
                    }
                }
            }
        }
    }

    [ContextMenu("Gen Tile Objects")]
    void GenTileObjects()
    {
        // WATER
        _frontWaterTilemap.gameObject.SetActive(false);
        for (int i = 0; i < WaterTiles.Count; i++)
        {
            var go = Instantiate(_prefabWater, WaterTiles[i].Pos, Quaternion.identity);
            go.transform.parent = _waterStore.transform;
            go.GetComponent<Water>().Set(WaterTiles[i]);
        }

        // Ground Basic 1
        _frontGroundBasic1.gameObject.SetActive(false);
        for (int i = 0; i < _brickTiles.Count; i++)
        {
            var go = Instantiate(_prefabGroundBasic1, _brickTiles[i].Pos, Quaternion.identity);
            go.transform.parent = _groundBasic1Store.transform;
            go.GetComponent<Brick>().Set(_brickTiles[i], null);
        }
        
        // Ground Basic 2
        _frontGroundBasic2.gameObject.SetActive(false);
        for (int i = 0; i < _brickTiles2.Count; i++)
        {
            var go = Instantiate(_prefabGroundBasic2, _brickTiles2[i].Pos, Quaternion.identity);
            go.transform.parent = _groundBasic2Store.transform;
            go.GetComponent<GroundBasic2>().Set(_brickTiles2[i]);
        }
    }
}