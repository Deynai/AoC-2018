using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace Day22
{
    public class Day22 : MonoBehaviour
    {
        private Day22GeoIndexer geo;
        private Texture2D texture;

        private void Start()
        {
            string[] input = InputHelper.ParseInputArray(22);

            int depth = int.Parse(Regex.Match(input[0], "\\d+").Value);
            int[] targetNum = Regex.Matches(input[1], "\\d+").Cast<Match>().Select(n => int.Parse(n.Value)).ToArray();
            Vector2Int target = new Vector2Int(targetNum[0], targetNum[1]);

            geo = new Day22GeoIndexer(target, depth);

            Part1();
        }
        private void Part1()
        {
            int padding_x = 50;
            int padding_y = 100;
            Day22Tile.Tile[,] cave = new Day22Tile.Tile[geo.target.x + padding_x, geo.target.y + padding_y];
            int riskLevel = 0;

            for(int i = 0; i < geo.target.x + padding_x; i++)
            {
                for(int j = 0; j < geo.target.y + padding_y; j++)
                {
                    int type = geo.GetType(i, j);
                    cave[i, j] = (Day22Tile.Tile) type;
                }
            }

            for(int i = 0; i < geo.target.x + 1; i++)
            {
                for(int j = 0; j < geo.target.y + 1; j++)
                {
                    riskLevel += (int)cave[i, j];
                }
            }

            texture = Day22CaveBuilder.PrintCave(cave);

            print($"Total Risk Level: {riskLevel}");

            Part2(cave);
        }

        private void Part2(Day22Tile.Tile[,] cave)
        {
            Day22CaveExplorer explorer = new Day22CaveExplorer(cave, geo.target);

            print($"{explorer.ExploreAStar()}");
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(texture, destination);
        }
    }
}
