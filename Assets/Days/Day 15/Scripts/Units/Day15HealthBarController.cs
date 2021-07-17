using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15HealthBarController : MonoBehaviour
{
    public Day15HealthBar healthBarPrefab;

    private Dictionary<Day15Health, Day15HealthBar> healthBars = new Dictionary<Day15Health, Day15HealthBar>();

    private void Awake()
    {
        Day15Health.OnHealthAdded += AddHealthBar;
        Day15Health.OnHealthRemoved += RemoveHealthBar;
    }

    private void AddHealthBar(Day15Health health)
    {
        if(!healthBars.ContainsKey(health))
        {
            var healthBar = Instantiate(healthBarPrefab, transform, false);
            healthBars.Add(health, healthBar);
            healthBar.SetHealth(health);
        }
    }

    private void RemoveHealthBar(Day15Health health)
    {
        if (healthBars.ContainsKey(health))
        {
            Destroy(healthBars[health].gameObject);
            healthBars.Remove(health);
        }
    }
}
