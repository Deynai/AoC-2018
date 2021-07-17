using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15TestBase : MonoBehaviour
{
    public int health;

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
    }
}
