using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day11 : MonoBehaviour
{
    int[,] powerArray;
    int serialNumber;

    private void Part1()
    {
        powerArray = new int[300, 300];

        for(int i = 0; i < powerArray.GetLength(0); i++)
        {
            for(int j = 0; j < powerArray.GetLength(1); j++)
            {
                powerArray[i, j] = SetPowerLevel(i, j);
            }
        }

        int max = int.MinValue;
        (int x, int y) maxPos = (-1,-1);

        for(int i = 0; i < powerArray.GetLength(0) - 2; i++)
        {
            for(int j = 0; j < powerArray.GetLength(1) - 2; j++)
            {
                int value = SumPowerSquare(i, j, 3);
                if(value > max)
                {
                    max = value;
                    maxPos = (i, j);
                }
            }
        }

        print($"Max power of {max} found at location {maxPos.x + 1},{maxPos.y + 1}");
    }

    private void Part2()
    {
        int max = int.MinValue;
        (int x, int y, int size) maxPos = (-1, -1, -1);
        for (int size = 1; size < 301; size++)
        {
            (int x, int y, int max) sizeMax = MaxPowerSquareOfSize(size);
            if(sizeMax.max > max)
            {
                max = sizeMax.max;
                maxPos = (sizeMax.x, sizeMax.y, size);
            }
        }

        print($"Max power of {max} found at location {maxPos.x + 1},{maxPos.y + 1} with size {maxPos.size}");
    }

    private int SetPowerLevel(int x_index, int y_index)
    {
        int x = x_index + 1;
        int y = y_index + 1;

        long rackId = x + 10;
        long powerLevel = rackId * y;
        powerLevel += serialNumber;
        powerLevel = powerLevel * rackId;
        powerLevel = GetHundredsDigit(powerLevel);
        powerLevel -= 5;
        return (int) powerLevel;
    }

    private long GetHundredsDigit(long num)
    {
        long digit = num % 1000;
        digit = digit / 100;
        return digit;
    }

    private int SumPowerSquare(int x, int y, int size)
    {
        int power = 0;
        for(int i = x; i < x + size; i++)
        {
            for(int j = y; j < y + size; j++)
            {
                power += powerArray[i, j];
            }
        }
        return power;
    }

    private (int x, int y, int power) MaxPowerSquareOfSize(int size)
    {
        int max = int.MinValue;
        (int x, int y, int power) maxPos = (-1, -1, -1);

        int columnsSize = powerArray.GetLength(0) - size + 1;

        int[,] columns = new int[powerArray.GetLength(0), columnsSize];
        for(int i = 0; i < powerArray.GetLength(0); i++)
        {
            int firstColumn = 0;
            for(int j = 0; j < size; j++)
            {
                firstColumn += powerArray[i, j];
            }
            columns[i, 0] = firstColumn;

            for(int j = 1; j < columnsSize; j++)
            {
                firstColumn -= powerArray[i, j - 1];
                firstColumn += powerArray[i, j + size - 1];
                columns[i, j] = firstColumn;
            }
        }

        for(int j = 0; j < columnsSize; j++)
        {
            int firstSquare = 0;
            for(int i = 0; i < size; i++)
            {
                firstSquare += columns[i, j];
            }

            for(int i = 1; i < columnsSize; i++)
            {
                firstSquare -= columns[i - 1, j];
                firstSquare += columns[i + size - 1, j];
                if(firstSquare > max)
                {
                    max = firstSquare;
                    maxPos = (i, j, max);
                }
            }
        }

        return maxPos;
    }

    public void Start()
    {
        serialNumber = int.Parse(InputHelper.ParseInputString(11));

        Part1();
        Part2();
    }
}
