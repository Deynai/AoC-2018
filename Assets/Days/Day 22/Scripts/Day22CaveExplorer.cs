using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Day22
{
    public class Day22CaveExplorer
    {
        Day22Tile.Tile[,] cave;
        Vector2Int target;
        int width;
        int height;

        bool[,] toolAllowed = new bool[,] { { true, false, true }, { true, true, false }, { false, true, true } };

        public Day22CaveExplorer(Day22Tile.Tile[,] cave, Vector2Int target)
        {
            this.cave = cave;
            this.target = target;
            width = cave.GetLength(0);
            height = cave.GetLength(1);
        }

        internal struct Node
        {
            public int tool;
            public Vector2Int pos;

            public Node(int _tool, Vector2Int _pos)
            {
                tool = _tool;
                pos = _pos;
            }
        }

        public int ExploreAStar()
        {
            LinkedList<(int, Node)> openSet = new LinkedList<(int, Node)>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.AddFirst((0, new Node(0, new Vector2Int(0, 0))));

            Vector2Int[] deltas = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
            
            while(openSet.Count > 0)
            {
                (int minute, Node n) currNode = openSet.First.Value;
                openSet.RemoveFirst();

                Debug.Log($"Pos: {currNode.n.pos}, Type: {cave[currNode.n.pos.x, currNode.n.pos.y]}, Tool: {currNode.n.tool}");

                if(currNode.n.tool == 0 && currNode.n.pos == target)
                {
                    return currNode.minute;
                }

                if (closedSet.Contains(currNode.n)) { continue; }
                else { closedSet.Add(currNode.n); }

                foreach(Vector2Int d in deltas)
                {
                    Vector2Int newPos = currNode.n.pos + d;
                    if(newPos.x < 0 || newPos.y < 0 || newPos.x >= width || newPos.y >= height) { continue; }
                    if (!ToolAllowedAtPos(currNode.n.tool, newPos)) { continue; }
                    Node newNode = new Node(currNode.n.tool, newPos);
                    TryAddNode(closedSet, openSet, currNode.minute + 1, newNode);
                }

                for (int i = 0; i < 3; i++)
                {
                    if(i == currNode.n.tool) { continue; }
                    if(ToolAllowedAtPos(i, currNode.n.pos))
                    {
                        Node newNode = new Node(i, currNode.n.pos);
                        TryAddNode(closedSet, openSet, currNode.minute + 7, newNode);
                    }
                }
            }

            return -1;
        }

        private bool ToolAllowedAtPos(int tool, Vector2Int pos)
        {
            int type = (int) cave[pos.x, pos.y];
            return toolAllowed[tool, type];
        }

        private void TryAddNode(HashSet<Node> closedSet, LinkedList<(int, Node)> openSet, int minutes, Node n)
        {
            if (closedSet.Contains(n)) { return; }
            else { InsertInOrder(ref openSet, minutes, n); }
        }

        private void InsertInOrder(ref LinkedList<(int, Node)> openSet, int minutes, Node n)
        {
            if(openSet.Count == 0)
            {
                openSet.AddFirst((minutes, n));
                return;
            }

            var currNode = openSet.First;

            for(int i = 0; i < openSet.Count; i++)
            {
                if(minutes < currNode.Value.Item1)
                {
                    openSet.AddBefore(currNode, (minutes, n));
                    return;
                }

                currNode = currNode.Next;
            }

            openSet.AddLast((minutes, n));
        }

        private int Heuristic(Node n)
        {
            return Mathf.Abs(target.x - n.pos.x) + Mathf.Abs(target.y - n.pos.y); 
        }
    }
}
