using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class PlayerController : MonoBehaviour, IPlayerController
{
    private readonly IList<ISystem> mSystems = new List<ISystem>();

    [SerializeField] private GemManager _gemManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private int _playerCount;
    [SerializeField] private Agent _playerPrefab;

    public void Initialize(AgentStats agentStatsPrefab, Camera camera, Transform uiRoot)
    {
        var availableCells = new List<Vector2Int>(_gridManager.AvailableCells);
        for (int i = 0; i < _playerCount; i++)
        {
            var index = availableCells.ElementAt(Random.Range(0, availableCells.Count));
            availableCells.Remove(index);
            var player = Instantiate(_playerPrefab, index.ToPosition(_gridManager.GridSize), Quaternion.identity);
            var agentStats = Instantiate(agentStatsPrefab, uiRoot, false);
            agentStats.Initialize(player, camera);
            var gemDiscoverySystem = new GemDiscoverySystem(player, _gemManager, _gridManager);
            var gridDiscoverySystem = new GridDiscoverySystem(player, _gridManager);
            mSystems.Add(gemDiscoverySystem);
            mSystems.Add(gridDiscoverySystem);
            player.Initialize(agentStats, gemDiscoverySystem, _gemManager, gridDiscoverySystem, _gridManager);
        }
    }

    private void Update()
    {
        if (mSystems != null && mSystems.Count > 0)
        {
            foreach (var system in mSystems)
            {
                system?.Update();
            }
        }
    }
}