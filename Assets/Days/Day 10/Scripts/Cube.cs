using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    Vector3 velocity;
    Vector3 position;
    CubeController cubeController;

    public void Initialise(Vector3 position, Vector3 velocity)
    {
        this.velocity = velocity;
        this.position = position;
    }

    void Start()
    {
        cubeController = FindObjectOfType<CubeController>();
    }

    void Update()
    {
        transform.position = position + velocity * cubeController.Step;
    }
}
