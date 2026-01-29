using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private Tile[,] _grid;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int UniqueTiles { get; private set; }
    public Board(int width, int height, int uniqueTiles = 4)
    {
        _grid = new Tile[width, height];
        Width = width;
        Height = height;
        UniqueTiles = uniqueTiles;
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        if (!IsInsideGrid(position))
            return null;

        return _grid[position.x, position.y];
    }

    public Vector2Int GetTileAtDirection(Vector2Int position, SwipeDirection direction)
    {
        Vector2Int neighborPos = position;

        switch (direction)
        {
            case SwipeDirection.Up:
                neighborPos = new Vector2Int(position.x, position.y - 1);
                break;
            case SwipeDirection.Down:
                neighborPos = new Vector2Int(position.x, position.y + 1);
                break;
            case SwipeDirection.Left:
                neighborPos = new Vector2Int(position.x - 1, position.y);
                break;
            case SwipeDirection.Right:
                neighborPos = new Vector2Int(position.x + 1, position.y);
                break;
        }

        // Check if the neighbor position is within bounds
        if (!IsInsideGrid(neighborPos))
            return new Vector2Int(-1, -1);

        return neighborPos;
    }

    public void Populate()
    {
        _grid = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int tileId = GetValidTileId(x, y);
                _grid[x, y] = new Tile { id = tileId };
            }
        }
    }

    public void Populate(int[,] gridIds)
    {
        _grid = new Tile[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y < gridIds.GetLength(0) && x < gridIds.GetLength(1) && gridIds[y, x] > 0)
                {
                    _grid[x, y] = new Tile { id = gridIds[y, x] };
                }
            }
        }
    }

    private int GetValidTileId(int x, int y)
    {
        int tileId;
        bool isValid;

        do
        {
            tileId = Random.Range(1, UniqueTiles);
            isValid = true;

            // Check horizontal left matches (need at least 2 tiles to the left)
            if (x >= 2 && _grid[x - 1, y] != null && _grid[x - 2, y] != null)
            {
                if (_grid[x - 1, y].id == _grid[x - 2, y].id && _grid[x - 1, y].id == tileId)
                {
                    isValid = false;
                }
            }

            // Check vertical up matches (need at least 2 tiles above)
            if (isValid && y >= 2 && _grid[x, y - 1] != null && _grid[x, y - 2] != null)
            {
                if (_grid[x, y - 1].id == _grid[x, y - 2].id && _grid[x, y - 1].id == tileId)
                {
                    isValid = false;
                }
            }

        } while (!isValid);

        return tileId;
    }

    public void Swipe(Vector2Int swipeStart, Vector2Int swipeEnd)
    {
        if (IsInsideGrid(swipeStart) && IsInsideGrid(swipeEnd))
        {
            (_grid[swipeEnd.x, swipeEnd.y], _grid[swipeStart.x, swipeStart.y]) = (_grid[swipeStart.x, swipeStart.y], _grid[swipeEnd.x, swipeEnd.y]);
        }
    }

    public bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    }

    public void Print()
    {
        string grid = "";
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                grid += _grid[x, y] == null ? "X " : _grid[x, y].ToString() + " ";
            }
            grid += "\n";
        }
    }

    public HashSet<Vector2Int> DetectMatch()
    {
        var horizontalMatches = DetectHorizontalMatches();
        var verticalMatches = DetectVerticalMatches();
        horizontalMatches.UnionWith(verticalMatches);
        return horizontalMatches;
    }

    private HashSet<Vector2Int> DetectHorizontalMatches()
    {
        HashSet<Vector2Int> matches = new();
        for (int y = 0; y < Height; y++)
        {
            int matchLength = 1;
            for (int x = 1; x < Width; x++)
            {
                Tile current = _grid[x, y];
                Tile previous = _grid[x - 1, y];

                // Only count a match if both tiles are not null and they have the same ID
                if (current != null && previous != null && current == previous)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        AddHorizontalMatch(matches, x - 1, y, matchLength);
                    }
                    matchLength = 1;
                }
            }
            // end of row
            if (matchLength >= 3)
            {
                AddHorizontalMatch(matches, Width - 1, y, matchLength);
            }
        }
        return matches;
    }

    private HashSet<Vector2Int> DetectVerticalMatches()
    {
        HashSet<Vector2Int> matches = new();
        for (int x = 0; x < Width; x++)
        {
            int matchLength = 1;
            for (int y = 1; y < Height; y++)
            {
                Tile current = _grid[x, y];
                Tile previous = _grid[x, y - 1];

                // Only count a match if both tiles are not null and they have the same ID
                if (current != null && previous != null && current == previous)
                {
                    matchLength++;
                }
                else
                {
                    if (matchLength >= 3)
                    {
                        AddVerticalMatch(matches, x, y - 1, matchLength);
                    }
                    matchLength = 1;
                }
            }
            // end of column
            if (matchLength >= 3)
            {
                AddVerticalMatch(matches, x, Height - 1, matchLength);
            }
        }
        return matches;
    }

    private void AddHorizontalMatch(HashSet<Vector2Int> matches, int endX, int y, int length)
    {
        for (int i = 0; i < length; i++)
        {
            matches.Add(new Vector2Int(endX - i, y));
        }
    }

    private void AddVerticalMatch(HashSet<Vector2Int> matches, int x, int endY, int length)
    {
        for (int i = 0; i < length; i++)
        {
            matches.Add(new Vector2Int(x, endY - i));
        }
    }

    public void ClearMatches(HashSet<Vector2Int> matches)
    {
        foreach (var match in matches)
        {
            _grid[match.x, match.y] = null;
        }
    }

    public void CollapseColumns()
    {
        for (int x = 0; x < Width; x++)
        {
            int writeIndex = Height - 1;
            
            for (int y = Height - 1; y >= 0; y--)
            {
                if (_grid[x, y] == null)
                    continue;

                // If we're not at the write position, move the tile down
                if (y != writeIndex)
                {
                    _grid[x, writeIndex] = _grid[x, y];
                    _grid[x, y] = null;
                }
                writeIndex--;
            }
        }
    }

    public void Refill()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_grid[x, y] == null)
                {
                    _grid[x, y] = new Tile { id = Random.Range(1, 4) };
                }
            }
        }
    }
}
