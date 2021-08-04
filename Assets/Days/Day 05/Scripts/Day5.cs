using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class Day5 : MonoBehaviour
{
    private void Part1()
    {
        string input = InputHelper.ParseInputString(5);
        print(ReducePolymer(input));
    }

    private void Part2()
    {
        string input = InputHelper.ParseInputString(5);
        char[] letters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        List<(char, int)> lengths = new List<(char, int)>();
        foreach(char c in letters)
        {
            string inputCopy = Regex.Replace(input, $"{c}|{char.ToUpper(c)}", "");
            lengths.Add((c, ReducePolymer(inputCopy)));
        }

        (char c, int length) minPoly = lengths.OrderBy(p => p.Item2).First();
        print($"Minimum Polymer Length of {minPoly.length} by removing {minPoly.c}|{char.ToUpper(minPoly.c)}");
    }

    private int ReducePolymer(string polymer)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < polymer.Length; i++)
        {
            if (sb.Length > 0 && CheckCollision(sb[sb.Length - 1], polymer[i]))
            {
                // if there's a collision, remove the last element
                sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                // if there isn't, add it to the stringbuilder
                sb.Append(polymer[i]);
            }
        }

        return sb.Length;
    }

    private bool CheckCollision(char a, char b)
    {
        // return true if a,b collide, else false
        // A = 65, a = 97
        if(a == b + 32 || a == b - 32)
        {
            return true;
        }
        return false;
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
