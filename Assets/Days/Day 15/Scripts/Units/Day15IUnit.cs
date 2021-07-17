using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Day15IUnit
{
    public int Order();
    public int GetHealth();
    public int GetFaction();
    public int Id();
    public Vector2Int Position();
    public bool Dead();

    // set position
    public void SetPosition(Vector2Int pos);
    //{
    //    transform.position = new Vector3(pos.x, 0, -pos.y);
    //}

    // make the unit take damage
    public void TakeDamage(int damageTaken);
    //{
    //    health -= damageTaken;
    //    if(health <= 0)
    //    {
    //        RemoveUnit();
    //    }
    //}

    // may be ok if this is private
    public void DestroyUnit();
    //{
    //    health = 0;
    //    this.isAlive = false;
    //    gameObject.SetActive(false);
    //    // want to subscribe an event here so that when it dies, it removes itself from any lists it's on in the game manager
    //}
}
