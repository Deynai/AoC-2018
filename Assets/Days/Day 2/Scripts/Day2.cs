using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day2 : MonoBehaviour
{
    public void Part1()
    {
        string[] input = InputHelper.ParseInputArray(2);

        int twos = 0;
        int threes = 0;

        foreach(string line in input)
        {
            Dictionary<char, int> letterCounts = new Dictionary<char, int>();
            foreach(char c in line)
            {
                if (letterCounts.ContainsKey(c))
                {
                    letterCounts[c]++;
                }
                else
                {
                    letterCounts.Add(c, 1);
                }
            }

            twos += letterCounts.Values.Contains(2) ? 1 : 0;
            threes += letterCounts.Values.Contains(3) ? 1 : 0;
        }

        print(twos * threes);
    }

    public void Part2()
    {
        string[] input = InputHelper.ParseInputArray(2);
        (int i, int j) match = (0,0);
        
        for(int i = 0; i < input.Length - 1; i++) {
            for(int j = i+1; j < input.Length; j++)
            {
                if(CompareStrings(input[i], input[j]) == 1)
                {
                    match = (i, j);
                }
            }
        }

        string matchTrimmed = "";
        for(int i = 0; i < input[match.i].Length; i++)
        {
            if(input[match.i][i] == input[match.j][i])
            {
                matchTrimmed += input[match.i][i];
            }
        }

        print(matchTrimmed);
    }

    private int CompareStrings(string a, string b)
    {
        int differences = 0;
        for(int i = 0; i < a.Length; i++)
        {
            if(a[i] != b[i])
            {
                differences++;
            }
        }
        return differences;
    }

    void Start()
    {
        Part1();
        Part2();
    }
}
