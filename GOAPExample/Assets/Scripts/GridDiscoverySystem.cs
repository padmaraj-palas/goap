using System.Collections.Generic;
using UnityEngine;

public sealed class GridDiscoverySystem : IGridDiscoverySystem
{
    private readonly IAgent mAgent;
    private readonly List<Vector2Int> mDiscoveredBreakableBlockLocations;
    private readonly IGridManager mGridManager;

    public GridDiscoverySystem(IAgent agent, IGridManager gridManager)
    {
        mAgent = agent;
        mDiscoveredBreakableBlockLocations = new List<Vector2Int>();
        mGridManager = gridManager;

        mGridManager.BlockBroke += OnBlockBroke;
    }

    public IReadOnlyCollection<Vector2Int> DiscoveredBreakableBlockLocations => mDiscoveredBreakableBlockLocations;

    public void RemoveDiscoveredLocation(Vector2Int location)
    {
        mDiscoveredBreakableBlockLocations.Remove(location);
    }

    public void Update()
    {
        if (mGridManager.TryGetNearbyBreakableIndex(mAgent.Position.ToIndex2D(mGridManager.GridSize), 3, out var index))
        {
            foreach (var location in index)
            {
                if (!mDiscoveredBreakableBlockLocations.Contains(location))
                    mDiscoveredBreakableBlockLocations.Add(location);
            }
        }
    }

    private void OnBlockBroke(Vector2Int index)
    {
        mDiscoveredBreakableBlockLocations.Remove(index);
    }
}
