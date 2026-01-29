# Top-Left Origin Coordinate System Update

## Overview

Updated both `Board.cs` and `BoardController.cs` to use a **top-left origin** coordinate system where:
- **(0, 0)** is at the **top-left** corner
- **X-axis**: increases from left to right (0 to Width-1)
- **Y-axis**: increases from top to bottom (0 to Height-1)
- **Gravity**: tiles fall **downward** (from lower y-values to higher y-values)

This is a more intuitive coordinate system for 2D games and matches common UI frameworks.

---

## Visual Representation

```
        X-AXIS →
       0    1    2    3    4
    ┌────┬────┬────┬────┬────┐
  0 │    │    │    │    │    │  ← TOP (y=0)
Y   ├────┼────┼────┼────┼────┤
│ 1 │    │    │    │    │    │
A   ├────┼────┼────┼────┼────┤
X 2 │    │    │    │    │    │
I   ├────┼────┼────┼────┼────┤
S 3 │    │    │    │    │    │
↓   ├────┼────┼────┼────┼────┤
  4 │    │    │    │    │    │  ← BOTTOM (y=4)
    └────┴────┴────┴────┴────┘

Origin: (0,0) at TOP-LEFT
Gravity: Tiles fall DOWN (y increases)
```

---

## Changes to Board.cs

### CollapseColumns() Method (Lines 231-252)

**Before (bottom-left origin):**
```csharp
public void CollapseColumns()
{
    for (int x = 0; x < Width; x++)
    {
        int writeIndex = 0; // Start from the bottom (y=0)
        
        // Scan from bottom to top, collecting non-null tiles
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
    }
}
```

**After (top-left origin):**
```csharp
public void CollapseColumns()
{
    for (int x = 0; x < Width; x++)
    {
        int writeIndex = Height - 1; // Start from the bottom (highest y value)
        
        // Scan from bottom to top, collecting non-null tiles
        for (int y = Height - 1; y >= 0; y--)
        {
            if (_grid[x, y] != null)
            {
                if (y != writeIndex)
                {
                    _grid[x, writeIndex] = _grid[x, y];
                    _grid[x, y] = null;
                }
                writeIndex--;
            }
        }
    }
}
```

**Key Changes:**
- `writeIndex` starts at `Height - 1` (bottom) instead of `0`
- Loop scans from `Height - 1` down to `0` (bottom to top)
- `writeIndex` decrements instead of increments
- Tiles fall to higher y-values (downward)

---

## Changes to BoardController.cs

### 1. GenerateBoard() Method (Line 49)

**Before:**
```csharp
// y=0 is at bottom, y increases upward
Vector3 position = transform.position + new Vector3(x * (1f + gap), y * (1f + gap), 0);
```

**After:**
```csharp
// y=0 is at top, y increases downward
Vector3 position = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);
```

---

### 2. Collapse() Method (Line 173)

**Before:**
```csharp
Vector3 newPos = transform.position + new Vector3(x * (1f + gap), y * (1f + gap), 0);
```

**After:**
```csharp
// y=0 is at top, so use -y for visual position
Vector3 newPos = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);
```

---

### 3. RefillAndAnimate() Method (Lines 204-206)

**Before:**
```csharp
// Spawn above the board (higher y value)
Vector3 spawnPos = transform.position + new Vector3(x * (1f + gap), (board.Height + 1) * (1f + gap), 0);
Vector3 finalPos = transform.position + new Vector3(x * (1f + gap), y * (1f + gap), 0);
```

**After:**
```csharp
// Spawn above the board (negative y value, above y=0)
Vector3 spawnPos = transform.position + new Vector3(x * (1f + gap), (1f + gap), 0);
// y=0 is at top, so use -y for visual position
Vector3 finalPos = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);
```

**Key Changes:**
- Spawn position is now at `+1f` (above y=0, the top)
- All visual positions use `-y` to map logical coordinates to screen space

---

## Coordinate Mapping

### Logical to Visual Mapping:
```
Logical Board:          Visual Screen:
(0,0) → top-left       transform.position + (0, 0)
(0,1) → one down       transform.position + (0, -1)
(0,2) → two down       transform.position + (0, -2)
(4,4) → bottom-right   transform.position + (4, -4)
```

### Gravity Behavior:
```
Before Collapse:        After Collapse:
y=0  │ 1  │    │       y=0  │    │    │
y=1  │ X  │    │  =>   y=1  │    │    │
y=2  │ 2  │    │       y=2  │ 1  │    │
y=3  │ 3  │    │       y=3  │ 2  │    │
y=4  │    │    │       y=4  │ 3  │    │

Tiles fall DOWN (to higher y-values)
```

---

## Impact on Tests

**IMPORTANT:** All existing tests were written for the **bottom-left origin** system. With this change to **top-left origin**, the tests will need to be updated to reflect:

1. **Coordinate expectations**: What was at (0,0) bottom-left is now at (0,4) bottom-left
2. **Gravity direction**: Tiles now fall from lower y to higher y
3. **Grid input interpretation**: The first row in `gridIds[0, ...]` is now the TOP row, not the bottom

### Example Test Update Needed:

**Old (bottom-left origin):**
```csharp
var gridIds = new int[,]
{
    {1, 0, 0, 0, 0},  // Row 0 (bottom)
    {2, 0, 0, 0, 0},  // Row 1
    {0, 0, 0, 0, 0},  // Row 2
    {3, 0, 0, 0, 0},  // Row 3
    {4, 0, 0, 0, 0}   // Row 4 (top)
};
```

**New (top-left origin):**
```csharp
var gridIds = new int[,]
{
    {4, 0, 0, 0, 0},  // Row 0 (top)
    {3, 0, 0, 0, 0},  // Row 1
    {0, 0, 0, 0, 0},  // Row 2
    {2, 0, 0, 0, 0},  // Row 3
    {1, 0, 0, 0, 0}   // Row 4 (bottom)
};
```

---

## Summary

✅ **Board.cs**: CollapseColumns now moves tiles to higher y-values (downward)
✅ **BoardController.cs**: All visual positions use `-y` to map top-left logical coordinates to screen space
✅ **Coordinate System**: (0,0) is now at top-left, matching common 2D game conventions
✅ **Gravity**: Tiles fall downward (from lower y to higher y)

⚠️ **Next Step**: Update all test files to work with the new top-left origin coordinate system!
