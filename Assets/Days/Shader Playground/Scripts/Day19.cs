using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day19 : MonoBehaviour
{
    private void Part1()
    {
        string[] input = InputHelper.ParseInputArray(19);





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
        if (overrideOp == -1) { overrideOp = op[0]; }
        try
        {
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
        catch (System.Exception)
        {
            print($"Exception while performing operation on states.");
            return state;
        }
    }
}
