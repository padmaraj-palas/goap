using System;
using UnityEngine;
using UnityEngine.AI;

public sealed class MoveToLocationStrategy : IActionStrategy
{
    private readonly IGridManager mGridManager;
    private readonly Func<Vector2Int> mLocationFunc;
    private readonly NavMeshAgent mNavMeshAgent;

    private Vector3 mTargetLocation;

    public MoveToLocationStrategy(IGridManager gridManager, Func<Vector2Int> locationFunc, NavMeshAgent navMeshAgent)
    {
        mGridManager = gridManager;
        mLocationFunc = locationFunc;
        mNavMeshAgent = navMeshAgent;
    }

    public bool CanPerform => true;

    public bool Completed => !mNavMeshAgent.hasPath && !mNavMeshAgent.pathPending;

    public void Start()
    {
        if (mLocationFunc == null)
            return;

        var location = mLocationFunc();
        mTargetLocation = location.ToPosition(mGridManager.GridSize);
        mNavMeshAgent.SetDestination(mTargetLocation);
    }

    public void Stop()
    { }

    public void Update(float deltaTime)
    { }
}
