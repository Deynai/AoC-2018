using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class Day19 : MonoBehaviour
{
    private void Start()
    {
        string[] input = InputHelper.ParseInputArray(19);
        Day19Program program = new Day19Program(input, 6, new Day19VM());

        Part1(program);
        Part2(program);
    }

    private void Part1(Day19Program program)
    {
        program.PerformSteps(10000000);

        if (program.hasTerminated) { print($"Has finished Part 1"); }
        print($"Final Register: {string.Join(",", program.register)}");
    }

    private void Part2(Day19Program program)
    {
        program.register = new int[]{ 1, 0, 0, 0, 0, 0 };
        program.hasTerminated = false;

        int bp = 0;
        bool startAdjusting = true;
        bool manualAdjust = false;

        // This program is effectively summing the factors of a number, E.
        /*

        int A, B, C, D, E, F;
        A = 0; B = 2; C = 1; D = 1; E = 10551358; F = 1;

        while (true)
        {
            // Check if D * F == E
            C = D * F;
            if(C == F)
            {
                A += F; // if it is, add F to A.
            }
            D++;
            if(D > E)
            {
                F = F + 1;
                if(F > E)
                {
                    return;
                }
                else
                {
                    D = 1;
                }
            }
        }

        // Or concisely:

        for(int D = 1; D < E; D++)
        {
            for(int F = 1; F < E; F++)
            {
                if(D * F == E)
                {
                    A += F;
                }
            }
        }

        We aim to improve it by effectively doing:
        
        for(int F = 1; F < Sqrt(E) + 1; i++){
            D = (int) E / F;
            if(D * F == E)
            {
                A += (D + F);
            }
        }

        */

        while (!program.hasTerminated)
        {
            if (bp++ > 100000) { print($"Hit Breakpoint"); break; }

            if(startAdjusting && program.register[program.Pointer] == 3)
            {
                manualAdjust = true;
                startAdjusting = false;
            }

            if (manualAdjust)
            {
                // Op 3: C = D*F, Op 4 checks if C == E. Let's skip ahead and just set D = E / F
                if(program.register[program.Pointer] == 3)
                {
                    program.register[3] = program.register[4] / program.register[5];
                    manualAdjust = false;
                    continue;
                }

                // Op 7: If C == E then we set A = A + F. Let's make it A = A + D + F and get both numbers in the pair.
                if(program.register[program.Pointer] == 7)
                {
                    program.register[0] += program.register[3];
                    manualAdjust = false;
                    continue;
                }
                // Op 8: D = D + 1. This is the D increment, since we are immediately jumping to the right value of D, we can set D to be E + 1 and end the loop early.
                if(program.register[program.Pointer] == 8)
                {
                    program.register[3] = program.register[4] + 1;
                    manualAdjust = false;
                    continue;
                }
                // Op 13: B += F > E ? 1 : 0. We can't easily skip incrementing F, but since we are adding both D and F we know we can stop when F > Sqrt(E).
                if(program.register[program.Pointer] == 13)
                {
                    if (program.register[5] > Mathf.CeilToInt(Mathf.Sqrt(program.register[4]))){
                        program.register[5] = program.register[4] + 1;
                    }
                    manualAdjust = false;
                    continue;
                }
            }

            if (!startAdjusting) { manualAdjust = true; }
            program.PerformSteps(1);
            //print($"{string.Join(",", program.register)}");
        }

        if (program.hasTerminated) { print($"Has finished Part 2"); }
        print($"Final Register: {string.Join(",", program.register)}");
    }
}
