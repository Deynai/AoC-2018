using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day8TreeManager
{
    private int[] treeInput;
    private int iterator;
    private Day8TreeNode head;
    private Dictionary<int, Day8TreeNode> tree;

    public bool isFinished { get { return iterator == treeInput.Length; } }
    public Day8TreeNode Head { get { return head; } }
    public Dictionary<int, Day8TreeNode> Tree { get { return tree; } }

    public Day8TreeManager(int[] input)
    {
        treeInput = input;
        iterator = 0;
        tree = new Dictionary<int, Day8TreeNode>();
    }

    public void ConstructTree()
    {
        if (isFinished) { return; }

        head = RecurseTree(0);

        if(iterator != treeInput.Length)
        {
            Debug.Log($"Error: Iterator finished at {iterator} with input size {treeInput.Length}");
        }
    }

    private Day8TreeNode RecurseTree(int index)
    {
        // construct node at given index
        Day8TreeNode node = new Day8TreeNode(index, treeInput[iterator], treeInput[iterator + 1]);
        iterator += 2;

        // construct number of children recursively
        for(int i = 0; i < node.childCount; i++)
        {
            node.AddChild(RecurseTree(iterator));
        }

        // take dataCount ints as the data after children are finished
        node.data = treeInput.Skip(iterator).Take(node.dataCount).ToArray();
        iterator += node.dataCount;

        SetNodeValue(node);
        tree.Add(index, node);
        return node;
    }

    private void SetNodeValue(Day8TreeNode node)
    {
        int value = 0;
        if(node.childCount == 0)
        {
            value = node.data.Sum();
        }
        else
        {
            value = node.data.Where(d => d > 0 && d <= node.childCount).Select(d => node.children[d-1].value).Sum();
        }
        node.value = value;
    }

}


