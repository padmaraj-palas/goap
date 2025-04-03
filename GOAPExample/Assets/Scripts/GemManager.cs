using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GemManager : MonoBehaviour, IGemManager
{
    private readonly IDictionary<Vector2Int, GameObject> mGems = new Dictionary<Vector2Int, GameObject>();
    private readonly IDictionary<Vector2Int, GameObject> mHiddenGems = new Dictionary<Vector2Int, GameObject>();

    [SerializeField] private int _gemCount;
    [SerializeField] private GameObject _gemPrefab;
    [SerializeField] private GridManager _gridManager;

    public event System.Action<Vector2Int> GemRemoved;

    public void Initialize()
    {
        var availableCells = new List<Vector2Int>(_gridManager.BreakableCells);
        for (int i = 0; i < _gemCount; i++)
        {
            var index = availableCells.ElementAt(Random.Range(0, availableCells.Count));
            availableCells.Remove(index);
            var gem = Instantiate(_gemPrefab, index.ToPosition(_gridManager.GridSize), Quaternion.identity);
            gem.SetActive(false);
            mHiddenGems.Add(index, gem);
        }

        _gridManager.BlockBroke += OnBlockBroke;
    }

    private void OnBlockBroke(Vector2Int index)
    {
        if (mHiddenGems.TryGetValue(index, out var gem))
        {
            mHiddenGems.Remove(index);
            mGems[index] = gem;
            gem.SetActive(true);
        }
    }

    public bool RemoveGem(Vector2Int index)
    {
        if (mGems.TryGetValue(index, out var gem))
        {
            mGems.Remove(index);
            Destroy(gem);
            GemRemoved?.Invoke(index);
            return true;
        }

        return false;
    }

    public bool TryGetNearbyGemIndex(Vector2Int location, float range, out Vector2Int[] gemLocation)
    {
        if (mGems.Count == 0)
        {
            gemLocation = default;
            return false;
        }
        
        try
        {
            var gems = mGems.Keys.Select(index => (index, (location - index).magnitude)).Where(t => t.magnitude <= range).OrderBy(t => t.magnitude).Select(t => t.index);
            gemLocation = gems.ToArray();
        }
        catch
        {
            gemLocation = default;
        }
        
        return gemLocation?.Length > 0;
    }
}