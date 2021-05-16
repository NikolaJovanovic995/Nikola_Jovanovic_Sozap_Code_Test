using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages map generatation, executes player and box movement during gameplay, stores all information for the current state of level
/// </summary>
public class LevelController : MonoBehaviour
{
    public event Action OnLevelComplete;
    public Vector2Int PlayerPosition => _playerPosition;

    [SerializeField] private Tilemap _tilemapGroundLayer;
    [SerializeField] private Tilemap _tilemapBoxHolderLayer;
    [SerializeField] private Tilemap _tilemapPlayerAndBoxLayer;
    [SerializeField] private TileBase _tileWall;
    [SerializeField] private TileBase _tileGrass;
    [SerializeField] private TileBase _tileBox;
    [SerializeField] private TileBase _tileBoxHolder;
    [SerializeField] private TileBase _tilePlayer;
    [SerializeField] private ParticleSystem _particleSystem;

    private MapElementType[,] _mapMatrix;
    private Vector2Int _playerPosition;
    private Vector2Int _mapDimension;
    private Dictionary<Vector2Int, bool> _boxHolderDictionary;
    private int _capturedBoxesCount = 0;

    public MapElementType GetElementAtIndex(int pX, int pY)
    {
        return (pX < 0 || pY < 0 || pX >= _mapMatrix.GetLength(0) || pY >= _mapMatrix.GetLength(1)) ? MapElementType.EMPTY : _mapMatrix[pX, pY];
    }

    public void MovePlayer(Vector2Int pTargetPlayerPosition, bool pMoveBox = false, Vector2Int pTargetBoxPosition = new Vector2Int())
    {
        _mapMatrix[_playerPosition.x, _playerPosition.y] = MapElementType.GRASS;
        _tilemapPlayerAndBoxLayer.SetTile((Vector3Int)_playerPosition, null);
        _playerPosition = pTargetPlayerPosition;
        _mapMatrix[_playerPosition.x, _playerPosition.y] = MapElementType.PLAYER;
        _tilemapPlayerAndBoxLayer.SetTile((Vector3Int)_playerPosition, _tilePlayer);
        if (pMoveBox)
        {
            _tilemapPlayerAndBoxLayer.SetTile((Vector3Int)pTargetBoxPosition, _tileBox);
            _mapMatrix[pTargetBoxPosition.x, pTargetBoxPosition.y] = MapElementType.BOX;
            onBoxMovement(pTargetPlayerPosition, pTargetBoxPosition);
        }
    }

    private void onBoxMovement(Vector2Int pBoxStartPosition, Vector2Int pBoxTargetPosition)
    {
        if(_boxHolderDictionary.ContainsKey(pBoxStartPosition))
        {
            if(_boxHolderDictionary[pBoxStartPosition] == true)
            {
                _capturedBoxesCount--;
                _boxHolderDictionary[pBoxStartPosition] = false;
            }
        }
        bool lIsBoxInPlace = false;
        if (_boxHolderDictionary.ContainsKey(pBoxTargetPosition))
        {
            if (_boxHolderDictionary[pBoxTargetPosition] == false)
            {
                _capturedBoxesCount++;
                _boxHolderDictionary[pBoxTargetPosition] = true;
                _particleSystem.transform.position = new Vector3(pBoxTargetPosition.x +0.5f, pBoxTargetPosition.y + 0.5f, 0f);
                _particleSystem.Play();
                lIsBoxInPlace = true;
            }
        }

        if (_capturedBoxesCount == _boxHolderDictionary.Count)
        {
            AudioManager.Instance.Play("LevelFinished");
            OnLevelComplete?.Invoke();
        }
        else if(lIsBoxInPlace)
        {
            AudioManager.Instance.Play("BoxInPlace");
        }
    }

    public void CreateLevel(MapData pMapData)
    {
        _mapDimension = pMapData.Dimensions;
        _mapMatrix = new MapElementType[_mapDimension.x, _mapDimension.y];
        _playerPosition = pMapData.PlayerPosition;
        _boxHolderDictionary = new Dictionary<Vector2Int, bool>();
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
                setTyleForAllLayers(_mapMatrix[i, j], new Vector3Int(i, j, 0));
            }
        }
    }

    private void setTyleForAllLayers(MapElementType pMapElement, Vector3Int pPosition)
    {
        setTyle(_tilemapGroundLayer, getFloorLayerTile(pMapElement), pPosition);
        setTyle(_tilemapPlayerAndBoxLayer, getPlayerAndBoxLayerTile(pMapElement), pPosition);

        if (isMapElementWithBox(pMapElement))
        {
            setBoxHolderTile(pMapElement == MapElementType.BOX_HOLDER_AND_BOX, pPosition);
        }
    }

    private void setTyle(Tilemap pTilemap, TileBase pTile, Vector3Int pPosition)
    {
        if(pTile != null)
        {
            pTilemap.SetTile(pPosition, pTile);
        }
    }

    private void setBoxHolderTile(bool pIsBoxHolderCaptured, Vector3Int pPosition)
    {
        _tilemapBoxHolderLayer.SetTile(pPosition, _tileBoxHolder);
        _boxHolderDictionary.Add((Vector2Int)pPosition, pIsBoxHolderCaptured);
        if (pIsBoxHolderCaptured)
        {
            _capturedBoxesCount++;
        }
    }

    private TileBase getFloorLayerTile(MapElementType pMapElement)
    {
        switch (pMapElement)
        {
            case MapElementType.WALL:
                return _tileWall;
            case MapElementType.GRASS:
            case MapElementType.BOX:
            case MapElementType.BOX_HOLDER:
            case MapElementType.PLAYER:
            case MapElementType.BOX_HOLDER_AND_BOX:
                return _tileGrass;
            default:
                return null;
        }
    }

    private TileBase getPlayerAndBoxLayerTile(MapElementType pMapElement)
    {
        switch (pMapElement)
        {
            case MapElementType.BOX:
            case MapElementType.BOX_HOLDER_AND_BOX:
                return _tileBox;
            case MapElementType.PLAYER:
            case MapElementType.BOX_HOLDER_AND_PLAYER:
                return _tilePlayer;
            default:
                return null;
        }
    }

    private bool isMapElementWithBox(MapElementType pMapElement)
    {
        switch (pMapElement)
        {
            case MapElementType.BOX_HOLDER:
            case MapElementType.BOX_HOLDER_AND_BOX:
            case MapElementType.BOX_HOLDER_AND_PLAYER:
                return true;
            default:
                return false;
        }
    }

    private void clearAllTiles()
    {
        _tilemapGroundLayer.ClearAllTiles();
        _tilemapBoxHolderLayer.ClearAllTiles();
        _tilemapPlayerAndBoxLayer.ClearAllTiles();
    }
}
