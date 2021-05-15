using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LevelController _levelController;

    public void Init()
    {
        InputController.OnMoveAttempt += onMoveAttempt;
    }

    private void onMoveAttempt(Vector2Int pDirection)
    {
        Vector2Int lCurrentPlayerPosition = _levelController.PlayerPosition;
        Vector2Int lTargetPlayerPosition = lCurrentPlayerPosition + pDirection;
        MapElementType lTargetedTile = _levelController.GetElementAtIndex(lTargetPlayerPosition.x, lTargetPlayerPosition.y);

        switch (lTargetedTile)
        {
            case MapElementType.BOX_HOLDER:
            case MapElementType.GRASS:
                _levelController.MovePlayer(lTargetPlayerPosition);
                break;
            case MapElementType.BOX:
            case MapElementType.BOX_HOLDER_WITH_BOX:
                tryBoxPush(lTargetPlayerPosition + pDirection, lTargetPlayerPosition);
                break;
            default:
                break;
        }

    }

    private void tryBoxPush(Vector2Int pTargetBoxPosition, Vector2Int pTargetPlayerPosition)
    {
        if (isTargetPositionForBoxPossible(pTargetBoxPosition))
        {
            _levelController.MovePlayer(pTargetPlayerPosition, true, pTargetBoxPosition);
        }
    }

    private bool isTargetPositionForBoxPossible(Vector2Int pPosition)
    {
        MapElementType mapElementType = _levelController.GetElementAtIndex(pPosition.x, pPosition.y);
        return mapElementType == MapElementType.GRASS || mapElementType == MapElementType.BOX_HOLDER;
    }
}
