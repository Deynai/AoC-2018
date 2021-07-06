using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class CubeController : MonoBehaviour
{
    public GameObject cube;
    public int Step = 0;

    private int skipSteps;
    private List<Cube> cubes;
    private int[,] cubeValues;

    private void Part1()
    {
        cubes = new List<Cube>();

        for (int i = 0; i < cubeValues.GetLength(0); i++)
        {
            GameObject newCube = Instantiate(cube, transform, false);
            newCube.GetComponent<Cube>().Initialise(new Vector3(cubeValues[i,0] + (cubeValues[i, 2] * skipSteps), 0, -cubeValues[i, 1] - (cubeValues[i, 3] * skipSteps)), new Vector3(cubeValues[i, 2], 0, -cubeValues[i, 3]));
            cubes.Add(newCube.GetComponent<Cube>());
        }

        print($"Steps required: {Step}");
    }

    private void LoadData()
    {
        string[] inputConditions = InputHelper.ParseInputArray(10);
        cubeValues = new int[inputConditions.Length, 4];

        for(int i = 0; i < inputConditions.Length; i++)
        {
            MatchCollection matches = Regex.Matches(inputConditions[i], "-?\\d+");
            for(int k = 0; k < 4; k++)
            {
                cubeValues[i, k] = int.Parse(matches[k].Value);
            }
        }

        FindStepsToSkip();
    }
    
    private void FindStepsToSkip()
    {
        int bestIndex = FindBestIndex(0, 1000);
        bestIndex = FindBestIndex(bestIndex, 10);
        bestIndex = FindBestIndex(bestIndex, 1);
        Step = bestIndex;
    }

    private int FindBestIndex(int startIndex, int precision)
    {
        int bestIndex = startIndex;
        double diff = MeanSquared(startIndex);
        double nextdiff = MeanSquared(startIndex + precision);
        int bp = 0;

        while(nextdiff < diff)
        {
            if(bp++ > 10000) { return startIndex - precision + 1; }
            startIndex += precision;
            diff = nextdiff;
            nextdiff = MeanSquared(startIndex + precision);
        }

        return startIndex - precision + 1;
    }

    private double MeanSquared(int steps)
    {
        float x = 0;
        float y = 0;
        double meanDiff = 0;

        for(int i = 0; i < cubeValues.GetLength(0); i++)
        {
            x += cubeValues[i, 0] + cubeValues[i, 2] * steps;
            y += cubeValues[i, 1] + cubeValues[i, 3] * steps;
        }

        x = x / cubeValues.GetLength(0);
        y = y / cubeValues.GetLength(0);

        for(int i = 0; i < cubeValues.GetLength(0); i++)
        {
            meanDiff += Mathf.Sqrt(Mathf.Pow(x - (cubeValues[i, 0] + cubeValues[i, 2] * steps), 2) + Mathf.Pow(y - (cubeValues[i, 1] + cubeValues[i, 3] * steps), 2));
        }

        return meanDiff;
    }
    
    void Start()
    {
        LoadData();
        Part1();
    }
}
