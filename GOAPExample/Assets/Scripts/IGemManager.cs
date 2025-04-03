using System;
using UnityEngine;

public interface IGemManager
{
    event Action<Vector2Int> GemRemoved;

    void Initialize();
    bool RemoveGem(Vector2Int index);
    bool TryGetNearbyGemIndex(Vector2Int location, float range, out Vector2Int[] gemLocation);
}
