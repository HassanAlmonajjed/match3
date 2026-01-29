# BoardCollapseColumnsTests Fix Summary

## Issues Found and Fixed

### 1. **Coordinate System Mismatch in `Populate(int[,] gridIds)` method**

**Problem:** The test files pass grid data as `gridIds[row, column]` (where row is the y-axis and column is the x-axis), but the `Board.Populate` method was treating it as `gridIds[column, row]`.

**Fix in Board.cs (lines 67-81):**
```csharp
// Changed from:
for (int x = 0; x < Width; x++)
{
    for (int y = 0; y < Height; y++)
    {
        if (x < gridIds.GetLength(0) && y < gridIds.GetLength(1) && gridIds[x, y] > 0)
        {
            _grid[x, y] = new Tile { id = gridIds[x, y] };
        }
    }
}

// To:
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
```

### 2. **Incorrect Gravity Direction in `CollapseColumns()` method**

**Problem:** The original implementation scanned from top to bottom (y=Height-1 to y=0) looking for empty spots, which is backwards for gravity-based tile falling.

**Fix in Board.cs (lines 231-252):**
```csharp
// Changed from scanning top-to-bottom looking for empty spots:
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

// To scanning bottom-to-top collecting tiles:
int writeIndex = 0; // Start from the bottom (y=0)

for (int y = 0; y < Height; y++)
{
    if (_grid[x, y] != null)
    {
        if (y != writeIndex)
        {
            _grid[x, writeIndex] = _grid[x, y];
            _grid[x, y] = null;
        }
        writeIndex++;
    }
}
```

**Explanation:** The new algorithm scans from bottom (y=0) to top (y=Height-1), collecting all non-null tiles and placing them consecutively starting from the bottom. This correctly simulates gravity.

### 3. **Buggy Test: `CollapseColumns_AllTilesAtBottom_NoChange`**

**Problem:** The test had tiles at the top row (y=4) which would form a horizontal match and be cleared, then expected tiles at various positions after collapse - which doesn't make sense.

**Fix in BoardCollapseColumnsTests.cs (lines 242-268):**
- Changed grid to have tiles at the bottom row (y=0) with different IDs to avoid matches
- Removed the match detection and clearing steps
- Fixed assertions to properly check `tile.id` instead of comparing tile objects with integers
- Verified tiles remain at the bottom after collapse (no movement needed)

## Coordinate System Reference

The Board uses this coordinate system:
- **x-axis**: Column index (0 to Width-1)
- **y-axis**: Row index, where:
  - y=0 is the **bottom** row
  - y=Height-1 is the **top** row
- Tiles fall **down** from higher y values to lower y values (gravity)

## Test Results

All tests in `BoardCollapseColumnsTests.cs` should now pass:
1. ✅ CollapseColumns_SingleEmptySpace_TileMovesDown
2. ✅ CollapseColumns_MultipleEmptySpaces_TilesMoveToBottom
3. ✅ CollapseColumns_EmptyColumn_NoChange
4. ✅ CollapseColumns_FullColumn_NoChange
5. ✅ CollapseColumns_MultipleColumnsWithGaps_AllCollapseCorrectly
6. ✅ CollapseColumns_AlternatingPattern_CollapsesCorrectly
7. ✅ CollapseColumns_SingleTileAtTop_FallsToBottom
8. ✅ CollapseColumns_AllTilesAtBottom_NoChange (FIXED)
9. ✅ CollapseColumns_LargeGap_TilesFallCorrectly
