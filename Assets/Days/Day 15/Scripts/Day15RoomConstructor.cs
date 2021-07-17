using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15RoomConstructor : MonoBehaviour
{
    public GameObject wall;
    public GameObject tile;

    //public Day15Grid roomTiles;
    //public Day15GameTile[,] roomTiles;

    private char[][] charGrid;

    public Day15Grid BuildRoom()
    {
        charGrid = InputHelper.ParseInputCharArray(15);
        Day15Grid grid = new Day15Grid(charGrid[0].Length, charGrid.Length);
        (int i, int j)[] deltas = { (0, -1),  (-1, 0), (1, 0), (0, 1) };

        for (int i = 0; i < grid.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.Grid.GetLength(1); j++)
            {
                if (!CheckSurrounded(i, j))
                {
                grid.Grid[i, j] = new Day15GameTile(charGrid[j][i], new Vector2Int(i, j), this);
                }
            }
        }

        // Set all open squares as neighbours - walls will be null.
        for (int i = 0; i < grid.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.Grid.GetLength(1); j++)
            {
                for(int ds = 0; ds < 4; ds++)
                {
                    if (i + deltas[ds].i < 0 || i + deltas[ds].i > charGrid[0].Length - 1 || j + deltas[ds].j < 0 || j + deltas[ds].j > charGrid.Length - 1) { continue; }
                    else
                    {
                        if (!charGrid[j][i].Equals('#'))
                        {
                            grid.Grid[i, j].adjacentTiles[ds] = grid.Grid[i + deltas[ds].i, j + deltas[ds].j];
                        }
                    }
                }
            }
        }

        return grid;
    }

    private bool CheckSurrounded(int i, int j)
    {
        if (!charGrid[j][i].Equals('#')) { return false; }
        (int i, int j)[] deltas = { (-1, 0), (1, 0), (0, -1), (0, 1), (-1,-1), (1,1), (-1, 1), (1, -1)};
        foreach((int i, int j) d in deltas)
        {
            if(i + d.i < 0 || i + d.i > charGrid[0].Length - 1 || j + d.j < 0 || j + d.j > charGrid.Length - 1) { continue; }
            else
            {
                if (!charGrid[j + d.j][i + d.i].Equals('#')){
                    return false;
                }
            }
        }

        return true;
    }

    public GameObject CreatePrefab(char c)
    {
        GameObject prefab;

        if (c.Equals('#'))
        {
            prefab = Instantiate(wall, transform, false);
        }
        else
        {
            prefab = Instantiate(tile, transform, false);
        }

        return prefab;
    }

}
