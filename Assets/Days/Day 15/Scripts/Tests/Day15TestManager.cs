using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15TestManager : MonoBehaviour
{
    public GameObject testUnit;

    private Day15TestBase unit;
    private float attackTime = 2.0f;
    private float timer = 0.0f;

    private void Attack()
    {
        unit.TakeDamage(10);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject newUnit = Instantiate(testUnit, this.transform, false);
        unit = newUnit.GetComponent<Day15TestChild>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= attackTime)
        {
            timer = 0;
            Attack();
        }
    }
}
