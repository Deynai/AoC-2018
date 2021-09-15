using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Day20MapBuilder
{
    public static int[,] GenerateMap(Dictionary<Vector2Int, int> dictMap, int[] bounds)
    {
        int width = bounds[1] - bounds[0];
        int height = bounds[3] - bounds[2];
        int[,] map = new int[width + 1, height + 1];

        foreach(KeyValuePair<Vector2Int, int> kvp in dictMap)
        {
            map[kvp.Key.x - bounds[0], kvp.Key.y - bounds[2]] = kvp.Value;
        }

        return map;
    }

    private static Color[] colors = new Color[] { Color.black, Color.grey, Color.grey, Color.grey };

    public static Texture2D PrintMap(int[,] map)
    {
        Texture2D texture = new Texture2D(map.GetLength(0), map.GetLength(1));
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                texture.SetPixel(i, j, colors[map[i, j]]);
            }
        }
        texture.Apply();

        return texture;
    }
}
