using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class Day7NodeController : MonoBehaviour
{
    public GameObject nodeObject;

    public Dictionary<string, Day7Node> nodes = new Dictionary<string, Day7Node>();

    public void RemoveParentFromChildren(string id)
    {
        Day7Node node = nodes[id];
        foreach(Day7Node child in node.children)
        {
            child.parents.Remove(node);
            child.RequestUpdate();
        }
    }

    public void ConstructNodes()                               
    {
        string[] input = InputHelper.ParseInputArray(7).Select(line => Regex.Replace(line, "Step | must be finished before step | can begin.", "-")).ToArray();
        nodes = new Dictionary<string, Day7Node>();
        foreach(string line in input)
        {
            MatchCollection matches = Regex.Matches(line, "\\w+");
            ConnectNodes(matches[0].Value.ToString(), matches[1].Value.ToString());
        }

        SetPositions();
    }

    private void SetPositions()
    {
        // set depths

        Day7Node[] heads = nodes.Where(n => n.Value.parents.Count() == 0).Select(n => n.Value).ToArray();
        foreach(Day7Node head in heads)
        {
            head.depth = 0;
            SetDepths(head, 0);
        }

        // set positions
        var depthGroups = nodes.GroupBy(n => n.Value.depth).Select(group => group.ToList());
        foreach(var depthGroup in depthGroups)
        {
            for(int i = 0; i < depthGroup.Count; i++)
            {
                depthGroup[i].Value.transform.Translate(new Vector3(depthGroup[i].Value.depth * 5, 0, i * 5));
            }
        }

        foreach(Day7Node node in nodes.Values)
        {
            node.RequestUpdate();
        }
    }

    private void SetDepths(Day7Node parent, int depth)
    {
        foreach(Day7Node child in parent.children)
        {
            child.depth = Math.Max(depth + 1, child.depth);
        }
        foreach(Day7Node child in parent.children)
        {
            SetDepths(child, depth + 1);
        }
    }

    private void ConnectNodes(string parent, string child)
    {
        if (!nodes.ContainsKey(parent))
        {
            nodes.Add(parent, CreateNewNodeObject(parent).GetComponent<Day7Node>());
        }
        if (!nodes.ContainsKey(child))
        {
            nodes.Add(child, CreateNewNodeObject(child).GetComponent<Day7Node>());
        }

        nodes[parent].children.Add(nodes[child]);
        nodes[child].parents.Add(nodes[parent]);
    }

    private GameObject CreateNewNodeObject(string id)
    {
        GameObject newNode = Instantiate(nodeObject, transform, false);
        newNode.GetComponent<Day7Node>().Constructor(id);
        newNode.name = id;
        return newNode;
    }

    public void ResetController()
    {
        foreach(KeyValuePair<string, Day7Node> node in nodes)
        {
            Destroy(node.Value.gameObject);
        }
        nodes.Clear();
    }
}
