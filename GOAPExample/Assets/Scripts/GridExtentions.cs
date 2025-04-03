using UnityEngine;

public static class GridExtentions
{
    public static bool IsStaticBlock(this Vector2Int index2D, Vector2Int size)
    {
        if (size.magnitude == 0)
        {
            return false;
        }

        return (index2D.x == 0 || index2D.x == (size.x - 1) || index2D.y == 0 || index2D.y == (size.y - 1)) || index2D.x % 2 == 0 && index2D.y % 2 == 0;
    }

    public static int ToIndex1D(this Vector2Int index2D, Vector2Int size)
    {
        return index2D.x + index2D.y * size.x;
    }

    public static Vector2Int ToIndex2D(this int index1D, Vector2Int size)
    {
        return new Vector2Int(index1D % size.x, index1D / size.y);
    }

    public static Vector2Int ToIndex2D(this Vector3 position, Vector2Int size)
    {
        var correctedPosition = position + Vector3.right * size.x * 0.5f + Vector3.forward * size.y * 0.5f;
        return new Vector2Int((int)correctedPosition.x, (int)correctedPosition.z);
    }

    public static Vector3 ToPosition(this int index1D, Vector2Int gridSize, float cellSize = 1)
    {
        return ToPosition(index1D.ToIndex2D(gridSize), gridSize, cellSize);
    }

    public static Vector3 ToPosition(this Vector2Int index2D, Vector2Int gridSize, float cellSize = 1)
    {
        return new Vector3((cellSize - gridSize.x * cellSize) * 0.5f + index2D.x * cellSize, 0, (cellSize - gridSize.y * cellSize) * 0.5f + index2D.y * cellSize);
    }
}