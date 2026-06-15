using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        class DitheringPass : ScriptableRenderPass
        {
            private Material m_Material;

            public DitheringPass(Material material, RenderPassEvent evt)
            {
                m_Material = material;
                renderPassEvent = evt;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                //Debug.Log("Dithering start.");

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var stack = VolumeManager.instance.stack;
                var dithering = stack.GetComponent<Dithering>();

                if (m_Material == null || dithering == null || !dithering.IsActive() || !cameraData.postProcessEnabled)
                    return;

                TextureHandle source = resourceData.activeColorTexture;

                // Create temp texture matching the screen
                TextureDesc desc = renderGraph.GetTextureDesc(source);
                desc.name = "_TempDithering";
                desc.clearBuffer = false;
                TextureHandle tempTex = renderGraph.CreateTexture(desc);

                // --- PASS 1: Apply Dither from Source to Temp ---
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Dithering Effect", out var passData))
                {
                    passData.material = m_Material;
                    passData.inputTexture = source;

                    passData.patternIndex = dithering.patternIndex.value;
                    passData.threshold = dithering.ditherThreshold.value;
                    passData.strength = dithering.ditherStrength.value;
                    passData.scale = dithering.ditherScale.value;

                    builder.UseTexture(source);
                    builder.SetRenderAttachment(tempTex, 0);
                    builder.AllowGlobalStateModification(true);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        data.material.SetInt("_PatternIndex", data.patternIndex);
                        data.material.SetFloat("_DitherThreshold", data.threshold);
                        data.material.SetFloat("_DitherStrength", data.strength);
                        data.material.SetFloat("_DitherScale", data.scale);

                        Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), data.material, 0);
                    });
                } // <--- The Raster Pass ends HERE

                // --- PASS 2: Copy Temp back to Source ---
                // This is now OUTSIDE the using block, so it won't throw the InvalidOperationException
                renderGraph.AddBlitPass(tempTex, source, Vector2.one, Vector2.zero);

                //Debug.Log("Dithering end.");
            }

            private class PassData
            {
                public Material material;
                public TextureHandle inputTexture;
                public int patternIndex;
                public float threshold, strength, scale;
            }
        }

        [SerializeField] public Shader ditherShader;
        private DitheringPass m_Pass;
        public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

        public override void Create()
        {
            if (ditherShader == null) return;
            Material mat = CoreUtils.CreateEngineMaterial(ditherShader);
            m_Pass = new DitheringPass(mat, injectionPoint);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_Pass != null && renderingData.cameraData.cameraType == CameraType.Game)
                renderer.EnqueuePass(m_Pass);
        }
    }
}