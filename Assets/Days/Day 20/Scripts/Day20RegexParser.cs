using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public class Day20RegexParser
{
    private Dictionary<Vector2Int, int> rooms = new Dictionary<Vector2Int, int>();
    private int[] bounds = new int[4];
    private string input;
    private int globalIndex = 0;

    public Day20RegexParser(string input)
    {
        this.input = input;
    }

    internal struct RegexPos
    {
        public int index;
        public int depth;
        public Vector2Int pos;

        public RegexPos(int index, int depth, Vector2Int pos)
        {
            this.index = index;
            this.depth = depth;
            this.pos = pos;
        }
    }

    public Dictionary<Vector2Int, int> GetRooms()
    {
        return rooms;
    }

    public int[] GetBounds()
    {
        return bounds;
    }
    public int[] GetPaddedBounds()
    {
        int[] paddedBounds = new int[4];
        paddedBounds[0] = bounds[0] - 1;
        paddedBounds[1] = bounds[1] + 1;
        paddedBounds[2] = bounds[2] - 1;
        paddedBounds[3] = bounds[3] + 1;
        return paddedBounds;
    }

    public void ParseRegex()
    {
        globalIndex = input.Length - 1;
        rooms = BackwardsParseRegex(0, 0, new Dictionary<Vector2Int, int>());
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        foreach (Vector2Int key in rooms.Keys)
        {
            if (key.x < bounds[0]) { bounds[0] = key.x; }
            else if (key.x > bounds[1]) { bounds[1] = key.x; }
            if (key.y < bounds[2]) { bounds[2] = key.y; }
            else if (key.y > bounds[3]) { bounds[3] = key.y; }
        }
    }

    // Dynamic Programming bottom-up approach:
    // Starts at the end of the array and overlays the path so far to each split in the branch, merging both together again when the recursion returns
    // eg: A(B|C)D -> BD and CD are calculated as separate dicts (arrays), then overlayed/merged into a new single dict E = BD U CD.
    // This merging stops exponential branching, but may be optimised considerably to cut out unnecessary copying, merging, and offsetting of every dict.

    private Dictionary<Vector2Int, int> BackwardsParseRegex(int _x, int _y, Dictionary<Vector2Int, int> prevRooms)
    {
        int x = 0;
        int y = 0;
        Dictionary<Vector2Int, int> sumRoomsDict = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> roomsDict = new Dictionary<Vector2Int, int>();
        roomsDict.Add(Vector2Int.zero, 1);

        MergeRooms(roomsDict, prevRooms);

        while(globalIndex > 0)
        {
            globalIndex--;
            char nextChar = input[globalIndex];
            if (nextChar == '(')
            {
                roomsDict = AdjustRoomsOffset(roomsDict, new Vector2Int(-x, -y));
                sumRoomsDict = MergeRooms(sumRoomsDict, roomsDict);
                return sumRoomsDict;
            }
            else if (nextChar == ')')
            {
                roomsDict = AdjustRoomsOffset(roomsDict, new Vector2Int(-x, -y));
                roomsDict = BackwardsParseRegex(0, 0, roomsDict);
                x = 0;
                y = 0;
            }
            else if (nextChar == '|')
            {
                roomsDict = AdjustRoomsOffset(roomsDict, new Vector2Int(-x,-y));
                sumRoomsDict = MergeRooms(sumRoomsDict, roomsDict);
                roomsDict.Clear();
                roomsDict.Add(Vector2Int.zero, 1);
                roomsDict = MergeRooms(roomsDict, prevRooms);
                x = 0;
                y = 0;
            }
            else
            {
                // reversed directions
                if (nextChar == 'S')
                {
                    AddRoom(ref roomsDict, x, y + 1, 2);
                    AddRoom(ref roomsDict, x, y + 2, 1);
                    y += 2;
                }
                else if (nextChar == 'W')
                {
                    AddRoom(ref roomsDict, x + 1, y, 3);
                    AddRoom(ref roomsDict, x + 2, y, 1);
                    x += 2;
                }
                else if (nextChar == 'N')
                {
                    AddRoom(ref roomsDict, x, y - 1, 2);
                    AddRoom(ref roomsDict, x, y - 2, 1);
                    y -= 2;
                }
                else if (nextChar == 'E')
                {
                    AddRoom(ref roomsDict, x - 1, y, 3);
                    AddRoom(ref roomsDict, x - 2, y, 1);
                    x -= 2;
                }
            }
        }

        roomsDict = AdjustRoomsOffset(roomsDict, new Vector2Int(-x, -y));
        sumRoomsDict = MergeRooms(sumRoomsDict, roomsDict);
        return sumRoomsDict;
    }

    private Dictionary<Vector2Int, int> AdjustRoomsOffset(Dictionary<Vector2Int, int> rooms, Vector2Int offset)
    {
        Dictionary<Vector2Int, int> newRooms = new Dictionary<Vector2Int, int>();

        foreach(KeyValuePair<Vector2Int, int> room in rooms)
        {
            newRooms.Add(new Vector2Int(room.Key.x + offset.x, room.Key.y + offset.y), room.Value);
        }

        return newRooms;
    }
    private Dictionary<Vector2Int, int> MergeRooms(Dictionary<Vector2Int, int> dict1, Dictionary<Vector2Int, int> dict2)
    {
        foreach(KeyValuePair<Vector2Int, int> kvp in dict2)
        {
            if (!dict1.ContainsKey(kvp.Key))
            {
                dict1.Add(kvp.Key, kvp.Value);
            }
        }

        return dict1;
    }
    private void AddRoom(ref Dictionary<Vector2Int, int> rooms, int x, int y, int val)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (!rooms.ContainsKey(pos))
        {
            rooms.Add(pos, val);
        }
    }

    // Parse Regex As Path Length
    // Efficiently calculates only the length and does not need to construct a map or directions to do so.
    // Counts each move and discards anything less than the max amount, also discards (NSWE|) "dead ends" as 0 length - can produce an error if a "dead end" is furthest away
    public int ParseRegexAsPathLength()
    {
        globalIndex = 1;

        return RecursePathLength();
    }

    private static Dictionary<char, Vector2Int> dirDict = new Dictionary<char, Vector2Int> { { 'N', new Vector2Int(0, 1) }, { 'E', new Vector2Int(1, 0) }, { 'S', new Vector2Int(0, -1) }, { 'W' , new Vector2Int(-1, 0) } };
    private int RecursePathLength()
    {
        List<int> lengths = new List<int>();
        Vector2Int sumDirs = new Vector2Int();
        int sum = 0;
        int totalMoves = 0;

        while (globalIndex < input.Length - 1)
        {
            char currChar = input[globalIndex];
            globalIndex++;

            // if (
            if (currChar == '(')
            {
                sum += RecursePathLength();
            }

            // if )
            if (currChar == ')')
            {
                int distance = sumDirs == Vector2Int.zero ? 0 : totalMoves;
                lengths.Add(distance + sum);
                return lengths.Max();
            }

            // if |
            if (currChar == '|')
            {
                int distance = sumDirs == Vector2Int.zero ? 0 : totalMoves;
                lengths.Add(distance + sum);
                sumDirs = Vector2Int.zero;
                sum = 0;
                totalMoves = 0;
            }

            // if letter
            if (dirDict.ContainsKey(currChar))
            {
                totalMoves++;
                sumDirs += dirDict[currChar];
            }
        }

        int dist = sumDirs == Vector2Int.zero ? 0 : totalMoves;
        lengths.Add(dist + sum);
        return lengths.Max();
    }
}
