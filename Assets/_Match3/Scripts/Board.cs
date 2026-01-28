using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private Tile[,] _grid;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Board(int width, int height)
    {
        _grid = new Tile[width, height];
        Width = width;
        Height = height;
    }

    public Tile GetTileAtPosition(Vector2Int position)
    {
        // implement it
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
        if (neighborPos.x < 0 || neighborPos.x >= Width || neighborPos.y < 0 || neighborPos.y >= Height)
            return new Vector2Int(-1, -1);

        return neighborPos;
    }

    public void Populate()
    {
        _grid = new Tile[,]
        {
            { new Tile { id = 1 }, new Tile { id = 2 }, new Tile { id = 2 }, new Tile { id = 3 }, new Tile { id = 1 } },
            { new Tile { id = 1 }, new Tile { id = 2 }, new Tile { id = 1 }, new Tile { id = 1 }, new Tile { id = 2 } },
            { new Tile { id = 2 }, new Tile { id = 1 }, new Tile { id = 2 }, new Tile { id = 2 }, new Tile { id = 3 } },
            { new Tile { id = 3 }, new Tile { id = 2 }, new Tile { id = 3 }, new Tile { id = 2 }, new Tile { id = 1 } },
            { new Tile { id = 1 }, new Tile { id = 3 }, new Tile { id = 3 }, new Tile { id = 1 }, new Tile { id = 2 } }
        };
        /*for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                _grid[x, y] = new Tile { id = Random.Range(1, 4) };
            }
        }*/
    }

    public void Swipe(Vector2Int swipeStart, Vector2Int swipeEnd)
    {
        (_grid[swipeEnd.x, swipeEnd.y], _grid[swipeStart.x, swipeStart.y]) = (_grid[swipeStart.x, swipeStart.y], _grid[swipeEnd.x, swipeEnd.y]);
    }

    public void Print()
    {
        string grid = "";
        for (int y = 0; y < _grid.GetLength(1); y++)
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                grid += _grid[x, y] == null ? "X" : _grid[x, y].ToString() + " ";
            }
            grid += "\n";
        }
        Debug.Log(grid);
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
                if (current == previous)
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
                if (current == previous)
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
        foreach (var pos in matches)
        {
            _grid[pos.x, pos.y] = null;
        }
    }

    public void CollapseColumns()
    {
        for (int x = 0; x < Width; x++)
        {
            int emptySpot = -1;
            for (int y = Height - 1; y >= 0; y--)
            {
                if (_grid[x, y] == null && emptySpot == -1)
                {
                    emptySpot = y;
                }
                else if (_grid[x, y] != null && emptySpot != -1)
                {
                    _grid[x, emptySpot] = _grid[x, y];
                    _grid[x, y] = null;
                    emptySpot--;
                }
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
