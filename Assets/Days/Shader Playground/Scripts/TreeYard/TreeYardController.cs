using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using ComputeShaderUtility;
using System.Linq;

public class TreeYardController : MonoBehaviour
{
    [SerializeField] ComputeShader _computeShader;
    [SerializeField] GameObject _plane;

    // Shader Kernels
    const int updateKernel = 0;
    const int initialiseKernel = 1;
    const int copyKernel = 2;
    const int colourKernel = 3;
    const int initialiseKernel2 = 4;

    // Buffer
    ComputeBuffer buffer = null;
    ComputeBuffer treesBuffer = null;
    ComputeBuffer yardsBuffer = null;
    //ComputeBuffer settingsBuffer = null;

    // Textures
    RenderTexture _stateTexture;
    RenderTexture _copyStateTexture;
    RenderTexture _displayTexture;

    Camera _cam;

    // Settings
    public TreeYardSettings settings;

    [Header("Display Settings")]
    public FilterMode filterMode = FilterMode.Point;
    public GraphicsFormat format = ComputeHelper.defaultGraphicsFormat;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        ComputeHelper.CreateRenderTexture(ref _stateTexture, settings.width, settings.height);
        ComputeHelper.CreateRenderTexture(ref _copyStateTexture, settings.width, settings.height);
        ComputeHelper.CreateRenderTexture(ref _displayTexture, settings.width, settings.height);

        char[][] input = InputHelper.ParseInputCharArray(18); // <-- fix puzzle day
                                                              // convert to vector2[] of points

        List<Vector2> treesList = new List<Vector2>();
        List<Vector2> yardsList = new List<Vector2>();

        for (int j = 0; j < input.Length; j++)
        {
            for (int i = 0; i < input[0].Length; i++)
            {
                if (input[j][i].Equals('|'))
                {
                    treesList.Add(new Vector2(i, j));
                }
                else if (input[j][i].Equals('#'))
                {
                    yardsList.Add(new Vector2(i, j));
                }
            }
        }

        Vector2[] trees = treesList.ToArray();
        Vector2[] yards = yardsList.ToArray();

        print($"trees length: {trees.Length}");
        print($"yards length: {yards.Length}");

        // shader values that should be set for the initial pass
        _computeShader.SetFloat("width", settings.width);
        _computeShader.SetFloat("height", settings.height);
        _computeShader.SetInt("treesLength", trees.Length);
        _computeShader.SetInt("yardsLength", yards.Length);
        _computeShader.SetTexture(initialiseKernel, "State", _stateTexture);
        _computeShader.SetTexture(initialiseKernel, "CopyState", _copyStateTexture);
        _computeShader.SetTexture(initialiseKernel2, "State", _stateTexture);

        // plug vector2[] of points into an initial shader kernel to set up the texture
        //int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector2));
        //buffer = new ComputeBuffer(trees.Length, stride);
        //buffer.SetData(trees);
        //_computeShader.SetBuffer(initialiseKernel, "trees", buffer);

        //buffer = new ComputeBuffer(yards.Length, stride);
        //buffer.SetData(yards);
        //_computeShader.SetBuffer(initialiseKernel, "yards", buffer);

        ComputeHelper.CreateAndSetBuffer<Vector2>(ref treesBuffer, trees, _computeShader, "trees", initialiseKernel);
        ComputeHelper.CreateAndSetBuffer<Vector2>(ref yardsBuffer, yards, _computeShader, "yards", initialiseKernel);

        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, initialiseKernel2);
        ComputeHelper.Dispatch(_computeShader, Mathf.Max(trees.Length, yards.Length), 1, 1, initialiseKernel);

        // plane to view texture on
        _plane.transform.localScale = (new Vector3(settings.width, 0, settings.height)).normalized + Vector3.up;
        _plane.GetComponent<MeshRenderer>().material.mainTexture = _displayTexture;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(_displayTexture, destination);
    }

    private void OnDestroy()
    {
        ComputeHelper.Release(treesBuffer, yardsBuffer, buffer);
    }

    private void FixedUpdate()
    {
        RunTick();
    }

    private void RunTick()
    {
        // set textures
        _computeShader.SetTexture(updateKernel, "State", _stateTexture);
        _computeShader.SetTexture(updateKernel, "CopyState", _copyStateTexture);
        _computeShader.SetTexture(copyKernel, "State", _stateTexture);
        _computeShader.SetTexture(copyKernel, "CopyState", _copyStateTexture);
        _computeShader.SetTexture(colourKernel, "State", _stateTexture);
        _computeShader.SetTexture(colourKernel, "Display", _displayTexture);

        // apply all settings
        _computeShader.SetInt("radiusNeighbourhood", settings.radiusNeighbourhood);

        _computeShader.SetVector("colourA", settings.colourA);
        _computeShader.SetVector("colourR", settings.colourR);
        _computeShader.SetVector("colourB", settings.colourB);
        _computeShader.SetVector("colourG", settings.colourG);

        _computeShader.SetFloats("openRequirements", settings.openOpen, settings.openTrees, settings.openYards);
        _computeShader.SetFloats("treeRequirements", settings.treeOpen, settings.treeTrees, settings.treeYards);
        _computeShader.SetFloats("yardRequirements", settings.yardOpen, settings.yardTrees, settings.yardYards);

        // dispatch to complete calculation of copy array
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, updateKernel);
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, copyKernel);
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, colourKernel);
    }
}
