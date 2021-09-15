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

    [Header("Colour Settings")]
    public Color colourR;
    public Color colourOverlay;

    [Header("Random Generation Settings")]
    public bool randomise;
    public int fixedSeed;
    public bool useFixedSeed;


    public void RandomizeConditions(int seed)
    {
        System.Random prng = !useFixedSeed ? new System.Random(seed) : new System.Random(fixedSeed);
        Debug.Log($"Seed: {seed}");

        trailWeight = RandomRange(prng, 0.1f, 5f);
        decayRate = RandomRange(prng, 0f, 0.2f);
        diffuseRate = RandomRange(prng, 0f, 5f);
        moveSpeed = RandomRange(prng, 1f, 10f);
        turnSpeed = RandomRange(prng, 0f, 1.5f);
        sensorAngleSpacing = RandomRange(prng, 0f, 270f);
        sensorOffsetDistance = RandomRange(prng, 1f, 150f);
        sensorSize = prng.Next(1, 9);
    }

    private float RandomRange(System.Random prng, float min, float max)
    {
        float scale = max - min;
        float randomFloat = (float) prng.NextDouble();
        randomFloat *= scale;
        randomFloat += min;
        return randomFloat;
    }
}
