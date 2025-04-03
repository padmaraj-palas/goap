using System.Collections.Generic;
using UnityEngine;

public sealed class GemDiscoverySystem : IGemDiscoverySystem
{
    private readonly IAgent mAgent;
    private readonly List<Vector2Int> mDiscoveredGemLocations;
    private readonly IGemManager mGemManager;
    private readonly IGridManager mGridManager;

    public GemDiscoverySystem(IAgent agent, IGemManager gemManager, IGridManager gridManager)
    {
        mAgent = agent;
        mDiscoveredGemLocations = new List<Vector2Int>();
        mGemManager = gemManager;
        mGridManager = gridManager;

        mGemManager.GemRemoved += OnGemRemoved;
    }

    public IReadOnlyCollection<Vector2Int> DiscoveredGemLocations => mDiscoveredGemLocations;

    public void RemoveLocation(Vector2Int gemLocation)
    {
        mDiscoveredGemLocations.Remove(gemLocation);
    }

    public void Update()
    {
        if (mGemManager.TryGetNearbyGemIndex(mAgent.Position.ToIndex2D(mGridManager.GridSize), 3, out var indexes))
        {
            foreach (var location in indexes)
            {
                if (!mDiscoveredGemLocations.Contains(location))
                    mDiscoveredGemLocations.Add(location);
            }
        }
    }

    private void OnGemRemoved(Vector2Int index)
    {
        mDiscoveredGemLocations.Remove(index);
    }
}
