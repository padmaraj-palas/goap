using System;
using UnityEngine;

public sealed class CollectGemStrategy : IActionStrategy
{
    private readonly IAgent mAgent;
    private readonly IGemDiscoverySystem mGemDiscoverySystem;
    private readonly Func<Vector2Int> mGemLocationFunc;
    private readonly IGemManager mGemManager;

    public CollectGemStrategy(IAgent agent,
        IGemDiscoverySystem gemDiscoverySystem,
        Func<Vector2Int> gemLocationFunc,
        IGemManager gemManager)
    {
        mAgent = agent;
        mGemDiscoverySystem = gemDiscoverySystem;
        mGemLocationFunc = gemLocationFunc;
        mGemManager = gemManager;
    }

    public bool CanPerform => mGemLocationFunc != null && mGemManager.TryGetNearbyGemIndex(mGemLocationFunc.Invoke(), 0.2f, out _);

    public bool Completed => true;

    public void Start()
    {
        var location = mGemLocationFunc.Invoke();
        mGemDiscoverySystem.RemoveLocation(location);
        if (mGemManager.RemoveGem(location))
        {
            mAgent.OnGemCollected(location);
        }
    }

    public void Stop()
    { }

    public void Update(float deltaTime)
    { }
}
