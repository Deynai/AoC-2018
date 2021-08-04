using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class Day17 : MonoBehaviour
{
    [SerializeField]
    private GameObject gridDisplay;
    private Color[] colors = { Color.white, Color.black, Color.blue, Color.cyan };

    private Camera cam;

    tile[,] grid;
    int[] x_range;
    int[] y_range;
    enum tile
    {
        sand,
        clay,
        water,
        flow
    }

    // Initialisation
    private void Initialise()
    {
        string input = InputHelper.ParseInputString(17);
        string[] inputLines = input.Trim().Replace("\r", "").Split('\n').ToArray();

        SetGridSize(input);
        
        foreach(string line in inputLines)
        {
            int[] digits = Regex.Matches(line, "\\d+").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();

            if (line[0].Equals('x'))
            {
                SetClayLine(digits[0], digits[0], digits[1], digits[2]);
            }
            else if (line[0].Equals('y'))
            {
                SetClayLine(digits[1], digits[2], digits[0], digits[0]);
            }
        }

        // Draw Initial Grid
        StartCoroutine(DrawInitialGrid());
    }
    private void SetGridSize(string input)
    {
        List<int> x_values = new List<int>();
        List<int> y_values = new List<int>();

        MatchCollection x_matches = Regex.Matches(input, "x.?\\d+(\\.\\.)?(\\d+)?");
        MatchCollection y_matches = Regex.Matches(input, "y.?\\d+(\\.\\.)?(\\d+)?");

        foreach(Match m in x_matches)
        {
            int[] numbers = Regex.Matches(m.Value, "\\d+").Cast<Match>().Select(n => int.Parse(n.Value)).ToArray();
            foreach(int num in numbers)
            {
                x_values.Add(num);
            }
        }

        foreach (Match m in y_matches)
        {
            int[] numbers = Regex.Matches(m.Value, "\\d+").Cast<Match>().Select(n => int.Parse(n.Value)).ToArray();
            foreach (int num in numbers)
            {
                y_values.Add(num);
            }
        }

        int x_min = x_values.Min();
        int x_max = x_values.Max();
        int y_min = y_values.Min();
        int y_max = y_values.Max();
        int padding = 5;
        x_range = new int[2] { x_min - padding, x_max - x_min + padding*2 };
        y_range = new int[2] { y_min - padding, y_max - y_min + padding*2 };

        grid = new tile[x_range[1], y_range[1]];
    }
    private void SetClayLine(int x_start, int x_end, int y_start, int y_end)
    {
        for(int i = x_start - x_range[0]; i <= x_end - x_range[0]; i++)
        {
            for(int j = y_start - y_range[0]; j <= y_end - y_range[0]; j++)
            {
                grid[i, j] = tile.clay;
            }
        }
    }
    private IEnumerator DrawInitialGrid()
    {
        Texture2D texture = new Texture2D(grid.GetLength(0), grid.GetLength(1));

        gridDisplay.GetComponent<Renderer>().material.mainTexture = texture;

        Vector3 scale = new Vector3(grid.GetLength(0), 0, grid.GetLength(1)).normalized * 100;
        gridDisplay.transform.localScale = scale + Vector3.up;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                texture.SetPixel(i, grid.GetLength(1) - j - 1, colors[(int)grid[i, j]]);
            }
        }
        texture.Apply();
        yield return null;
    }

    // Part 1

    private IEnumerator Solution()
    {
        yield return RunWater();
    }

    private IEnumerator RunWater()
    {
        // algorithm idea:

        // queue to compute source blocks in case we split into 2 or more streams
        // each source block will be calculated in turn
        // queue ends if y out of range

        // calculation: 
        // if sand below, flow directly down, source block 1 down added to queue
        // if clay or water below, check -x and +x direction to edge of grid, 3 cases ->
        //                  cases:
        //                  2 edge: sand exists below on both sides, fill to the sand with flow, and add 2 new source blocks 1 down
        //                  1 edge: sand exists on only one side with clay blocking on the other, fill to the clay and sand with flow, and create 1 source block 1 down sand side
        //                  clay on both sides: fill the range with water, and add a water source directly 1 up

        // we will update grid and gridDisplay as we go
        // maybe keep count of water/flow blocks added as we go

        Vector2Int startSource = new Vector2Int(500 - x_range[0], 0 - y_range[0]);

        Queue<Vector2Int> sourceBlockQueue = new Queue<Vector2Int>();
        sourceBlockQueue.Enqueue(startSource);
        int[] y_limits = { 5, y_range[1] - 5 };

        // Camera
        Vector3 camStart = new Vector3(0, 74, -502);
        Vector3 camEnd = new Vector3(0, 74, 452);
        float currentCameraScroll = -452;

        while(sourceBlockQueue.Count > 0)
        {
            Vector2Int sourceBlock = sourceBlockQueue.Dequeue();

            currentCameraScroll = Mathf.Max(currentCameraScroll, Mathf.Lerp(camStart.z, camEnd.z, (float)sourceBlock.y / y_limits[1]));
            Vector3 newCamPosition = new Vector3(camStart.x, camStart.y, currentCameraScroll);
            cam.transform.position = newCamPosition;
            
            // Initial filters

            // if we have dropped off the bottom, cancel
            if (sourceBlock.y > y_limits[1])
            {
                continue;
            }

            // if we're water already, set source as 1 up, and cancel this one
            if (grid[sourceBlock.x, sourceBlock.y].Equals(tile.water))
            {
                sourceBlockQueue.Enqueue(new Vector2Int(sourceBlock.x, sourceBlock.y - 1));
                continue;
            }

            // if we're about to land on already flowing water, cancel
            if (grid[sourceBlock.x, sourceBlock.y + 1].Equals(tile.flow))
            {
                grid[sourceBlock.x, sourceBlock.y] = tile.flow;
                UpdateGridDisplay(sourceBlock.x, sourceBlock.y);
                continue;
            }
            // if we are flowing water, with flowing water left and right too, then cancel
            if (grid[sourceBlock.x, sourceBlock.y].Equals(tile.flow) && grid[sourceBlock.x - 1, sourceBlock.y].Equals(tile.flow) && grid[sourceBlock.x + 1, sourceBlock.y].Equals(tile.flow) && !grid[sourceBlock.x, sourceBlock.y + 1].Equals(tile.sand))
            {
                continue;
            }

            // Pathing

            if (grid[sourceBlock.x, sourceBlock.y + 1].Equals(tile.sand))
            {
                grid[sourceBlock.x, sourceBlock.y] = tile.flow;
                UpdateGridDisplay(sourceBlock.x, sourceBlock.y);
                sourceBlockQueue.Enqueue(new Vector2Int(sourceBlock.x, sourceBlock.y + 1));
            }

            else
            {
                // LEFT CHECK
                (int end, bool isBlocked, bool isSource) leftData = ScanDirection(sourceBlock, -1);

                if (leftData.isSource){
                    sourceBlockQueue.Enqueue(new Vector2Int(sourceBlock.x + leftData.end, sourceBlock.y));
                }

                // RIGHT CHECK
                (int end, bool isBlocked, bool isSource) rightData = ScanDirection(sourceBlock, 1);

                if (rightData.isSource)
                {
                    sourceBlockQueue.Enqueue(new Vector2Int(sourceBlock.x + rightData.end, sourceBlock.y));
                }

                // BOTH SIDES
                if(leftData.isBlocked && rightData.isBlocked)
                {
                    for (int x = sourceBlock.x + leftData.end; x <= sourceBlock.x + rightData.end; x++)
                    {
                        grid[x, sourceBlock.y] = tile.water;
                    }
                    UpdateGridDisplay(sourceBlock.x + leftData.end, sourceBlock.x + rightData.end, sourceBlock.y, sourceBlock.y);
                    sourceBlockQueue.Enqueue(new Vector2Int(sourceBlock.x, sourceBlock.y - 1));
                }
                else
                {
                    for (int x = sourceBlock.x + leftData.end; x <= sourceBlock.x + rightData.end; x++)
                    {
                        grid[x, sourceBlock.y] = tile.flow;
                    }
                    UpdateGridDisplay(sourceBlock.x + leftData.end, sourceBlock.x + rightData.end, sourceBlock.y, sourceBlock.y);
                }
            }

            yield return null;
        }

        print($"Total water count: {CountWater(y_limits)}");
        print($"Total water at rest count: {CountWaterAtRest(y_limits)}");
    }

    private (int endVector, bool isBlocked, bool isSource) ScanDirection(Vector2Int sourceBlock, int dir)
    {
        int scan_x = sourceBlock.x;
        int step_count = 0;

        while(!grid[scan_x, sourceBlock.y].Equals(tile.clay))
        {
            scan_x += dir;
            step_count += dir;

            if(grid[scan_x, sourceBlock.y + 1].Equals(tile.sand))
            {
                return (step_count, false, true);
            }
            else if(grid[scan_x, sourceBlock.y + 1].Equals(tile.flow))
            {
                return (step_count, false, false);
            }
        }

        step_count -= dir;
        return (step_count, true, false);
    }

    private int CountWater(int[] y_limits)
    {
        int count = 0;
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = y_limits[0]; j <= y_limits[1]; j++)
            {
                if(grid[i,j].Equals(tile.water) || grid[i, j].Equals(tile.flow))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private int CountWaterAtRest(int[] y_limits)
    {
        int count = 0;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = y_limits[0]; j <= y_limits[1]; j++)
            {
                if (grid[i, j].Equals(tile.water))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void UpdateGridDisplay(int x, int y)
    {
        Texture2D texture = (Texture2D) gridDisplay.GetComponent<Renderer>().material.mainTexture;

        texture.SetPixel(x, grid.GetLength(1) - y - 1, colors[(int)grid[x, y]]);
        texture.Apply();
    }

    private void UpdateGridDisplay(int x1, int x2, int y1, int y2)
    {
        Texture2D texture = (Texture2D)gridDisplay.GetComponent<Renderer>().material.mainTexture;
        for (int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                texture.SetPixel(i, grid.GetLength(1) - j - 1, colors[(int)grid[i, j]]);
            }
        }
        texture.Apply();
    }

    void Start()
    {
        cam = Camera.main;
        Initialise();

        StartCoroutine(Solution());
    }
}
