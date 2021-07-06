using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#nullable enable
public class Day7TaskManager
{
    public Dictionary<string, int> tasks { get; }
    List<(string, int)> completedTaskHistory = new List<(string, int)>();
    public int totalTime { get; set; }

    public Day7TaskManager()
    {
        totalTime = 0;
        tasks = new Dictionary<string, int>();
    }

    public void AddTask(string id, int time)
    {
        if (!tasks.ContainsKey(id))
        {
            tasks.Add(id, time);
        }
    }
    public void AddTask(Task task)
    {
        if (!tasks.ContainsKey(task.id))
        {
            tasks.Add(task.id, task.time);
        }
    }

    public int Count()
    {
        return tasks.Count();
    }

    public List<Task> WaitForNextTask()
    {
        if(tasks.Count > 0)
        {
            Task nextTask = new Task(tasks.OrderBy(p => p.Value).First());
            totalTime += nextTask.time;

            string[] keys = tasks.Keys.ToArray();
            foreach(string id in keys)
            {
                tasks[id] -= nextTask.time;
            }
        }

        return GetFinishedTasks();
    }

    private List<Task> GetFinishedTasks()
    {
        List<Task> finishedTasks = tasks.Where(t => t.Value <= 0).Select(t => new Task(t)).ToList();
        foreach(Task finishedTask in finishedTasks)
        {
            tasks.Remove(finishedTask.id);
            completedTaskHistory.Add((finishedTask.id, totalTime));
        }
        return finishedTasks;
    }
}

public class Task
{
    public string id;
    public int time;

    public Task(string id, int time)
    {
        this.id = id;
        this.time = time;
    }

    public Task(KeyValuePair<string, int> kvp)
    {
        this.id = kvp.Key;
        this.time = kvp.Value;
    }
}
