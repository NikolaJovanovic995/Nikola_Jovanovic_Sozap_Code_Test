using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemapGroundLayer;
    [SerializeField] private Tilemap _tilemapBoxHolderLayer;
    [SerializeField] private Tilemap _tilemapMovingLayer;

    [SerializeField] private TileBase _tileWall;
    [SerializeField] private TileBase _tileGrass;
    [SerializeField] private TileBase _tileBox;
    [SerializeField] private TileBase _tileBoxHolder;
    [SerializeField] private TileBase _tilePlayer;

    private MapElementType[,] _mapMatrix;
    private Vector2Int _playerPosition;
    private Vector2Int _mapDimension;

    public MapElementType[,] MapMatrix => _mapMatrix;
    public Vector2Int PlayerPosition => _playerPosition;

    public void Start()
    {
        //MapData mapData = new MapData();//JsonUtility.FromJson<MapData>(jsonFile.text);
        //mapData.Dimensions = new Vector2Int(8, 5);
        //mapData.PlayerPosition = new Vector2Int(4, 1);
        //mapData.MapElements = new MapElementType[8 * 5];

        string lJsonString = File.ReadAllText("Assets/LevelData/Level_4.json");
        MapData myObject = JsonUtility.FromJson<MapData>(lJsonString);
        createLevel(myObject);

        //for (int y = 0; y < mapData.Dimensions.y; y++)
        //{
        //    for (int x = 0; x < mapData.Dimensions.x; x++)
        //    {

        //        //_tileMap.SetTile(new Vector3Int(x, y, 0), getTile(myObject.MapElements.e[x*5+y]));
        //        mapData.MapElements[y*(mapData.Dimensions.x -1) + x] = MapElementType.WALL;//new MapElementData(x, y, MapElementType.WALL);
        //    }
        //}

        //string json = JsonUtility.ToJson(mapData);
        //File.WriteAllText("Assets/LevelData/Level1V2.json", json);


        //myObject = JsonUtility.FromJson<MyClass>(json);

    }

    public void MovePlayer(Vector2Int pTargetPlayerPosition, bool pMoveBox = false, Vector2Int pBoxTargetPosition = new Vector2Int())
    {
        if(pMoveBox)
        {
            _tilemapMovingLayer.SetTile((Vector3Int)pTargetPlayerPosition, null);
            _tilemapMovingLayer.SetTile((Vector3Int)pBoxTargetPosition, _tileBox);
            _mapMatrix[pBoxTargetPosition.x, pBoxTargetPosition.y] = MapElementType.BOX;
        }
        _mapMatrix[_playerPosition.x, _playerPosition.y] = MapElementType.GRASS;
        _tilemapMovingLayer.SetTile((Vector3Int)_playerPosition, null);
        _playerPosition = pTargetPlayerPosition;
        _mapMatrix[_playerPosition.x, _playerPosition.y] = MapElementType.PLAYER;
        _tilemapMovingLayer.SetTile((Vector3Int)_playerPosition, _tilePlayer);
    }

    private void createLevel(MapData pMapData)
    {
        _mapDimension = pMapData.Dimensions;
        _mapMatrix = new MapElementType[_mapDimension.x, _mapDimension.y];
        _playerPosition = pMapData.PlayerPosition;
        Vector3Int position;
        clearAllTiles();

        for (int j = 0; j < _mapDimension.y; j++)
        {
            for (int i = 0; i < _mapDimension.x; i++)
            {
                _mapMatrix[i, j] = pMapData.MapElements[j * (_mapDimension.x) + i];
                position = new Vector3Int(i, j, 0);
                MapElementType elementType = _mapMatrix[i, j];
                _tilemapGroundLayer.SetTile(position, getFloorTile(elementType));
                if (elementType == MapElementType.BOX_HOLDER || elementType == MapElementType.BOX_HOLDER_WITH_BOX)
                {
                    _tilemapBoxHolderLayer.SetTile(position, _tileBoxHolder);
                }
                TileBase movingLayerTile = getMovingLayerTile(elementType);
                if (movingLayerTile != null)
                {
                    _tilemapMovingLayer.SetTile(position, movingLayerTile);
                }
            }
        }
    }

    //private MapElementType[,] convertArrayToMatrix(int[] pArray, int pX, int pY)
    //{
    //    if (pArray.Length != pX * pY)
    //    {
    //        throw new ArgumentException("Invalid length");
    //    }
    //    MapElementType[,] lMatrix = new MapElementType[pX, pY];
    //    Buffer.BlockCopy(pArray, 0, lMatrix, 0, pArray.Length * sizeof(int));
    //    return lMatrix;
    //}

    //private void createLevel(MapData pMapData)
    //{
    //    _mapMatrix = new int[pMapData.Dimensions.x, pMapData.Dimensions.y];
    //    clearAllTiles();
    //    foreach (MapElementType mapElementType in pMapData.MapElements)
    //    {
    //        _tilemapGroundLayer.SetTile((Vector3Int)mapElementType.Position, getFloorTile(mapElementType.MapElementType));

    //        if (mapElementType.MapElementType == MapElementType.BOX_HOLDER)
    //        {
    //            _tilemapBoxHolderLayer.SetTile((Vector3Int)mapElementType.Position, _tileBoxHolder);
    //        }
    //        TileBase movingLayerTile = getMovingLayerTile(mapElementType.MapElementType);
    //        if (movingLayerTile != null)
    //        {
    //            _tilemapMovingLayer.SetTile((Vector3Int)mapElementType.Position, movingLayerTile);
    //        }
    //        _mapMatrix[mapElementType.Position.x, mapElementType.Position.y] = (int)mapElementType.MapElementType;
    //        _playerPosition = pMapData.PlayerPosition;
    //    }
    //}

    private string getPath(int pLevelNumber)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Application.persistentDataPath + "/" + "Level_"+ pLevelNumber + ".json";
        }
        else
        {
            //PC
            return Application.dataPath + "/" + "Level_" + pLevelNumber + ".json";
        }
    }



    private TileBase getFloorTile(MapElementType pMapElement)
    {
        switch (pMapElement)
        {
            case MapElementType.WALL:
                return _tileWall;
            case MapElementType.GRASS:
            case MapElementType.BOX:
            case MapElementType.BOX_HOLDER:
            case MapElementType.PLAYER:
            case MapElementType.BOX_HOLDER_WITH_BOX:
                return _tileGrass;
            case MapElementType.EMPTY:
            default:
                return null;
        }
    }
    private TileBase getMovingLayerTile(MapElementType pMapElement)
    {
        switch (pMapElement)
        {
            case MapElementType.BOX:
            case MapElementType.BOX_HOLDER_WITH_BOX:
                return _tileBox;
            case MapElementType.PLAYER:
                return _tilePlayer;
            default:
                return null;
        }
    }

    private void clearAllTiles()
    {
        _tilemapGroundLayer.ClearAllTiles();
        _tilemapBoxHolderLayer.ClearAllTiles();
        _tilemapMovingLayer.ClearAllTiles();
    }
}

[Serializable]
public class MapData
{
    public Vector2Int Dimensions;
    public Vector2Int PlayerPosition;
    public MapElementType[] MapElements;
}

[Serializable]
public enum MapElementType
{
    EMPTY = 0,
    WALL = 1,
    GRASS = 2,
    BOX = 3,
    BOX_HOLDER = 4,
    PLAYER = 5,
    BOX_HOLDER_WITH_BOX = 6
}
