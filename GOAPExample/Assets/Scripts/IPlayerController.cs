using UnityEngine;

public interface IPlayerController
{
    void Initialize(AgentStats agentStatsPrefab, Camera camera, Transform uiRoot);
}
