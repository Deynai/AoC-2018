using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class Day7 : MonoBehaviour
{
    Day7NodeController nodeController;

    public TextMeshProUGUI timeText;

    private IEnumerator Solution()
    {
        //yield return StartCoroutine(Part1());
        yield return StartCoroutine(Part2());
    }

    private IEnumerator Part1()
    {
        nodeController.ConstructNodes();
        Dictionary<string, Day7Node> nodes = nodeController.nodes;

        HashSet<string> checkedNodes = new HashSet<string>();
        List<string> orderedNodes = new List<string>();

        KeyValuePair<string, Day7Node> nextNode;
        var nextNodes = nodes.Where(n => n.Value.parents.Count() == 0 && !checkedNodes.Contains(n.Key)).OrderBy(n => n.Key);
        while (nextNodes.Count() > 0)
        {
            foreach (KeyValuePair<string, Day7Node> node in nextNodes)
            {
                node.Value.SetColor(Color.green);
            }
            yield return null;

            nextNode = nextNodes.First();
            checkedNodes.Add(nextNode.Key);
            orderedNodes.Add(nextNode.Key);
            nextNode.Value.SetColor(Color.black);
            yield return null;

            foreach (Day7Node child in nextNode.Value.children)
            {
                child.parents.Remove(nextNode.Value);
            }
            nextNodes = nodes.Where(n => n.Value.parents.Count() == 0 && !checkedNodes.Contains(n.Key)).OrderBy(n => n.Key);
            yield return null;
        }

        print($"Node order: {string.Join("", orderedNodes)}");
        yield break;
    }

    private IEnumerator Part2()
    {
        nodeController.ResetController();
        nodeController.ConstructNodes();
        Dictionary<string, Day7Node> nodes = nodeController.nodes;
        HashSet<string> checkedNodes = new HashSet<string>();
        HashSet<string> queuedNodes = new HashSet<string>();

        Day7TaskManager taskManager = new Day7TaskManager();

        int workers = 5;
        bool isFinished = false;
        int bp = 0;
        KeyValuePair<string, Day7Node> nextNode;

        while (!isFinished)
        {
            if(++bp > 10000) { break; }

            var nextNodes = nodes.Where(n => n.Value.parents.Count() == 0 && !queuedNodes.Contains(n.Key)).OrderBy(n => n.Key);

            /// colors
            foreach(KeyValuePair<string,Day7Node> node in nextNodes)
            {
                node.Value.SetColor(Color.green);
                int taskTime = GetTaskTime(node.Key);
                node.Value.SetText(taskTime.ToString()); 
            }

            if (taskManager.Count() < workers && nextNodes.Count() > 0)
            {
                nextNode = nextNodes.First();
                int taskTime = GetTaskTime(nextNode.Key);
                taskManager.AddTask(nextNode.Key, taskTime);
                queuedNodes.Add(nextNode.Key);
                /// colors
                nextNode.Value.SetColor(Color.yellow);
                nextNode.Value.SetText(taskTime.ToString());

                yield return new WaitForSeconds(0.2f);
                continue;
            }

            yield return new WaitForSeconds(0.2f);

            List<Task> finishedTasks = taskManager.WaitForNextTask();
            foreach(KeyValuePair<string, int> task in taskManager.tasks)
            {
                nodes[task.Key].SetText(task.Value.ToString());
            }
            timeText.text = taskManager.totalTime.ToString();

            foreach(Task task in finishedTasks)
            {
                checkedNodes.Add(task.id);
                nodeController.RemoveParentFromChildren(task.id);
                nodes[task.id].SetColor(Color.black);
                nodes[task.id].HideText();
                yield return null;
            }

            if(checkedNodes.Count() == nodes.Count())
            {
                isFinished = true;
            }
        }

        print(taskManager.totalTime);

        yield break;
    }
    private int GetTaskTime(string id)
    {
        int taskTime = 0;
        foreach(char c in id)
        {
            taskTime += c - 4;
        }
        return taskTime;
    }

    public void Start()
    {
        nodeController = FindObjectOfType<Day7NodeController>();
        timeText.text = "0";
        StartCoroutine(Solution());
    }
}
