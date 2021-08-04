using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day18 : MonoBehaviour
{
    int x_size;
    int y_size;
    State[,] grid;

    int openCount;
    int treeCount;
    int yardCount;
    int[,,] hashtable;

    Texture2D gridTexture;
    int blockpx = 16;
    Color[] gridColors = { new Color(178f/255,242f/255,121f/255), new Color(71f / 255, 120f / 255, 29f / 255), new Color(82f / 255, 63f / 255, 20f / 255) };

    enum State
    {
        open,
        tree,
        yard
    }

    private void Initialise()
    {
        char[][] input = InputHelper.ParseInputCharArray(18);
        y_size = input.Length;
        x_size = input[0].Length;

        grid = new State[x_size, y_size];

        for(int i = 0; i < x_size; i++)
        {
            for(int j = 0; j < y_size; j++)
            {
                if (input[j][i].Equals('|')) { grid[i, j] = State.tree; }
                else if (input[j][i].Equals('#')) { grid[i, j] = State.yard; }
                else { grid[i, j] = State.open; }
            }
        }

        InitHashTable();

        for(int i = 0; i < x_size; i++)
        {
            for(int j = 0; j < y_size; j++)
            {
                if (grid[i, j].Equals(State.tree)) { treeCount++; }
                else if (grid[i, j].Equals(State.yard)) { yardCount++; }
            }
        }
        openCount = x_size * y_size - treeCount - yardCount;

        gridTexture = new Texture2D(x_size * blockpx, y_size * blockpx);
        GetComponent<Renderer>().material.mainTexture = gridTexture;
        //transform.localScale = new Vector3(x_size, 1, y_size);
        DrawGrid();
    }

    private IEnumerator Solution()
    {
        yield return Part1();
    }

    private IEnumerator Part1()
    {
        HashSet<int> prevStates = new HashSet<int>();
        prevStates.Add(HashGrid());

        bool foundDuplicate = false;
        int firstDuplicatedHash = 0;
        List<int> indexOfDuplicatedHash = new List<int>();
        int cycleSize = 1;

        for(int i = 1; i <= 1000000000; i++)
        {
            Tick();
            yield return null;

            if (i == 10)
            {
                print($"Trees: {treeCount}, Lumberyards: {yardCount}, Resource Value: {treeCount * yardCount} ");
            }

            int hash = HashGrid();

            if (!foundDuplicate && prevStates.Contains(hash))
            {
                firstDuplicatedHash = hash;
                foundDuplicate = true;
            }
            else if(!foundDuplicate)
            {
                prevStates.Add(hash);
            }

            if(foundDuplicate && hash.Equals(firstDuplicatedHash))
            {
                indexOfDuplicatedHash.Add(i);
                print($"Found Dupe at {i}");
                //yield return new WaitForSeconds(1.0f);
                if (indexOfDuplicatedHash.Count > 1)
                {
                    cycleSize = indexOfDuplicatedHash[1] - indexOfDuplicatedHash[0];
                    break;
                }
            }
        }

        int afterInitialNoiseMinutes = 1000000000 - indexOfDuplicatedHash[0];
        int cycleMinute = afterInitialNoiseMinutes % cycleSize;

        for(int i = 0; i < cycleMinute; i++)
        {
            Tick();
            yield return null;
        }

        print($"Trees: {treeCount}, Lumberyards: {yardCount}, Resource Value: {treeCount * yardCount} ");
    }


    private void Tick()
    {
        State[,] nextGrid = new State[x_size, y_size];

        for(int i = 0; i < x_size; i++)
        {
            for(int j = 0; j < y_size; j++)
            {
                nextGrid[i,j] = GetNextState(i, j);
            }
        }

        grid = nextGrid;

        DrawGrid();
    }

    private State GetNextState(int x, int y)
    {
        (int open, int tree, int yard) neighbourCounts = CountNeighbourStates(x, y);
        switch (grid[x, y])
        {
            case State.open:
                if (neighbourCounts.tree >= 3) {
                    openCount--;
                    treeCount++;
                    return State.tree; }
                else { return State.open; }
            case State.tree:
                if (neighbourCounts.yard >= 3) {
                    treeCount--;
                    yardCount++;
                    return State.yard; }
                else { return State.tree; }
            case State.yard:
                if (neighbourCounts.yard >= 1 && neighbourCounts.tree >= 1) { return State.yard; }
                else {
                    yardCount--;
                    openCount++;
                    return State.open; }
            default:
                print($"Invalid State in Switch");
                return State.open;
        }
    }

    private (int,int,int) CountNeighbourStates(int x, int y)
    {
        int opens = 0;
        int trees = 0;
        int yards = 0;

        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) { continue; }

                int dx = x + i;
                int dy = y + j;
                if (dx < 0 || dx >= x_size || dy < 0 || dy >= y_size) { continue; }

                if (grid[dx, dy].Equals(State.open)) { opens++; }
                else if (grid[dx, dy].Equals(State.tree)) { trees++; }
                else { yards++; }
            }
        }

        return (opens, trees, yards);
    }

    private void DrawGrid()
    {

        for(int i = 0; i < x_size; i++)
        {
            for(int j = 0; j < y_size; j++)
            {
                for(int px = 0; px < blockpx; px++)
                {
                    for(int py = 0; py < blockpx; py++)
                    {
                        gridTexture.SetPixel(i*blockpx + px, (y_size - j)*blockpx - 1 - py, gridColors[(int)grid[i, j]]);
                    }
                }
            }
        }
        gridTexture.Apply();
    }

    private void InitHashTable()
    {
        hashtable = new int[x_size,y_size,3];

        for(int i = 0; i < x_size; i++)
        {
            for(int j = 0; j < y_size; j++)
            {
                for (int z = 0; z < 3; z++)
                {
                    hashtable[i, j, z] = Random.Range(0, int.MaxValue);
                }
            }
        }
    }

    private int HashGrid()
    {
        int hash = 0;
        for(int i = 0; i < x_size; i++)
        {
            for(int j = 0; j < y_size; j++)
            {
                hash = hash ^ hashtable[i, j, (int)grid[i, j]];
            }
        }
        return hash;
    }

    private void Start()
    {
        Initialise();

        StartCoroutine(Solution());
    }
}
