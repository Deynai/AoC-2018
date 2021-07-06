using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day8 : MonoBehaviour
{
    Day8TreeManager treeManager;

    private void Part1()
    {
        int[] input = InputHelper.ParseInputString(8).Split(' ').Select(x => int.Parse(x)).ToArray();

        treeManager = new Day8TreeManager(input);
        treeManager.ConstructTree();

        long dataSum = 0;
        foreach(Day8TreeNode node in treeManager.Tree.Values)
        {
            dataSum += node.data.Sum();
        }
        print(dataSum);
    }

    private void Part2()
    {
        print(treeManager.Head.value);
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
