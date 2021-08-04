
namespace ComputeShaderUtility
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Reflection;
    using UnityEngine.Experimental.Rendering;

    public static class ComputeHelper
    {
        public const FilterMode defaultFilterMode = FilterMode.Bilinear;
        public const GraphicsFormat defaultGraphicsFormat = GraphicsFormat.R16G16B16A16_SFloat; // GraphicsFormat.R8G8B8A8_Unorm;

        // Subscribe to this event to be notified when buffers created in edit mode should be released
        // (i.e before script compilation occurs, and when exitting edit mode)
        public static event System.Action shouldReleaseEditModeBuffers;

        public static void Dispatch(ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
        {
            Vector3Int threadGroupSizes = GetThreadGroupSizes(cs, kernelIndex);
            int numGroupsX = Mathf.CeilToInt(numIterationsX / (float)threadGroupSizes.x);
            int numGroupsY = Mathf.CeilToInt(numIterationsY / (float)threadGroupSizes.y);
            int numGroupsZ = Mathf.CeilToInt(numIterationsZ / (float)threadGroupSizes.z);
            cs.Dispatch(kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
        }

        public static Vector3Int GetThreadGroupSizes(ComputeShader compute, int kernelIndex = 0)
        {
            uint x, y, z;
            compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
            return new Vector3Int((int)x, (int)y, (int)z);
        }

        public static void CreateRenderTexture(ref RenderTexture texture, int width, int height)
        {
            CreateRenderTexture(ref texture, width, height, defaultFilterMode, defaultGraphicsFormat);
        }

        public static void CreateRenderTexture(ref RenderTexture texture, int width, int height, FilterMode filterMode, GraphicsFormat format)
        {
            texture = new RenderTexture(width, height, 0);
            texture.graphicsFormat = format;
            texture.enableRandomWrite = true;
            texture.autoGenerateMips = false;
            texture.Create();

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = filterMode;
        }

        public static void SetRenderTexture(RenderTexture texture, ComputeShader cs, string nameID, params int[] kernels)
        {
            for(int i = 0; i < kernels.Length; i++)
            {
                cs.SetTexture(kernels[i], nameID, texture);
            }
        }

        public static void CopyRenderTexture(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);
        }

        public static void CreateAndSetBuffer<T>(ref ComputeBuffer buffer, T[] data, ComputeShader cs, string nameID, int kernelIndex = 0)
        {
            int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

            bool createNewBuffer = buffer == null || !buffer.IsValid() || buffer.count != data.Length || buffer.stride != stride;
            if (createNewBuffer)
            {
                Release(buffer);
                buffer = new ComputeBuffer(data.Length, stride);
            }
            buffer.SetData(data);
            cs.SetBuffer(kernelIndex, nameID, buffer);
        }


        // releases buffers
        public static void Release(params ComputeBuffer[] buffers)
        {
            for(int i = 0; i < buffers.Length; i++)
            {
                if(buffers[i] != null)
                {
                    buffers[i].Release();
                }
            }
        }

        public static void Release(params RenderTexture[] textures)
        {
            for(int i = 0; i < textures.Length; i++)
            {
                if(textures[i] != null)
                {
                    textures[i].Release();
                }
            }
        }

        public static float[] PackFloats(params float[] values)
        {
            float[] packed = new float[values.Length * 4];
            for(int i = 0; i < values.Length; i++)
            {
                packed[i * 4] = values[i];
            }
            return values;
        }

        public static void SetParams(System.Object settings, ComputeShader shader, string variableNamePrefix="", string variableNameSuffix = "")
        {
            FieldInfo[] fields = settings.GetType().GetFields();

            foreach(FieldInfo field in fields)
            {
                var fieldType = field.FieldType;
                string shaderVariableName = variableNamePrefix + field.Name + variableNameSuffix;

                if(fieldType == typeof(Vector4) || fieldType == typeof(Vector3) || fieldType == typeof(Vector2))
                {
                    shader.SetVector(shaderVariableName, (Vector4)field.GetValue(settings));
                }
                else if(fieldType == typeof(int))
                {
                    shader.SetInt(shaderVariableName, (int)field.GetValue(settings));
                }
                else if(fieldType == typeof(float))
                {
                    shader.SetFloat(shaderVariableName, (float)field.GetValue(settings));
                }
                else if(fieldType == typeof(bool))
                {
                    shader.SetBool(shaderVariableName, (bool)field.GetValue(settings));
                }
                else
                {
                    Debug.Log($"Type {fieldType} not implemented.");
                }
            }
        }

        public static bool CanRunEditModeCompute
        {
            get
            {
                return CheckIfCanRunInEditMode();
            }
        }
        
#if UNITY_EDITOR
        static UnityEditor.PlayModeStateChange playModeState;

        static ComputeHelper()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= MonitorPlayModeState;
            UnityEditor.EditorApplication.playModeStateChanged += MonitorPlayModeState;

            UnityEditor.Compilation.CompilationPipeline.compilationStarted -= OnCompilationStarted;
            UnityEditor.Compilation.CompilationPipeline.compilationStarted += OnCompilationStarted;
        }

        static void MonitorPlayModeState(UnityEditor.PlayModeStateChange state)
        {
            playModeState = state;
            if(state == UnityEditor.PlayModeStateChange.ExitingEditMode)
            {
                if(shouldReleaseEditModeBuffers != null)
                {
                    shouldReleaseEditModeBuffers();
                }
            }
        }

        static void OnCompilationStarted(System.Object obj)
        {
            if(shouldReleaseEditModeBuffers != null)
            {
                shouldReleaseEditModeBuffers();
            }
        }
#endif

        static bool CheckIfCanRunInEditMode()
        {
            bool isCompilingOrExitingEditMode = false;
#if UNITY_EDITOR
            isCompilingOrExitingEditMode |= UnityEditor.EditorApplication.isCompiling;
            isCompilingOrExitingEditMode |= playModeState == UnityEditor.PlayModeStateChange.ExitingEditMode;
#endif
            bool canRun = !isCompilingOrExitingEditMode;
            return canRun;
        }
    }

}
