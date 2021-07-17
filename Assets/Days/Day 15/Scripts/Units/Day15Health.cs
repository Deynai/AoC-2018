using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Day15Health : MonoBehaviour
{
    public static event Action<Day15Health> OnHealthAdded = delegate { };
    public static event Action<Day15Health> OnHealthRemoved = delegate { };

    public int maxHealth = 200;

    public int CurrentHealth { get; private set; }

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
        OnHealthAdded(this);
    }

    public void ModifyHealth(int amount)
    {
        CurrentHealth += amount;

        float currentHealthPct = (float)CurrentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }

    private void OnDisable()
    {
        OnHealthRemoved(this);
    }

}
