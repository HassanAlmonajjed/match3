using UnityEngine;
using System;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class TileController : MonoBehaviour
{
    public static event Action<TileController, SwipeDirection> PlayerSwiped;

    private SpriteRenderer _spriteRenderer;
    private Vector2 _mouseDownPos;
    private bool _isPressed;
    private const float MinSwipeDistance = 0.3f;
    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileImage(int id)
    {
        Sprite sprite = BoardController.Instance.GetSpriteForTileById(id);
        _spriteRenderer.sprite = sprite;
    }

    private void OnMouseDown()
    {
        _isPressed = true;
        _mouseDownPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (!_isPressed)
            return;

        _isPressed = false;

        Vector2 mouseUpPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = mouseUpPos - _mouseDownPos;

        SwipeDirection direction = GetSwipeDirection(delta);

        if (direction != SwipeDirection.None)
        {
            PlayerSwiped?.Invoke(this, direction);
        }
    }

    private SwipeDirection GetSwipeDirection(Vector2 delta)
    {
        if (delta.magnitude < MinSwipeDistance)
            return SwipeDirection.None;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else
        {
            return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
        }
    }

    public static void SwipeTiles(TileController tile1, TileController tile2)
    {
        if (tile1 == null || tile2 == null)
            return;

        AnimateSwipe(tile1, tile2);
    }

    private static void AnimateSwipe(TileController tile1, TileController tile2)
    {
        if (tile1 == null || tile2 == null) return;

        Vector3 tile1StartPos = tile1.transform.position;
        Vector3 tile2StartPos = tile2.transform.position;
        float duration = 0.3f;

        // Animate tile1 to tile2's position
        tile1.transform.DOMove(tile2StartPos, duration)
            .SetEase(Ease.InOutQuad)
            .SetLink(tile1.gameObject);

        if (tile2 != null) // double check in case destruction happened exactly between lines
        {
            tile2.transform.DOMove(tile1StartPos, duration)
                .SetEase(Ease.InOutQuad)
                .SetLink(tile2.gameObject);
        }
    }


}

public enum SwipeDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

