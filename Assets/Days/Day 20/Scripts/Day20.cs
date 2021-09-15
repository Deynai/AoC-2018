using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day20 : MonoBehaviour
{
    public Texture2D tex;

    private void Start()
    {
        string input = InputHelper.ParseInputString(20);

        Day20RegexParser parser = new Day20RegexParser(input);

        parser.ParseRegex();
        int[] bounds = parser.GetPaddedBounds();

        int[,] map = Day20MapBuilder.GenerateMap(parser.GetRooms(), bounds);
        tex = Day20MapBuilder.PrintMap(map);
        tex.filterMode = FilterMode.Point;

        int furthestRoom = Day20BFS.FindFurthestRoom(map, bounds);

        print($"Furthest Room: {furthestRoom}");

        StartCoroutine(Day20BFS.FindRoomsFurtherThanNum(map, bounds, 1000, this));
    }



    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(tex, destination);
    }
}
