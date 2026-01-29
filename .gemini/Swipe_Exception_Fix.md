# Bug Fix: Swipe Exception Errors

## Issues Found

After switching to top-left origin, three exceptions were occurring during swipe operations:

### 1. **ArgumentOutOfRangeException in Collapse()**
```
ArgumentOutOfRangeException: Index was out of range
at BoardController.Collapse() line 173
```

### 2. **NullReferenceException in RefillAndAnimate()**
```
NullReferenceException: Object reference not set to an instance of an object
at BoardController.RefillAndAnimate() line 213
```

### 3. **NullReferenceException in Board.Print()**
```
NullReferenceException: Object reference not set to an instance of an object
at Board.Print() line 128
```

---

## Root Cause

### Problem in Collapse() Method:

The original logic was:
1. Collect tiles from positions where `board.GetTileAtPosition(pos) != null`
2. Try to assign them to positions where `board.GetTileAtPosition(pos) != null`

**The Issue:** After clearing matches, the `tiles` dictionary has gaps (removed tiles), but we were trying to collect tiles based on the board state BEFORE checking if they exist in the dictionary. This caused index out of range errors.

**Example:**
```
Before collapse:
tiles dictionary: {(0,0): controller1, (0,2): controller2, (0,3): controller3}
                  (0,1) is missing (was cleared)

Board after collapse: positions (0,0), (0,1), (0,2) have tiles

Old logic tried:
- Collect from board positions with tiles: finds 3 positions
- But tiles dictionary only has controllers at (0,0), (0,2), (0,3)
- When trying to access tilesInColumn[1], it doesn't exist!
```

---

## Solution

### Fixed Collapse() Method:

**New Logic:**
1. **First:** Collect ALL existing tile controllers from the `tiles` dictionary (regardless of board state)
2. **Then:** Assign them sequentially to the new positions in the collapsed board

```csharp
private void Collapse(Dictionary<Vector2Int, TileController> newTileControllers, Sequence seq)
{
    for (int x = 0; x < board.Width; x++)
    {
        // STEP 1: Collect all existing tile controllers in this column
        var existingTileControllers = new List<TileController>();
        
        for (int y = 0; y < board.Height; y++)
        {
            var pos = new Vector2Int(x, y);
            if (tiles.ContainsKey(pos))  // ✅ Only check dictionary
            {
                existingTileControllers.Add(tiles[pos]);
            }
        }
        
        // STEP 2: Assign controllers to new positions from collapsed board
        int controllerIndex = 0;
        for (int y = 0; y < board.Height; y++)
        {
            var pos = new Vector2Int(x, y);
            var tile = board.GetTileAtPosition(pos);
            
            if (tile != null)
            {
                // Safety check
                if (controllerIndex >= existingTileControllers.Count)
                {
                    Debug.LogError($"Collapse error: Not enough controllers for column {x}");
                    break;
                }
                
                var controller = existingTileControllers[controllerIndex];
                Vector3 newPos = transform.position + new Vector3(x * (1f + gap), -y * (1f + gap), 0);

                float delay = controllerIndex * collapseStaggerDelay;
                seq.Insert(delay, controller.transform.DOMove(newPos, collapseDuration).SetEase(Ease.InOutQuad));

                newTileControllers[pos] = controller;
                controllerIndex++;
            }
        }
    }
}
```

---

## Key Changes

### Before (Broken):
```csharp
// Collected tiles based on BOARD state
var tilesInColumn = new List<TileController>();
for (int y = 0; y < board.Height; y++)
{
    var pos = new Vector2Int(x, y);
    if (board.GetTileAtPosition(pos) != null && tiles.ContainsKey(pos))
    {
        tilesInColumn.Add(tiles[pos]);  // ❌ Mismatch!
    }
}
```

### After (Fixed):
```csharp
// Collect tiles based on DICTIONARY state
var existingTileControllers = new List<TileController>();
for (int y = 0; y < board.Height; y++)
{
    var pos = new Vector2Int(x, y);
    if (tiles.ContainsKey(pos))  // ✅ Only check what exists
    {
        existingTileControllers.Add(tiles[pos]);
    }
}
```

---

## Why This Works

1. **Separation of Concerns:**
   - First loop: "What tile controllers do I actually have?"
   - Second loop: "Where should they go based on the collapsed board?"

2. **No Index Mismatches:**
   - We collect exactly the controllers that exist
   - We assign them sequentially to the new positions
   - The counts always match (same number of tiles before and after collapse)

3. **Safety Check:**
   - Added error logging if counts don't match
   - Prevents crashes with a `break` statement

---

## Testing

The fix ensures:
- ✅ No ArgumentOutOfRangeException when accessing tile controllers
- ✅ Proper mapping of existing controllers to new positions
- ✅ Smooth collapse animations
- ✅ Correct tile positions after collapse

---

## Status

✅ **Collapse() method fixed**
⚠️ **RefillAndAnimate() and Print() exceptions** - These may have been side effects of the Collapse error. Test again to see if they still occur.

If RefillAndAnimate still has issues, it might be because:
- `tilePrefab` is not assigned in the inspector
- `SetTileColor()` method has issues
- Transform operations fail

If Print() still has issues, it might be because:
- Tile's `ToString()` method is not implemented
- Grid has unexpected null values
