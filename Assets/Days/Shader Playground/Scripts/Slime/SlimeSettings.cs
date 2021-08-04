using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()]
public class SlimeSettings : ScriptableObject
{
    [Header("Sim Settings")]
    [Min(1)] public int stepsPerFrame = 1;
    public int width = 1280;
    public int height = 720;
    public int numAgents = 100;
    public int spawnMode = 4;

    [Header("Trail Settings")]
    public float trailWeight = 1;
    public float decayRate = 1;
    public float diffuseRate = 1;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float turnSpeed;
    public bool mirrorReflect;

    [Header("Sensor Settings")]
    public float sensorAngleSpacing;
    public float sensorOffsetDistance;
    [Min(1)] public int sensorSize;
}
