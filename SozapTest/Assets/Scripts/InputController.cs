using System;
using UnityEngine;

/// <summary>
/// Manages input events
/// </summary>
public class InputController : MonoBehaviour
{
    public static event Action<Vector2Int> OnMoveAttempt;
    public static bool ProcessPlayerMovementInput = false;
    public static bool ProcessKeyboardInput = false;

    public static void FireMoveEvent(Vector2Int pDirection)
    {
        if (ProcessPlayerMovementInput)
        {
            OnMoveAttempt?.Invoke(pDirection);
        }
    }

    public void Update()
    {
        if (ProcessKeyboardInput)
        {
            if (ProcessPlayerMovementInput)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    OnMoveAttempt?.Invoke(Vector2Int.up);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    OnMoveAttempt?.Invoke(Vector2Int.down);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    OnMoveAttempt?.Invoke(Vector2Int.left);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    OnMoveAttempt?.Invoke(Vector2Int.right);
                }
            }
        }
    }
}
