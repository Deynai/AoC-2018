using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class InputHelper
{
    public static string ParseInputString(int day)
    {
        string path = $"{Application.dataPath}/Days/Day {day}/Input/input.txt";
        string input;
        try
        {
            input = File.ReadAllText(path).Trim();
        }
        catch (System.Exception e)
        {
            throw e;
        }
        return input;
    }

    public static string[] ParseInputArray(int day)
    {
        string path = $"{Application.dataPath}/Days/Day {day}/Input/input.txt";
        string[] input;
        try
        {
            input = File.ReadAllLines(path).Select(line => line.Trim()).ToArray();
        }
        catch (System.Exception e)
        {
            throw e;
        }
        return input;
    }

    public static List<string> ParseInputList(int day)
    {
        string path = $"{Application.dataPath}/Days/Day {day}/Input/input.txt";
        List<string> input;
        try
        {
            input = File.ReadAllLines(path).Select(line => line.Trim()).ToList();
        }
        catch (System.Exception e)
        {
            throw e;
        }
        return input;
    }

    public static char[][] ParseInputCharArray(int day)
    {
        string path = $"{Application.dataPath}/Days/Day {day}/Input/input.txt";
        char[][] input;
        try
        {
            input = File.ReadAllLines(path).Select(line => line.Trim().ToCharArray()).ToArray();
        }
        catch (System.Exception e)
        {
            throw e;
        }
        return input;
    }

    public static char[][] ParseInputCharArrayNoTrim(int day)
    {
        string path = $"{Application.dataPath}/Days/Day {day}/Input/input.txt";
        char[][] input;
        try
        {
            input = File.ReadAllLines(path).Select(line => line.ToCharArray()).ToArray();
        }
        catch (System.Exception e)
        {
            throw e;
        }
        return input;
    }
}
