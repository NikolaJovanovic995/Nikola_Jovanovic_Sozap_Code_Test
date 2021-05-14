using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LevelController _levelController;

    public void Awake()
    {
        InputController.OnMoveAttempt += InputController_OnMoveAttempt;
    }

    private void InputController_OnMoveAttempt(Vector2Int pDirection)
    {
        Vector2Int _currentPlayerPosition = _levelController.PlayerPosition;
        Vector2Int _targetPlayerPosition = _currentPlayerPosition + pDirection;
        if (isPositionInvalid(_targetPlayerPosition))
        {
            return;
        }

        MapElementType targetedTile = _levelController.MapMatrix[_targetPlayerPosition.x, _targetPlayerPosition.y];
        switch (targetedTile)
        {
            case MapElementType.WALL:
                return;
            case MapElementType.BOX_HOLDER:
            case MapElementType.GRASS:
                _levelController.MovePlayer(_targetPlayerPosition);
                break;
            case MapElementType.BOX:
            case MapElementType.BOX_HOLDER_WITH_BOX:
                Vector2Int _targetBoxPosition = _targetPlayerPosition + pDirection;
                if (isPositionInvalid(_targetBoxPosition) || isTargetPositionForBoxImpossible(_targetBoxPosition))
                {
                    return;
                }
                else
                {
                    _levelController.MovePlayer(_targetPlayerPosition, true, _targetBoxPosition);
                }

                break;
            default:
                break;
        }

    }

    private bool isPositionInvalid(Vector2Int pPosition)
    {
        return (pPosition.x < 0 || pPosition.y < 0 || pPosition.x >= _levelController.MapMatrix.GetLength(0) || pPosition.y >= _levelController.MapMatrix.GetLength(1));
    }

    private bool isTargetPositionForBoxImpossible(Vector2Int pPosition)
    {
        MapElementType mapElementType = (MapElementType)_levelController.MapMatrix[pPosition.x, pPosition.y];
        return mapElementType == MapElementType.WALL || mapElementType == MapElementType.BOX;
    }
}
