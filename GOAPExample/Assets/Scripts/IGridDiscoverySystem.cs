using System.Collections.Generic;
using UnityEngine;

public interface IGridDiscoverySystem : ISystem
{
    IReadOnlyCollection<Vector2Int> DiscoveredBreakableBlockLocations { get; }

    void RemoveDiscoveredLocation(Vector2Int location);
}