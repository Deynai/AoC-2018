using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class Day9 : MonoBehaviour
{
    int playerCount;
    int finalMarble;

    private void Part1()
    {
        MatchCollection inputMatches = Regex.Matches(InputHelper.ParseInputString(9), "\\d+");
        playerCount = int.Parse(inputMatches[0].Value);
        finalMarble = int.Parse(inputMatches[1].Value);

        Day9MarbleManager marbleManager = new Day9MarbleManager(playerCount, finalMarble);
        marbleManager.RunGameLL();
        print(marbleManager.Scores.Max());
    }

    private void Part2()
    {
        Day9MarbleManager marbleManager = new Day9MarbleManager(playerCount, finalMarble * 100);
        marbleManager.RunGameLL();
        print(marbleManager.Scores.Max());
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
