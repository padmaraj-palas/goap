using System.Linq;
using UnityEngine;

public sealed class BreakBlockStrategy : IActionStrategy
{
    private readonly IAgent mAgent;
    private readonly IGridDiscoverySystem mGridDiscoverySystem;
    private readonly IGridManager mGridManager;

    public BreakBlockStrategy(IAgent agent, IGridDiscoverySystem gridDiscoverySystem, IGridManager gridManager)
    {
        mAgent = agent;
        mGridDiscoverySystem = gridDiscoverySystem;
        mGridManager = gridManager;
    }

    public bool CanPerform => mGridDiscoverySystem.DiscoveredBreakableBlockLocations.Any(index => Vector2Int.Distance(index, mAgent.Position.ToIndex2D(mGridManager.GridSize)) <= 1.1f);

    public bool Completed => true;

    public void Start()
    {
        foreach (var index in mGridDiscoverySystem.DiscoveredBreakableBlockLocations)
        {
            if (Vector2Int.Distance(index, mAgent.Position.ToIndex2D(mGridManager.GridSize)) <= 1.1f)
            {
                mGridDiscoverySystem.RemoveDiscoveredLocation(index);
                mGridManager.BreakBlock(index);
                return;
            }
        }
    }

    public void Stop()
    { }

    public void Update(float deltaTime)
    { }
}
