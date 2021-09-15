using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "LifeSettings")]
public class LifeSettings : ScriptableObject
{
	[Header("Sim Settings")]
	public int stepsPerFrame = 1;
    public int width = 960;
    public int height = 600;
    //public int spawnMode = 1;
    public Vector2 noiseOffset;
	public int maxRadius = 10;
	//[Min(1)] public int numStartPoints = 100;
	public int fixedSeed;
	public bool useFixedSeed = false;

    [Header("Neighbour Settings")]
    //public float innerRadius = 1f;
    //public float outerRadius = 27f;

    public LifeRule[] rules;
    public int numRules;

    //// Channel 1
    //[Range(0, 1)] public float birth1;
    //[Range(0, 1)] public float birth2;
    //[Range(0, 1)] public float death1;
    //[Range(0, 1)] public float death2;
    //[Range(0, 1)] public float step1;
    //[Range(0, 1)] public float step2;
    //public float changeRate = 0.5f;

    [Header("Colours")]
    public Color colourR;

	public void RandomizeConditions(int seed)
	{
		System.Random prng = !useFixedSeed ? new System.Random(seed) : new System.Random(fixedSeed);
		Debug.Log($"Seed: {seed}");
		if (rules == null || rules.Length != numRules)
		{
			rules = new LifeRule[numRules];
		}

		for (int i = 0; i < rules.Length; i++)
		{
			rules[i].radiusMinMax = RandomRadii(prng);
			rules[i].aliveMinMax = CalculateMinMaxPair(prng);
			rules[i].deadMinMax = CalculateMinMaxPair(prng);
		}
	}

	Vector2Int RandomRadii(System.Random prng)
	{
		int maxPossibleRadius = this.maxRadius;
		int radiusA = prng.Next(0, maxPossibleRadius);
		int radiusB = prng.Next(0, maxPossibleRadius);
		int minRadius = (radiusA < radiusB) ? radiusA : radiusB;
		int maxRadius = (radiusA > radiusB) ? radiusA : radiusB;
		return new Vector2Int(minRadius, maxRadius);
	}

	static Vector2 CalculateMinMaxPair(System.Random prng)
	{
		float a = (float)prng.NextDouble();
		float b = (float)prng.NextDouble();

		return a > b ? new Vector2(b, a) : new Vector2(a, b);
	}
}
