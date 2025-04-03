using System.Collections.Generic;
using UnityEngine;

public interface IGemDiscoverySystem : ISystem
{
    IReadOnlyCollection<Vector2Int> DiscoveredGemLocations { get; }

    void RemoveLocation(Vector2Int gemLocation);
}
