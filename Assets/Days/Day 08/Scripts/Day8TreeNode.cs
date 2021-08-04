using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day8TreeNode
{
    public int id { get; set; }
    public int childCount { get; set; }
    public int dataCount { get; set; }
    public int[] data { get; set; }
    public int value { get; set; }
    public List<Day8TreeNode> children { get; }

    public Day8TreeNode(int id, int childCount, int dataCount)
    {
        this.id = id;
        this.childCount = childCount;
        this.dataCount = dataCount;
        data = new int[dataCount];
        children = new List<Day8TreeNode>();
    }

    public void AddChild(Day8TreeNode node)
    {
        children.Add(node);
    }
}
