using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Day22
{
    public static class Day22CaveBuilder
    {
        private static Color[] colors = new Color[] { Color.grey, Color.cyan, Color.yellow };

        public static Texture2D PrintCave(Day22Tile.Tile[,] cave)
        {
            int width = cave.GetLength(0);
            int height = cave.GetLength(1);
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    int type = (int)cave[i, j];
                    texture.SetPixel(i, height - j - 1, colors[type]);
                }
            }
            texture.Apply();

            return texture;
        }
    }

}
