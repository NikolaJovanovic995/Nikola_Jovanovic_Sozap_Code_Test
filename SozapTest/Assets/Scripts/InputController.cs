using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static event Action<Vector2Int> OnMoveAttempt;

    void Update()
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
