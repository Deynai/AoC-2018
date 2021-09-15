using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text.RegularExpressions;

public class Day16 : MonoBehaviour
{
    int[][][] intOps;
    int[][] endTest;

    int[] opMapping = new int[16]; // maps op code to operation, i.e if 9 is muli() then opMapping[9] = 4 (3? -> 4th index)

    private void InitialiseInput()
    {
        string regex_numbers = "\\d+";

        string[] inputBlocks = InputHelper.ParseInputString(16).Replace("\r", "").Trim().Split(new string[] { "\n\n" }, StringSplitOptions.None);

        // intOps[block number][line number][int value]
        intOps = inputBlocks.Take(inputBlocks.Length - 2)
                                            .Select(block => block.Split('\n')
                                                                    .Select(line => {
                                                                        MatchCollection matches = Regex.Matches(line, regex_numbers);
                                                                        return matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
                                                                    })
                                                                    .ToArray())
                                            .ToArray();

        
        endTest = inputBlocks.Last().Split('\n')
                                        .Select(line => {
                                            MatchCollection matches = Regex.Matches(line, regex_numbers);
                                            return matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
                                        })
                                        .ToArray();
    }

    private void Part1()
    {
        (int op, List<int> similarOps)[] testedOps = intOps.Select(block => TestAllOps(block)).ToArray();
        var groupedOps = testedOps.GroupBy(count => count.similarOps.Count); //.Where(matchingOpsCount => matchingOpsCount >= 3).Count();
        var moreThanThreeOps = testedOps.Where(x => x.similarOps.Count >= 3).Count();
        print($"Part 1: Test Ops with more than 3 possibilities {moreThanThreeOps}");

        // part 2 setup
        // group by each operation, and intersect each set of similar operations to find possible candidates
        var similarOpsGroups = testedOps.GroupBy(x => x.op)
                                        .Select(group => (
                                                            op: group.Key,
                                                            candidates: group.Select(ops => ops.similarOps)
                                                                                .Skip(1)
                                                                                .Aggregate(group.Select(f => f.similarOps).First().ToList(),
                                                                                                            (h, e) => h.Intersect(e).ToList()
                                                                                           )
                                                         )
                                        )
                                        .OrderBy(matches => matches.candidates.Count())
                                        .ToList();

        // if an op has only one candidate, that must be the operation, and we can eliminate that as a possibility from all other ops
        // iterating until each op has only one candidate gives a complete mapping

        // can definitely do this in a more direct and efficient way, but this brute forcing works fine for n = 16
        int singles = 0;
        int bp = 0;
        while (!singles.Equals(16))
        {
            singles = 0;
            if(bp++ > 50) { print($"BP hit: Cannot reduce possible ops further."); break; }
            for(int i = 0; i < similarOpsGroups.Count; i++)
            {
                if (similarOpsGroups[i].candidates.Count.Equals(1))
                {
                    singles++;
                    for(int j = 0; j < similarOpsGroups.Count; j++)
                    {
                        if (!i.Equals(j))
                        {
                            similarOpsGroups[j] = (similarOpsGroups[j].op, similarOpsGroups[j].candidates.Except(similarOpsGroups[i].candidates).ToList());
                        }
                    }
                }
            }
        }

        // set the mapping and we're now ready to compute part 2
        foreach(var group in similarOpsGroups.OrderBy(p => p.op))
        {
            Debug.Assert(group.candidates.Count == 1);
            opMapping[group.op] = group.candidates.First();
        }

        print($"{string.Join(",", opMapping)}");
    }

    private void Part2()
    {
        int[] currentState = new int[4] { 0, 0, 0, 0 };

        foreach(int[] operation in endTest)
        {
            currentState = RunOpCode(currentState, operation, opMapping[operation[0]]);
        }

        print($"Final register values: {string.Join(",", currentState)}");
    }

    // OPS - register: {0, 1, 2, 3}, op: {OP, A, B, C}
    // Addition
    private int[] addr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of adding register A and register B.
        output[op[3]] = state[op[1]] + state[op[2]];
        return output;
    }

    private int[] addi(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of adding register A and value B.
        output[op[3]] = state[op[1]] + op[2];
        return output;
    }

    // Multiplication
    private int[] mulr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of multiplying register A and register B.
        output[op[3]] = state[op[1]] * state[op[2]];
        return output;
    }
    private int[] muli(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of multiplying register A and value B
        output[op[3]] = state[op[1]] * op[2];
        return output;
    }

    // Bitwise AND
    private int[] banr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of the bitwise AND of register A and register B.
        output[op[3]] = state[op[1]] & state[op[2]];
        return output;
    }
    private int[] bani(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of the bitwise AND of register A and value B
        output[op[3]] = state[op[1]] & op[2];
        return output;
    }

    // Bitwise OR
    private int[] borr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of the bitwise OR of register A and register B.
        output[op[3]] = state[op[1]] | state[op[2]];
        return output;
    }
    private int[] bori(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores into register C the result of the bitwise OR of register A and value B
        output[op[3]] = state[op[1]] | op[2];
        return output;
    }

    // Assignment
    private int[] setr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // copies the contents of register A into register C. (Input B is ignored.)
        output[op[3]] = state[op[1]];
        return output;
    }
    private int[] seti(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // stores value A into register C. (Input B is ignored.)
        output[op[3]] = op[1];
        return output;
    }

    // Greater-than Testing
    private int[] gtir(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
        output[op[3]] = op[1] > state[op[2]] ? 1 : 0;
        return output;
    }
    private int[] gtri(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
        output[op[3]] = state[op[1]] > op[2] ? 1 : 0;
        return output;
    }
    private int[] gtrr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
        output[op[3]] = state[op[1]] > state[op[2]] ? 1 : 0;
        return output;
    }

    // Equality Testing
    private int[] eqir(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
        output[op[3]] = op[1] == state[op[2]] ? 1 : 0;
        return output;
    }
    private int[] eqri(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
        output[op[3]] = state[op[1]] == op[2] ? 1 : 0;
        return output;
    }
    private int[] eqrr(int[] state, int[] op)
    {
        int[] output = state.ToArray();
        // sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
        output[op[3]] = state[op[1]] == state[op[2]] ? 1 : 0;
        return output;
    }

    private int[] RunOpCode(int[] state, int[] op, int overrideOp = -1)
    {
        if(overrideOp == -1) { overrideOp = op[0]; }

        switch (overrideOp)
        {
            case 0: 
                return addr(state, op);
            case 1:
                return addi(state, op);
            case 2:
                return mulr(state, op);
            case 3:
                return muli(state, op);
            case 4:
                return banr(state, op);
            case 5:
                return bani(state, op);
            case 6:
                return borr(state, op);
            case 7:
                return bori(state, op);
            case 8:
                return setr(state, op);
            case 9:
                return seti(state, op);
            case 10:
                return gtir(state, op);
            case 11:
                return gtri(state, op);
            case 12:
                return gtrr(state, op);
            case 13:
                return eqir(state, op);
            case 14:
                return eqri(state, op);
            case 15:
                return eqrr(state, op);
            default:
                print($"Invalid OP code");
                return state;
        }
    }

    private bool CheckEqualState(int[] state, int[] state2)
    {
        for(int i = 0; i < 4; i++)
        {
            if(state[i] != state2[i])
            {
                return false;
            }
        }
        return true;
    }

    private (int op, List<int> similarOps) TestAllOps(int[][] testBlock)
    {
        //int matchingOpsCount = 0;
        int givenOp = testBlock[1][0];
        List<int> similarOps = new List<int>();

        for(int op = 0; op < 16; op++)
        {
            int[] opState = RunOpCode(testBlock[0], testBlock[1], op);
            if(CheckEqualState(opState, testBlock[2]))
            {
                //matchingOpsCount++;
                similarOps.Add(op);
            }
        }

        return (givenOp, similarOps);
    }

    void Start()
    {
        InitialiseInput();

        Part1();

        Part2();
    }
}
