using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day19VM
{
    public static Dictionary<string, int> opMapping = new Dictionary<string, int>
    {
        { "addr", 0 },
        { "addi", 1 },
        { "mulr", 2 },
        { "muli", 3 },
        { "banr", 4 },
        { "bani", 5 },
        { "borr", 6 },
        { "bori", 7 },
        { "setr", 8 },
        { "seti", 9 },
        { "gtir", 10},
        { "gtri", 11},
        { "gtrr", 12},
        { "eqir", 13},
        { "eqri", 14},
        { "eqrr", 15}
    };
    List<string> debugLog = new List<string>();

    public Day19VM()
    {

    }

    // OPS - register: {0, 1, 2, 3}, op: {OP, A, B, C}
    // Addition
    private void addr(ref int[] register, int[] op)
    {
        // stores into register C the result of adding register A and register B.
        register[op[3]] = register[op[1]] + register[op[2]];
    }
    private void addi(ref int[] register, int[] op)
    {
        // stores into register C the result of adding register A and value B.
        register[op[3]] = register[op[1]] + op[2];
    }

    // Multiplication
    private void mulr(ref int[] register, int[] op)
    {
        // stores into register C the result of multiplying register A and register B.
        register[op[3]] = register[op[1]] * register[op[2]];
    }
    private void muli(ref int[] register, int[] op)
    {
        // stores into register C the result of multiplying register A and value B
        register[op[3]] = register[op[1]] * op[2];
    }

    // Bitwise AND
    private void banr(ref int[] register, int[] op)
    {
        // stores into register C the result of the bitwise AND of register A and register B.
        register[op[3]] = register[op[1]] & register[op[2]];
    }
    private void bani(ref int[] register, int[] op)
    {
        // stores into register C the result of the bitwise AND of register A and value B
        register[op[3]] = register[op[1]] & op[2];
    }

    // Bitwise OR
    private void borr(ref int[] register, int[] op)
    {
        // stores into register C the result of the bitwise OR of register A and register B.
        register[op[3]] = register[op[1]] | register[op[2]];
    }
    private void bori(ref int[] register, int[] op)
    {
        // stores into register C the result of the bitwise OR of register A and value B
        register[op[3]] = register[op[1]] | op[2];
    }

    // Assignment
    private void setr(ref int[] register, int[] op)
    {
        // copies the contents of register A into register C. (Input B is ignored.)
        register[op[3]] = register[op[1]];
    }
    private void seti(ref int[] register, int[] op)
    {
        // stores value A into register C. (Input B is ignored.)
        register[op[3]] = op[1];
    }

    // Greater-than Testing
    private void gtir(ref int[] register, int[] op)
    {
        // sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
        register[op[3]] = op[1] > register[op[2]] ? 1 : 0;
    }
    private void gtri(ref int[] register, int[] op)
    {
        // sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
        register[op[3]] = register[op[1]] > op[2] ? 1 : 0;
    }
    private void gtrr(ref int[] register, int[] op)
    {
        // sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
        register[op[3]] = register[op[1]] > register[op[2]] ? 1 : 0;
    }

    // Equality Testing
    private void eqir(ref int[] register, int[] op)
    {
        // sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
        register[op[3]] = op[1] == register[op[2]] ? 1 : 0;
    }
    private void eqri(ref int[] register, int[] op)
    {
        // sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
        register[op[3]] = register[op[1]] == op[2] ? 1 : 0;
    }
    private void eqrr(ref int[] register, int[] op)
    {
        // sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
        register[op[3]] = register[op[1]] == register[op[2]] ? 1 : 0;
    }

    private void RunOpCode(ref int[] register, int[] op, int overrideOp = -1)
    {
        if (overrideOp == -1) { overrideOp = op[0]; }
        try
        {
            switch (overrideOp)
            {
                case 0:
                    addr(ref register, op); return;
                case 1:
                    addi(ref register, op); return;
                case 2:
                    mulr(ref register, op); return;
                case 3:
                    muli(ref register, op); return;
                case 4:
                    banr(ref register, op); return;
                case 5:
                    bani(ref register, op); return;
                case 6:
                    borr(ref register, op); return;
                case 7:
                    bori(ref register, op); return;
                case 8:
                    setr(ref register, op); return;
                case 9:
                    seti(ref register, op); return;
                case 10:
                    gtir(ref register, op); return;
                case 11:
                    gtri(ref register, op); return;
                case 12:
                    gtrr(ref register, op); return;
                case 13:
                    eqir(ref register, op); return;
                case 14:
                    eqri(ref register, op); return;
                case 15:
                    eqrr(ref register, op); return;
                default:
                    debugLog.Add($"Invalid OP code");
                    return;
            }
        }
        catch (System.Exception)
        {
            debugLog.Add($"Exception while performing operation on states.");
            return;
        }
    }

    public void RunOpCode(Day19Program program)
    {
        RunOpCode(ref program.register, program.GetOperation());
    }
}
