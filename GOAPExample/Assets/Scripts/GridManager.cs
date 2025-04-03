using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GridManager : MonoBehaviour, IGridManager
{
    private readonly IList<Vector2Int> mAvailableCells = new List<Vector2Int>();
    private readonly IDictionary<Vector2Int, GameObject> mBreakableBlocks = new Dictionary<Vector2Int, GameObject>();

    [SerializeField] private GameObject _breakableBlock;
    [SerializeField] private Vector2Int _gridSize;
    
    public ICollection<Vector2Int> AvailableCells => mAvailableCells;
    public ICollection<Vector2Int> BreakableCells => mBreakableBlocks.Keys;
    public Vector2Int GridSize => _gridSize;

    public event System.Action<Vector2Int> BlockBroke;

    public bool BreakBlock(Vector2Int index)
    {
        if (mBreakableBlocks.TryGetValue(index, out var block))
        {
            mBreakableBlocks.Remove(index);
            if (!mAvailableCells.Contains(index))
            {
                mAvailableCells.Add(index);
            }

            Destroy(block);
            BlockBroke?.Invoke(index);
            return true;
        }

        return false;
    }

    public void Initialize()
    {
        for (int y = 1; y < _gridSize.y - 1; y++)
        {
            for (int x = 1; x < _gridSize.x - 1; x++)
            {
                var index = new Vector2Int(x, y);
                if (index.IsStaticBlock(_gridSize))
                {
                    continue;
                }

                bool generateBrakable = Random.Range(1, 21) % 2 == 0;
                if (!generateBrakable)
                {
                    mAvailableCells.Add(index);
                    continue;
                }
                
                var block = Instantiate(_breakableBlock, index.ToPosition(_gridSize), Quaternion.identity);
                mBreakableBlocks.Add(index, block);
            }
        }
    }

    public bool IsNavigatable(Vector2Int index)
    {
        return !index.IsStaticBlock(_gridSize) && !mBreakableBlocks.ContainsKey(index);
    }

    public bool TryGetNearbyBreakableIndex(Vector2Int location, float range, out Vector2Int[] breakableIndex)
    {
        if (mBreakableBlocks.Count == 0)
        {
            breakableIndex = default;
            return false;
        }
        
        try
        {
            var breakables = mBreakableBlocks.Keys.Select(index => (index, (location - index).magnitude)).Where(t => t.magnitude <= range).OrderBy(t => t.magnitude).Select(t => t.index);
            breakableIndex = breakables.ToArray();
        }
        catch
        {
            breakableIndex = default;
        }

        return breakableIndex?.Length > 0;
    }

    public bool TryGetNearbyCellIndexes(Vector2Int location, float range, out Vector2Int[] indexes)
    {
        if (mAvailableCells.Count == 0)
        {
            indexes = null;
            return false;
        }
        
        indexes = mAvailableCells.Where(index => (location - index).magnitude <= range).ToArray();
        return indexes != null && indexes.Length > 0;
    }
}
