using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15Goblin : Day15Unit
{
    //public string unitName = "Goblin";
    //public int health = 200;
    //public int attack = 3;
    //public int faction = 0;
    //public int id;
    //public int readingOrder;
    //public Vector2Int pos;
    //public bool isDead = false;

    public override void Initialise(Vector2Int position)
    {
        unitName = "Goblin";
        SetHealth(200);
        attack = 3;
        faction = 1;
        id = nextId++;
        SetPosition(position);
    }
}
