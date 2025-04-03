using UnityEngine;

public interface IAgent
{
    Vector3 Position { get; }

    void Initialize(
        AgentStats agentStats,
        IGemDiscoverySystem gemDiscoverySystem,
        IGemManager gemManager,
        IGridDiscoverySystem gridDiscoverySystem,
        IGridManager gridManager);
    void OnGemCollected(Vector2Int index);
}