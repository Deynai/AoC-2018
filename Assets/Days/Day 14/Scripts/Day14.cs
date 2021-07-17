using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day14 : MonoBehaviour
{
    List<int> recipes;
    int elf1;
    int elf2;
    int target;

    int[] scoreSequence;


    private void Initialise()
    {
        recipes = new List<int>();
        recipes.Add(3);
        recipes.Add(7);
        elf1 = 0;
        elf2 = 1;
        target = int.Parse(InputHelper.ParseInputString(14));
    }

    private void Part1()
    {
        Initialise();

        while (recipes.Count < target + 11)
        {
            // generate new recipes and add them
            AddNewRecipes();
            // increment elf indices
            NextElfRecipes();
        }

        List<int> finalScores = recipes.Skip(target).Take(10).ToList();

        print($"{string.Join("", finalScores)}");
    }

    private void AddNewRecipes()
    {
        int sum = recipes[elf1] + recipes[elf2];
        Debug.Assert(sum >= 0 && sum < 19);
        if(sum > 9)
        {
            recipes.Add(1);
        }
        recipes.Add(sum % 10);
    }

    private void NextElfRecipes()
    {
        elf1 = (elf1 + recipes[elf1] + 1) % recipes.Count;
        elf2 = (elf2 + recipes[elf2] + 1) % recipes.Count;
    }

    private void Part2()
    {
        Initialise();

        scoreSequence = target.ToString().Select(c => int.Parse(c.ToString())).ToArray();
        bool foundSequence = false;

        //for (int i = 0; i < recipes.Count - scoreSequence.Length; i++)
        //{
        //    if (CheckScoreSequence(i))
        //    {
        //        print($"Found score sequence at index: {i}");
        //        foundSequence = true;
        //        break;
        //    }
        //}

        int bp = 0;
        int nextIndex = 0;
        while (!foundSequence)
        {
            if (bp++ > 100000) { print($"Hit bp"); break; }
            for (int i = 0; i < 1000; i++)
            {
                AddNewRecipes();
                NextElfRecipes();
            }

            for(int i = nextIndex; i < recipes.Count - scoreSequence.Length; i++)
            {
                if (CheckScoreSequence(i))
                {
                    print($"Found score sequence at index: {i}. Sequence: {string.Join("", recipes.Skip(i).Take(scoreSequence.Length))}");
                    foundSequence = true;
                    break;
                }
            }

            nextIndex = recipes.Count - scoreSequence.Length;
        }
    }

    private bool CheckScoreSequence(int index)
    {
        if(index < 0)
        {
            return false;
        }

        for(int i = 0; i < scoreSequence.Length; i++)
        {
            if(!recipes[i + index].Equals(scoreSequence[i]))
            {
                return false;
            }
        }

        return true;
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
