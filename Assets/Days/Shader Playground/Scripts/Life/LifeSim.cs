using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using ComputeShaderUtility;
using System.Linq;

public class LifeSim : MonoBehaviour
{
    [SerializeField] ComputeShader _computeShader;

    // Shader Kernels
    const int updateKernel = 0;
    const int initialiseKernel = 1;
    const int colourKernel = 2;

    // Buffer
    ComputeBuffer buffer = null;
    ComputeBuffer ruleBuffer = null;

    // Textures
    RenderTexture _stateTexture;
    RenderTexture _copyStateTexture;
    RenderTexture _displayTexture;

    Camera _cam;

    // Settings
    public LifeSettings settings;
    int frameCount = 0;

    private void Start()
    {
        Application.targetFrameRate = 60;
        Init();
    }

    private void Init()
    {
        ComputeHelper.CreateRenderTexture(ref _stateTexture, settings.width, settings.height);
        ComputeHelper.CreateRenderTexture(ref _copyStateTexture, settings.width, settings.height);
        ComputeHelper.CreateRenderTexture(ref _displayTexture, settings.width, settings.height);

        //// Constructs predetermined points
        //Vector2[] actives = new Vector2[settings.numStartPoints];
        //for(int i = 0; i < settings.numStartPoints; i++)
        //{
        //    Vector2 centre = new Vector2(settings.width / 2, settings.height / 2);
        //    Vector2 startPos = Vector2.zero;

        //    if(settings.spawnMode == 1)
        //    {
        //        startPos = centre + Random.insideUnitCircle * settings.height * 0.15f;
        //    }

        //    actives[i] = startPos;
        //}

        // generate random rules
        RandomConditions();

        // shader values that should be set for the initial pass
        _computeShader.SetFloat("width", settings.width);
        _computeShader.SetFloat("height", settings.height);
        _computeShader.SetVector("noiseOffset", settings.noiseOffset);
        //_computeShader.SetInt("activesLength", actives.Length);
        _computeShader.SetTexture(initialiseKernel, "State", _stateTexture);
        _computeShader.SetTexture(initialiseKernel, "CopyState", _copyStateTexture);

        // Buffers
        //ComputeHelper.CreateAndSetBuffer<Vector2>(ref buffer, actives, _computeShader, "actives", initialiseKernel);

        // Initialise Predetermined points
        //ComputeHelper.Dispatch(_computeShader, actives.Length, 1, 1, initialiseKernel);
    }

    private void ResetSim()
    {
        Init();
    }

    private void RandomConditions()
    {
        int rand = Random.Range(0, int.MaxValue);
        settings.RandomizeConditions(rand);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(_displayTexture, destination);
    }

    private void OnDestroy()
    {
        ComputeHelper.Release(ruleBuffer, buffer);
        ComputeHelper.Release(_stateTexture, _displayTexture, _copyStateTexture);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < settings.stepsPerFrame; i++)
        {
            RunTick();
        }
    }

    private void RunTick()
    {
        // set textures
        ComputeHelper.SetRenderTexture(_stateTexture, _computeShader, "State", updateKernel, colourKernel);
        ComputeHelper.SetRenderTexture(_copyStateTexture, _computeShader, "CopyState", updateKernel);
        ComputeHelper.SetRenderTexture(_displayTexture, _computeShader, "Display", colourKernel);

        ComputeHelper.CreateAndSetBuffer<LifeRule>(ref ruleBuffer, settings.rules, _computeShader, "rules", updateKernel);

        // apply all settings
        _computeShader.SetFloat("deltaTime", Time.deltaTime);
        _computeShader.SetFloat("time", Time.realtimeSinceStartup);
        _computeShader.SetInt("frameCount", frameCount);
        frameCount++;
        //_computeShader.SetFloat("innerRadius", settings.innerRadius);
        //_computeShader.SetFloat("outerRadius", settings.outerRadius);
        
        //// channel 1
        //_computeShader.SetFloats("birthInterval", settings.birth1, settings.birth2);
        //_computeShader.SetFloats("deathInterval", settings.death1, settings.death2);
        //_computeShader.SetFloats("alphaStep", settings.step1, settings.step2);
        //_computeShader.SetFloat("changeRate", settings.changeRate);

        _computeShader.SetVector("colourR", settings.colourR);

        // dispatch to complete calculation of copy array
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, updateKernel);
        ComputeHelper.CopyRenderTexture(_copyStateTexture, _stateTexture);
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, colourKernel);
    }
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            frameCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetSim();
            frameCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.C)){
            RandomConditions();
        }
    }
}
