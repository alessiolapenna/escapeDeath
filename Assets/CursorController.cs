using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType
{
    pointer,
    walk,
    walkPressed,
    pickup,
    attack
}

public class CursorController : MonoBehaviour
{
    [SerializeField] Texture2D cursorPointer;
    [SerializeField] Texture2D cursorWalk;
    [SerializeField] Texture2D cursorWalkPressed;
    [SerializeField] Texture2D cursorPickup;
    [SerializeField] Texture2D cursorAttack;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UpdateCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.pointer:
                Cursor.SetCursor(cursorPointer, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.walk:
                Cursor.SetCursor(cursorWalk, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.walkPressed:
                Cursor.SetCursor(cursorWalkPressed, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.pickup:
                Cursor.SetCursor(cursorPickup, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.attack:
                Cursor.SetCursor(cursorAttack, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
}
