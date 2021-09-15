using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

public class Day19Program
{
    public int[] register;
    public bool hasTerminated = false;

    private Day19VM vm;
    private int[][] operationList;
    private int pointerIndex;
    public int Pointer { get { return pointerIndex; } }

    public Day19Program(string[] input, int registerSize, Day19VM vm)
    {
        pointerIndex = int.Parse(input[0][4].ToString());
        operationList = input.Skip(1).Select(line =>
                                    {
                                        MatchCollection matches = Regex.Matches(line, "\\w+");
                                        var ints = matches.Cast<Match>().Skip(1).Select(n => int.Parse(n.Value)).ToArray();
                                        int[] op = { Day19VM.opMapping[matches[0].Value], ints[0], ints[1], ints[2]};
                                        return op;
                                    })
                                    .ToArray();

        register = new int[registerSize];
        this.vm = vm;
    }

    public int[] GetOperation()
    {
        return operationList[register[pointerIndex]];
    }

    public void PerformSteps(int steps)
    {
        for(int i = 0; i < steps; i++)
        {
            if(register[pointerIndex] >= operationList.Length) { hasTerminated = true; return; }

            int pointerValue = register[pointerIndex];
            vm.RunOpCode(this);
            register[pointerIndex]++;
        }
    }
}
