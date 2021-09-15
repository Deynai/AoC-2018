using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Tree/Yard Preset")]
public class TreeYardSettings : ScriptableObject {
    [Header("Sim Settings")]
    public int width = 960;
    public int height = 600;
    public int spawnMode = 1;

    [Header("Life Settings")]
    public int radiusNeighbourhood = 1;

    public int openOpen = 0;
    public int openTrees = 3;
    public int openYards = 0;

    public int treeOpen = 0;
    public int treeTrees = 0;
    public int treeYards = 3;

    public int yardOpen = 0;
    public int yardTrees = 1;
    public int yardYards = 1;

    [Header("Colours")]
    public Color colourA;
    public Color colourR;
    public Color colourB;
    public Color colourG;

    //[Header("Postprocessing")]
    //public float diffuseRate;
    //public float decayRate;

    //[Header("Channel Settings")]

    //// A to R/B/G
    //float aToR = 2f;
    //float aToB = 2f;
    //float aToG = 2f;

    //// R to A/B/G
    //float rToA = 2f;
    //float rToB = 2f;
    //float rToG = 2f;

    //// B to A/R/G
    //float bToA = 2f;
    //float bToR = 2f;
    //float bToG = 2f;

    //// G to A/R/B
    //float gToA = 2f;
    //float gToR = 2f;
    //float gToB = 2f;

}
