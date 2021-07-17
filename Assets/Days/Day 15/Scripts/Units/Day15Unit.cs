using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day15Unit : MonoBehaviour
{
    public static int nextId = 0;
    public string unitName = "Unassigned Unit";
    public int health;
    public int attack;
    public int faction;
    public int id;
    public Vector2Int pos;
    public bool isDead = false;
    private Day15Health healthB;

    public virtual void Initialise(Vector2Int position) { print("No unit initialisation method found"); }

    public int Order { get { return pos.x + 1000 * pos.y; } }

    void Awake()
    {
        healthB = GetComponent<Day15Health>();    
    }

    // set position
    public void SetPosition(Vector2Int pos)
    {
        this.pos = pos;
        transform.position = new Vector3(pos.x, transform.position.y, -pos.y);
    }
    public void SetHealth(int health)
    {
        this.health = health;
        GetComponent<Day15Health>().maxHealth = health;
    }

    // make the unit take damage
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        healthB.ModifyHealth(-damageTaken);
        if (health <= 0)
        {
            DestroyUnit();
        }
    }

    public void DestroyUnit()
    {
        this.health = 0;
        this.isDead = true;
        gameObject.SetActive(false);
        // want to subscribe an event here so that when it dies, it removes itself from any lists it's on in the game manager
    }

}
