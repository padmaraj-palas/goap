using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGridManager
{
    ICollection<Vector2Int> AvailableCells { get; }
    ICollection<Vector2Int> BreakableCells { get; }
    Vector2Int GridSize { get; }

    event Action<Vector2Int> BlockBroke;

    bool BreakBlock(Vector2Int index);
    void Initialize();
    bool IsNavigatable(Vector2Int index);
    bool TryGetNearbyBreakableIndex(Vector2Int location, float range, out Vector2Int[] breakableIndex);
    bool TryGetNearbyCellIndexes(Vector2Int location, float range, out Vector2Int[] indexes);
}
