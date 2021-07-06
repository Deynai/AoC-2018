using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day12PlantpotManager : MonoBehaviour
{
    public GameObject emptypot;
    public GameObject flowerpot;

    private (int index, bool flower)[] pots;
    private Dictionary<int, bool> rules;
    private long steps = 20;
    private List<int> sumHistory = new List<int>();

    private Dictionary<int, GameObject> potsObjects;

    private void LoadInput()
    {
        string[] input = InputHelper.ParseInputArray(12);

        string initialState = input[0].Substring(15);
        pots = new (int, bool)[initialState.Length];
        rules = new Dictionary<int, bool>();

        for (int i = 0; i < initialState.Length; i++)
        {
            pots[i] = (i, initialState[i].Equals('#'));
        }

        foreach(string line in input.Skip(2))
        {
            bool flower = line[9].Equals('#');
            int val = 0;
            for(int i = 0; i < 5; i++)
            {
                val += line[i].Equals('#') ? 1 << i : 0;
            }

            rules.Add(val, flower);
        }
    }

    private bool CheckFlower(int index)
    {
        int flowerVal = 0;
        for(int i = -2; i < 3; i++)
        {
            flowerVal += pots[index + i].flower ? 1 << i + 2 : 0;
        }
        return rules[flowerVal];
    }

    private void ExtendPots()
    {
        int extendAmount = 5;
        (int, bool)[] newPots = new (int, bool)[pots.Length + extendAmount*2];

        for(int i = 0; i < extendAmount; i++)
        {
            newPots[i] = (pots[0].index - extendAmount + i, false);
            newPots[newPots.Length - i - 1] = (pots[pots.Length - 1].index + extendAmount - i, false);
        }
        for(int i = 0; i < pots.Length; i++)
        {
            newPots[i + extendAmount] = pots[i];
        }

        pots = newPots;
    }
    private void ExtendUp()
    {
        int extendAmount = 5;
        (int, bool)[] newPots = new (int, bool)[pots.Length + extendAmount];

        for (int i = 0; i < extendAmount; i++)
        {
            newPots[newPots.Length - i - 1] = (pots[pots.Length - 1].index + extendAmount - i, false);
        }
        for (int i = 0; i < pots.Length; i++)
        {
            newPots[i] = pots[i];
        }

        pots = newPots;
    }
    private void ExtendDown()
    {
        int extendAmount = 5;
        (int, bool)[] newPots = new (int, bool)[pots.Length + extendAmount];

        for (int i = 0; i < extendAmount; i++)
        {
            newPots[i] = (pots[0].index - extendAmount + i, false);
        }
        for (int i = 0; i < pots.Length; i++)
        {
            newPots[i + extendAmount] = pots[i];
        }

        pots = newPots;
    }

    private void NextStep()
    {
        // Extend Array
        for(int i = 0; i < 3; i++)
        {
            if(pots[i].flower)
            {
                ExtendDown();
                break;
            }
        }
        for(int i = 0; i < 3; i++)
        {
            if(pots[pots.Length - i - 1].flower)
            {
                ExtendUp();
                break;
            }
        }

        (int, bool)[] nextPots = new (int, bool)[pots.Length];
        for(int i = 0; i < 3; i++)
        {
            nextPots[i] = pots[i];
            nextPots[nextPots.Length - i - 1] = pots[pots.Length - i - 1];
        }

        for(int i = 3; i < pots.Length - 2; i++)
        {
            nextPots[i] = (pots[i].index, CheckFlower(i));
        }

        pots = nextPots;
    }

    private IEnumerator Solution() 
    {
        yield return Part1();
        Part2();
        yield break;
    }

    private IEnumerator Part1()
    {
        LoadInput();

        DrawPots();
        yield return null;

        for (int i = 0; i < steps; i++)
        {
            NextStep();
            DrawPots();
            yield return new WaitForSeconds(0.1f);
        }

        print($"Value after {steps} steps is {SumPots()}");
        yield break;
    }

    private void Part2()
    {
        LoadInput();

        int numberOfSteps = 0;
        List<int> differenceHistory = new List<int>();
        int bp = 0;

        while (true)
        {
            if(bp++ > 10000) { break; }
            NextStep();
            numberOfSteps++;
            sumHistory.Add(SumPots());
            if(numberOfSteps > 5 && sumHistory[sumHistory.Count-1] - sumHistory[sumHistory.Count - 2] == sumHistory[sumHistory.Count - 2] - sumHistory[sumHistory.Count - 3])
            {
                break;
            }
        }

        int lastCount = sumHistory[sumHistory.Count - 1];
        int difference = lastCount - sumHistory[sumHistory.Count - 2];
        long remainingSteps = 50000000000L - numberOfSteps;
        print($"Identical differences found at step {numberOfSteps} with value {lastCount}. Adding {remainingSteps} more generations of difference {difference} gives a count of: {lastCount + remainingSteps * difference}");
    }

    private int SumPots()
    {
        int sum = 0;
        foreach ((int index, bool flower) pot in pots)
        {
            sum += pot.flower ? pot.index : 0;
        }
        return sum;
    }

    private void DrawPots()
    {
        for (int i = 0; i < pots.Length; i++)
        {
            if (!potsObjects.ContainsKey(pots[i].index))
            {
                GameObject newPot = Instantiate(pots[i].flower ? flowerpot : emptypot, transform, false);
                newPot.transform.position = new Vector3(pots[i].index, 0, 0);
                potsObjects.Add(pots[i].index, newPot);
            }
            else
            {
                Destroy(potsObjects[pots[i].index]);
                GameObject newPot = Instantiate(pots[i].flower ? flowerpot : emptypot, transform, false);
                newPot.transform.position = new Vector3(pots[i].index, 0, 0);
                potsObjects[pots[i].index] = newPot;
            }
        }
    }

    void Start()
    {
        potsObjects = new Dictionary<int, GameObject>();
        StartCoroutine(Solution());
    }

    void Update()
    {
        
    }
}
