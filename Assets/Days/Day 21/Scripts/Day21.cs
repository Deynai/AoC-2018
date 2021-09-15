using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day21 : MonoBehaviour
{
    Day21Program program;

    // Start is called before the first frame update
    void Start()
    {
        //program = new Day21Program(InputHelper.ParseInputArray(21), 6, new Day21VM());

        Part1();
    }

    private void Part1()
    {
        
        program = new Day21Program(InputHelper.ParseInputArray(21), 6, new Day21VM());
        //program.register[0] = 4682012;
        HashSet<int> seenValues = new HashSet<int>();

        for(long i = 0; i < 3000000000; i++)
        {
            // operation 28 is the only one that uses register[0]
            if(program.register[4] == 28)
            {
                program.PrintRegister();
                if (seenValues.Contains(program.register[1]))
                {
                    print($"Repeated value: {program.register[1]}");
                    break;
                }
                seenValues.Add(program.register[1]);
            }
            program.PerformSteps(1);
        }
    }
}
