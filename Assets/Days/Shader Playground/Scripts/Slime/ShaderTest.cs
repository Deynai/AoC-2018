using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComputeShaderUtility;
using UnityEngine.Experimental.Rendering;

public class ShaderTest : MonoBehaviour
{
    [SerializeField] ComputeShader _computeShader;
    [SerializeField] GameObject _plane;

    // Shader Kernels
    const int updateKernel = 0;
    const int diffuseMapKernel = 1;
    const int colourKernel = 2;

    // Buffer
    ComputeBuffer buffer = null;
    //ComputeBuffer settingsBuffer = null;

    // Textures
    RenderTexture _renderTexture;
    RenderTexture _renderTextureDiffusion;
    RenderTexture _displayTexture;

    Camera _cam;

    // Settings
    public SlimeSettings settings;

    [Header("Display Settings")]
    public FilterMode filterMode = FilterMode.Point;
    public GraphicsFormat format = ComputeHelper.defaultGraphicsFormat;

    private void Awake()
    {
        _cam = Camera.main;
    }

    void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        ComputeHelper.CreateRenderTexture(ref _renderTexture, settings.width, settings.height);
        ComputeHelper.CreateRenderTexture(ref _renderTextureDiffusion, settings.width, settings.height);
        ComputeHelper.CreateRenderTexture(ref _displayTexture, settings.width, settings.height);

        Agent[] agents = new Agent[settings.numAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            Vector2 centre = new Vector2(settings.width / 2, settings.height / 2);
            Vector2 startPos = Vector2.zero;
            float randomAngle = Random.value * Mathf.PI * 2;
            float angle = 0;

            if (settings.spawnMode == 1)
            {
                startPos = centre;
                angle = randomAngle;
            }
            else if (settings.spawnMode == 2)
            {
                startPos = new Vector2(Random.Range(0, settings.width), Random.Range(0, settings.height));
                angle = randomAngle;
            }
            else if (settings.spawnMode == 3)
            {
                startPos = centre + Random.insideUnitCircle * settings.height * 0.5f;
                angle = Mathf.Atan2((centre - startPos).normalized.y, (centre - startPos).normalized.x);
            }
            else if (settings.spawnMode == 4)
            {
                startPos = centre + Random.insideUnitCircle * settings.height * 0.25f;
                angle = randomAngle;
            }

            agents[i] = new Agent(startPos, angle);
        }

        // Create and set buffer
        ComputeHelper.CreateAndSetBuffer(ref buffer, agents, _computeShader, "agents", updateKernel);

        if (settings.randomise)
        {
            RandomConditions();
        }

        _computeShader.SetInt("numAgents", settings.numAgents);
        _computeShader.SetFloat("width", settings.width);
        _computeShader.SetFloat("height", settings.height);

        //_plane.transform.localScale = (new Vector3(settings.width, 0, settings.height)).normalized + Vector3.up;
        //_plane.GetComponent<MeshRenderer>().material.mainTexture = _renderTexture;
    }

    private void ResetSim()
    {
        Initialise();
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
    

    private void FixedUpdate()
    {
        for(int i = 0; i < settings.stepsPerFrame; i++)
        {
            RunTick();
        }
    }

    void RunTick()
    {
        ComputeHelper.SetRenderTexture(_renderTexture, _computeShader, "TrailMap", diffuseMapKernel, updateKernel);
        ComputeHelper.SetRenderTexture(_renderTextureDiffusion, _computeShader, "DiffusedTrailMap", diffuseMapKernel, colourKernel);
        ComputeHelper.SetRenderTexture(_displayTexture, _computeShader, "Display", colourKernel);

        _computeShader.SetFloat("deltaTime", Time.deltaTime);
        _computeShader.SetFloat("time", Time.realtimeSinceStartup);

        _computeShader.SetFloat("trailWeight", settings.trailWeight);
        _computeShader.SetFloat("decayRate", settings.decayRate);
        _computeShader.SetFloat("diffusionRate", settings.diffuseRate);

        _computeShader.SetFloat("moveSpeed", settings.moveSpeed);
        _computeShader.SetFloat("turnSpeed", settings.turnSpeed);
        _computeShader.SetBool("mirrorReflect", settings.mirrorReflect);

        float _sensorAngleSpacing = settings.sensorAngleSpacing * 3.1416f / 180;
        _computeShader.SetFloat("sensorAngleRad", _sensorAngleSpacing);
        _computeShader.SetFloat("sensorOffsetDistance", settings.sensorOffsetDistance);
        _computeShader.SetFloat("sensorSize", settings.sensorSize);

        _computeShader.SetVector("colourR", settings.colourR);
        _computeShader.SetVector("colourOverlay", settings.colourOverlay);

        ComputeHelper.Dispatch(_computeShader, settings.numAgents, 1, 1, kernelIndex: updateKernel);
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, kernelIndex: diffuseMapKernel);
        ComputeHelper.Dispatch(_computeShader, settings.width, settings.height, 1, kernelIndex: colourKernel);

        ComputeHelper.CopyRenderTexture(_renderTextureDiffusion, _renderTexture);
    }

    private void OnDestroy()
    {
        buffer.Release();
    }

    public struct Agent
    {
        public Vector2 position;
        public float angle;

        public Agent(Vector2 pos, float angle)
        {
            this.position = pos;
            this.angle = angle;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //frameCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetSim();
            //frameCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            RandomConditions();
        }
    }
}
