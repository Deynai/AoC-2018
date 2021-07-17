using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15TestChild : Day15TestBase
{
    //public new int health = 200;

    private int updateCount = 200;
    private int count = 0;

    private void PrintHealth()
    {
        print($"Current Health: {health}");
    }

    private void Start()
    {
        health = 200;        
    }

    private void Update()
    {
        count++;
        if(count % updateCount == 0)
        {
            PrintHealth();
        }
    }
}
