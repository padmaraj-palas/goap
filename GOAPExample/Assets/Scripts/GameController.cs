using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private AgentStats _agentStatsPrefab;
    [SerializeField] private Camera _camera;
    [SerializeField] private GemManager _gemManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _uiRoot;

    private void Start()
    {
        _gridManager.Initialize();
        _gemManager.Initialize();
        _playerController.Initialize(_agentStatsPrefab, _camera, _uiRoot);
    }
}
