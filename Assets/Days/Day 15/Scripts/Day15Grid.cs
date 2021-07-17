using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15Grid
{
    public Day15GameTile[,] Grid;

    public Day15Grid(int x, int y)
    {
        Grid = new Day15GameTile[x, y];
    }

    public void ClearUnit(Vector2Int pos)
    {
        Grid[pos.x, pos.y].hasUnit = false;
        Grid[pos.x, pos.y].unit = null;
    }

    public void SetUnit(Vector2Int pos, Day15Unit unit)
    {
        Grid[pos.x, pos.y].hasUnit = true;
        Grid[pos.x, pos.y].unit = unit;
    }
}
