using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day1 : MonoBehaviour
{
    public void Part1()
    {
        List<string> input = InputHelper.ParseInputList(1);
        int startFrequency = 0;
        foreach(string freq in input)
        {
            startFrequency += int.Parse(freq);
        }
        Debug.Log(startFrequency);
    }

    public void Part2()
    {
        List<string> input = InputHelper.ParseInputList(1);
        HashSet<int> freqHistory = new HashSet<int>();
        bool foundDuplicate = false;

        int frequency = 0;
        freqHistory.Add(frequency);
        while (!foundDuplicate)
        {
            foreach(string freq in input)
            {
                frequency += int.Parse(freq);
                if (!freqHistory.Add(frequency))
                {
                    foundDuplicate = true;
                    break;
                }
            }
        }
        Debug.Log(frequency);
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
