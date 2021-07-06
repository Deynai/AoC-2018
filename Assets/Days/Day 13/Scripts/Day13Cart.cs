using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day13Cart : MonoBehaviour
{
    public bool hasCrashed = false;
    private int turnCount;
    public (int x, int y) pos;
    public int moveOrder { get { return (int)(transform.position.x + (transform.position.z * 1000)); } } // maybe inefficient for a lot of checks?

    public void Initialise(Vector3 pos, int facing)
    {
        transform.Rotate(Vector3.up * 90 * facing);
        transform.position = pos;
        this.pos = (Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        turnCount = 0;
    }

    public void MoveForward()
    {
        if (!hasCrashed)
        {
            transform.position += transform.forward;
            pos = (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }
    }

    public void IntersectionTurn()
    {
        switch (turnCount)
        {
            case 0: TurnRight();
                break;
            case 1:
                break;
            case 2: TurnLeft();
                break;
            default: Debug.Log($"Cart attempted invalid turn");
                break;
        }
        turnCount = (turnCount + 1) % 3;
    }

    public void TurnLeft()
    {
        if (!hasCrashed)
        {
            transform.Rotate(Vector3.up * -90);
        }
    }

    public void TurnRight()
    {
        if (!hasCrashed)
        {
            transform.Rotate(Vector3.up * 90);
        }
    }

    public void TurnA()
    {
        // a \ turn
        transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.z, 0, transform.forward.x));
    }

    public void TurnB()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(-transform.forward.z, 0, -transform.forward.x));
    }

    public void Crash()
    {
        hasCrashed = true;
        gameObject.SetActive(false);
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag.Equals("Cart"))
    //    {
    //        hasCrashed = true;
    //        gameObject.SetActive(false);
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 3);
    }
}
