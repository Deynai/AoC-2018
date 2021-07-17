using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day15Pathfinding
{
    public Day15Grid grid;
    private class Node
    {
        public Day15GameTile tile;
        public int dValue;
        public int hValue;
        public Node parent;

        public Node(Day15GameTile _tile, int _dValue, int _hValue, Node _parent)
        {
            tile = _tile;
            dValue = _dValue;
            hValue = _hValue;
            parent = _parent;
        }

        public void UpdateNode(int _dValue, int _hValue, Node _parent)
        {
            dValue = _dValue;
            hValue = _hValue;
            parent = _parent;
        }
    }

    public Day15Pathfinding(Day15Grid _grid)
    {
        grid = _grid;
    }

    public int AbsDistance(Vector2Int start, Vector2Int end)
    {
        return (Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y));
    }

    // returns a tuple for new location, and whether we moved at all
    public Vector2Int BFSMoveToNearest(Vector2Int start)
    {
        Node startNode = new Node(grid.Grid[start.x, start.y], 0, 0, null);

        // the open set to pull next node from
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startNode);

        // the closed set to flag nodes as visited
        HashSet<Day15GameTile> seenTiles = new HashSet<Day15GameTile>();
        seenTiles.Add(startNode.tile);

        Node current;
        int stopDepth = int.MaxValue;
        List<Node> unitsFound = new List<Node>();

        while ((queue.Count > 0))
        {
            current = queue.Dequeue();
            if(current.dValue > stopDepth) { break; }

            // if this node position matches our hashset of unit locations, add this unit to a list and finish this depth level of the queue
            if (current.tile.hasUnit && current.tile.unit.faction != startNode.tile.unit.faction)
            {
                stopDepth = current.dValue;
                unitsFound.Add(current);
            }
            else
            {
                foreach (Day15GameTile tile in current.tile.adjacentTiles)
                {
                    if (tile != null && !seenTiles.Contains(tile))
                    {
                        if (tile.Walkable || (tile.hasUnit && tile.unit.faction != startNode.tile.unit.faction))
                        {
                            queue.Enqueue(new Node(tile, current.dValue + 1, 0, current));
                            seenTiles.Add(tile);
                        }
                    }
                }
            }
        }

        // once a unit is found and the queue cleared of all nodes less than or equal to that depth
        // find the first enemy in reading order and follow the parents to one before the start node from that enemy
        // if stopDepth is 1 we are already next to a target unit so don't move
        if (unitsFound.Count.Equals(0) || stopDepth.Equals(1))
        {
            return start;
        }
        else
        {
            Node targetNode = unitsFound.OrderBy(u => u.tile.Order).First();
            for (int i = 0; i < stopDepth-1; i++)
            {
                targetNode = targetNode.parent;
            }
            return targetNode.tile.pos;
        }
    }
}
