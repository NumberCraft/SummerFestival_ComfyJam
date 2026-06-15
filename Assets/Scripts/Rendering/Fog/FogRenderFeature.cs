using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        class FogPass : ScriptableRenderPass
        {
            private Material m_Material;

            public FogPass(Material material, RenderPassEvent evt)
            {
                m_Material = material;
                renderPassEvent = evt;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                //Debug.Log("Fog start.");

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var stack = VolumeManager.instance.stack;
                var fog = stack.GetComponent<Fog>();

                if (m_Material == null || fog == null || !fog.IsActive() || !cameraData.postProcessEnabled)
                    return;

                TextureHandle source = resourceData.activeColorTexture;

                // CRITICAL: Ensure Depth is available
                TextureHandle depth = resourceData.activeDepthTexture;

                TextureDesc desc = renderGraph.GetTextureDesc(source);
                desc.name = "_TempFog";
                desc.clearBuffer = false;
                TextureHandle tempTex = renderGraph.CreateTexture(desc);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Fog Effect", out var passData))
                {
                    passData.material = m_Material;
                    passData.inputTexture = source;

                    // Transfer Volume values
                    passData.distance = fog.fogDistance.value; // Assuming your Volume component has 'fogDistance'
                    passData.density = fog.fogDensity.value;
                    passData.color = fog.fogColor.value;
                    passData.noiseScale = fog.noiseScale.value;
                    passData.noiseStrength = fog.noiseStrength.value;

                    builder.UseTexture(source);
                    // TELL RENDER GRAPH WE NEED DEPTH
                    builder.UseTexture(depth);

                    builder.SetRenderAttachment(tempTex, 0);
                    builder.AllowGlobalStateModification(true);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        // Ensure these match your shader property names exactly
                        data.material.SetFloat("_FogDensity", data.density);
                        // If your 'Fog' volume component has a distance/end property, pass it here:
                        // data.material.SetFloat("_FogDistance", data.fogDistance); 

                        data.material.SetFloat("_FogDensity", data.density);
                        data.material.SetFloat("_FogDistance", data.distance); // <-- IS THIS LINE MISSING?
                        data.material.SetColor("_FogColor", data.color);
                        data.material.SetFloat("_NoiseScale", data.noiseScale);
                        data.material.SetFloat("_NoiseStrength", data.noiseStrength);

                        Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), data.material, 0);
                    });
                }

                renderGraph.AddBlitPass(tempTex, source, Vector2.one, Vector2.zero);

                //Debug.Log("Fog end.");
            }
        }

        [SerializeField] public Shader fogShader;
        private FogPass m_Pass;
        public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

        public override void Create()
        {
            if (fogShader == null) return;
            Material mat = CoreUtils.CreateEngineMaterial(fogShader);
            m_Pass = new FogPass(mat, injectionPoint);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_Pass != null && renderingData.cameraData.cameraType == CameraType.Game)
                renderer.EnqueuePass(m_Pass);
        }
    }
}