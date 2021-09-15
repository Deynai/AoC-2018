using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Day20BFS
{
    public static int FindFurthestRoom(int[,] map, int[] bounds)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        int depth = 0;
        queue.Enqueue(new Vector3Int(0 - bounds[0], 0 - bounds[2], depth));
        visited.Add(new Vector2Int(0 - bounds[0], 0 - bounds[2]));

        Vector2Int[] deltas = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

        while(queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            depth = Mathf.Max(depth, current.z);

            foreach(Vector2Int d in deltas)
            {
                Vector2Int nextPos = new Vector2Int(current.x + d.x, current.y + d.y);
                if (visited.Contains(nextPos)){ continue; }
                if(map[nextPos.x, nextPos.y] > 0)
                {
                    queue.Enqueue(new Vector3Int(nextPos.x, nextPos.y, current.z + 1));
                    visited.Add(nextPos);
                }
            }
        }

        // only pass a door every 2nd step
        return depth / 2;
    }

    public static IEnumerator FindRoomsFurtherThanNum(int[,] map, int[] bounds, int num, Day20 texClass)
    {
        Texture2D texture = texClass.tex;
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        int depth = 0;
        queue.Enqueue(new Vector3Int(0 - bounds[0], 0 - bounds[2], depth));
        visited.Add(new Vector2Int(0 - bounds[0], 0 - bounds[2]));
        Vector2Int[] deltas = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

        int currDepth = 0;
        int roomCount = 0;
        Color[] colors = new Color[] { Color.blue, Color.cyan };
        Color col;
        List<Vector3Int> roomsOfDepth = new List<Vector3Int>();

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            depth = Mathf.Max(depth, current.z);

            if(depth > currDepth)
            {
                //col = currDepth >= num*2 ? colors[1] : colors[0];
                col = Color.Lerp(colors[0], colors[1], currDepth / (float) (num*7));
                currDepth++;
                foreach (Vector3Int room in roomsOfDepth)
                {
                    texture.SetPixel(room.x, room.y, col);
                }
                texture.Apply();
                roomsOfDepth.Clear();
                roomsOfDepth.Add(current);
                yield return null;
            }
            else
            {
                roomsOfDepth.Add(current);
            }

            if(depth % 2 == 0 && depth >= num*2) { roomCount++; }

            foreach (Vector2Int d in deltas)
            {
                Vector2Int nextPos = new Vector2Int(current.x + d.x, current.y + d.y);
                if (visited.Contains(nextPos)) { continue; }
                if (map[nextPos.x, nextPos.y] > 0)
                {
                    queue.Enqueue(new Vector3Int(nextPos.x, nextPos.y, current.z + 1));
                    visited.Add(nextPos);
                }
            }
        }

        col = currDepth >= num * 2 ? colors[1] : colors[0];
        foreach (Vector3Int room in roomsOfDepth)
        {
            texture.SetPixel(room.x, room.y, col);
        }
        texture.Apply();

        Debug.Log($"Rooms that pass more doors than {num}: {roomCount}");
    }
}
