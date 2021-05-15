using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    public event Action OnLevelComplete;

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
    private Dictionary<Vector2Int, bool> _boxHoldersDictionary;
    private int _capturedBoxesCount = 0;

    public Vector2Int PlayerPosition => _playerPosition;

    public MapElementType GetElementAtIndex(int pX, int pY)
    {
        return (pX < 0 || pY < 0 || pX >= _mapMatrix.GetLength(0) || pY >= _mapMatrix.GetLength(1)) ? MapElementType.EMPTY : _mapMatrix[pX, pY];
    }

    public void MovePlayer(Vector2Int pTargetPlayerPosition, bool pMoveBox = false, Vector2Int pBoxTargetPosition = new Vector2Int())
    {
        _mapMatrix[_playerPosition.x, _playerPosition.y] = MapElementType.GRASS;
        _tilemapMovingLayer.SetTile((Vector3Int)_playerPosition, null);
        _playerPosition = pTargetPlayerPosition;
        _mapMatrix[_playerPosition.x, _playerPosition.y] = MapElementType.PLAYER;
        _tilemapMovingLayer.SetTile((Vector3Int)_playerPosition, _tilePlayer);
        if (pMoveBox)
        {
            _tilemapMovingLayer.SetTile((Vector3Int)pBoxTargetPosition, _tileBox);
            _mapMatrix[pBoxTargetPosition.x, pBoxTargetPosition.y] = MapElementType.BOX;
            onBoxMovement(pTargetPlayerPosition, pBoxTargetPosition);
        }
    }

    private void onBoxMovement(Vector2Int pBoxStartPosition, Vector2Int pBoxTargetPosition)
    {
        if(_boxHoldersDictionary.ContainsKey(pBoxStartPosition))
        {
            if(_boxHoldersDictionary[pBoxStartPosition] == true)
            {
                _capturedBoxesCount--;
                _boxHoldersDictionary[pBoxStartPosition] = false;
                Debug.Log("Captured boxes: "+ _capturedBoxesCount);
            }
        }

        if (_boxHoldersDictionary.ContainsKey(pBoxTargetPosition))
        {
            if (_boxHoldersDictionary[pBoxTargetPosition] == false)
            {
                _capturedBoxesCount++;
                _boxHoldersDictionary[pBoxTargetPosition] = true;
                Debug.Log("Captured boxes: " + _capturedBoxesCount);
            }
        }

        if(_capturedBoxesCount == _boxHoldersDictionary.Count)
        {
            Debug.Log("Game Won!!!");
            OnLevelComplete?.Invoke();
        }
    }

    public void CreateLevel(MapData pMapData)
    {
        _mapDimension = pMapData.Dimensions;
        _mapMatrix = new MapElementType[_mapDimension.x, _mapDimension.y];
        _playerPosition = pMapData.PlayerPosition;
        _boxHoldersDictionary = new Dictionary<Vector2Int, bool>();
        _capturedBoxesCount = 0;
        clearAllTiles();
        createMap(pMapData);
    }

    private void createMap(MapData pMapData)
    {
        for (int j = 0; j < _mapDimension.y; j++)
        {
            for (int i = 0; i < _mapDimension.x; i++)
            {
                _mapMatrix[i, j] = pMapData.MapElements[j * (_mapDimension.x) + i];
                setTyle(_mapMatrix[i, j], new Vector3Int(i, j, 0));
            }
        }
    }

    private void setTyle(MapElementType pElementType, Vector3Int pPosition)
    {
        _tilemapGroundLayer.SetTile(pPosition, getFloorTile(pElementType));
        if (pElementType == MapElementType.BOX_HOLDER || pElementType == MapElementType.BOX_HOLDER_WITH_BOX)
        {
            _tilemapBoxHolderLayer.SetTile(pPosition, _tileBoxHolder);
            bool lIsBoxHolderCaptured = pElementType == MapElementType.BOX_HOLDER_WITH_BOX;
            if(lIsBoxHolderCaptured)
            {
                _capturedBoxesCount++;
            }
            _boxHoldersDictionary.Add((Vector2Int)pPosition, lIsBoxHolderCaptured);

        }
        TileBase movingLayerTile = getMovingLayerTile(pElementType);
        if (movingLayerTile != null)
        {
            _tilemapMovingLayer.SetTile(pPosition, movingLayerTile);
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
