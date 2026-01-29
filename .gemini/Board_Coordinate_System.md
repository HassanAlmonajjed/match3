# Board Coordinate System Visualization

## Grid Layout

The Board uses a coordinate system where:
- **X-axis**: Columns (0 to Width-1, left to right)
- **Y-axis**: Rows (0 to Height-1, bottom to top)

```
Visual representation of a 5x5 board:

       Column: 0    1    2    3    4
              ┌────┬────┬────┬────┬────┐
Row 4 (top)   │    │    │    │    │    │  y=4
              ├────┼────┼────┼────┼────┤
Row 3         │    │    │    │    │    │  y=3
              ├────┼────┼────┼────┼────┤
Row 2         │    │    │    │    │    │  y=2
              ├────┼────┼────┼────┼────┤
Row 1         │    │    │    │    │    │  y=1
              ├────┼────┼────┼────┼────┤
Row 0 (bottom)│    │    │    │    │    │  y=0
              └────┴────┴────┴────┴────┘
               x=0  x=1  x=2  x=3  x=4
```

## Gravity Direction

Tiles fall **DOWN** from higher y-values to lower y-values:

```
Before Collapse:          After Collapse:

y=4  │ 4  │    │    │    │    │      │    │    │    │    │    │
     ├────┼────┼────┼────┼────┤      ├────┼────┼────┼────┼────┤
y=3  │ 3  │    │    │    │    │      │ 4  │    │    │    │    │
     ├────┼────┼────┼────┼────┤      ├────┼────┼────┼────┼────┤
y=2  │ X  │    │    │    │    │  =>  │ 3  │    │    │    │    │
     ├────┼────┼────┼────┼────┤      ├────┼────┼────┼────┼────┤
y=1  │ 2  │    │    │    │    │      │ 2  │    │    │    │    │
     ├────┼────┼────┼────┼────┤      ├────┼────┼────┼────┼────┤
y=0  │ 1  │    │    │    │    │      │ 1  │    │    │    │    │
     └────┴────┴────┴────┴────┘      └────┴────┴────┴────┴────┘
      x=0                              x=0

(X = cleared/empty tile)
```

## Test Grid Input Format

When tests create grids using `int[,]`, they use `[row, column]` indexing:

```csharp
var gridIds = new int[,]
{
    {1, 0, 0, 0, 0},  // Row 0 (y=0, bottom) - tile 1 at column 0
    {2, 0, 0, 0, 0},  // Row 1 (y=1)        - tile 2 at column 0
    {0, 0, 0, 0, 0},  // Row 2 (y=2)        - empty
    {3, 0, 0, 0, 0},  // Row 3 (y=3)        - tile 3 at column 0
    {4, 0, 0, 0, 0}   // Row 4 (y=4, top)   - tile 4 at column 0
};
```

This maps to the internal `_grid[x, y]` as:
- `gridIds[0, 0]` (row 0, col 0) → `_grid[0, 0]` (x=0, y=0)
- `gridIds[1, 0]` (row 1, col 0) → `_grid[0, 1]` (x=0, y=1)
- `gridIds[2, 0]` (row 2, col 0) → `_grid[0, 2]` (x=0, y=2)
- etc.

## CollapseColumns Algorithm

The algorithm works as follows for each column:

1. Start with `writeIndex = 0` (bottom position)
2. Scan from y=0 to y=Height-1 (bottom to top)
3. For each non-null tile found:
   - Move it to position `writeIndex`
   - Increment `writeIndex`
4. All tiles above `writeIndex` become null

Example for column 0:

```
Initial state:     After scan:
y=4: tile 4        y=4: null
y=3: tile 3        y=3: tile 4  (writeIndex=4)
y=2: null          y=2: tile 3  (writeIndex=3)
y=1: tile 2        y=1: tile 2  (writeIndex=2)
y=0: tile 1        y=0: tile 1  (writeIndex=1)
```

All tiles are compacted to the bottom with no gaps.
