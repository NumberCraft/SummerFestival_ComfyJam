using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        class PixelationPass : ScriptableRenderPass
        {
            private Material m_Material;
            // Most older shaders use _MainTex. Unity 6 Blitter uses _BlitTexture.
            // We will set BOTH to be safe.
            private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
            private static readonly int BlitTexId = Shader.PropertyToID("_BlitTexture");

            private static readonly int WidthId = Shader.PropertyToID("_WidthPixelation");
            private static readonly int HeightId = Shader.PropertyToID("_HeightPixelation");
            private static readonly int PrecisionId = Shader.PropertyToID("_ColorPrecision");

            public PixelationPass(Material material, RenderPassEvent evt)
            {
                m_Material = material;
                renderPassEvent = evt;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                //Debug.Log("Pixelation start.");

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var stack = VolumeManager.instance.stack;
                var pixelation = stack.GetComponent<Pixelation>();

                if (m_Material == null || pixelation == null || !pixelation.IsActive() || !cameraData.postProcessEnabled)
                    return;

                // In Unity 6, this is the current "Screen" texture
                TextureHandle source = resourceData.activeColorTexture;

                // Create a temporary texture that matches the screen exactly
                TextureDesc desc = renderGraph.GetTextureDesc(source);
                desc.filterMode = FilterMode.Point; // Makes the pixel edges crisp
                desc.name = "_TempPixelation";
                desc.clearBuffer = false;
                TextureHandle tempTex = renderGraph.CreateTexture(desc);

                // PASS 1: Copy the current screen into our Temp Texture
                // This replaces your old 'cmd.Blit(source, destination)'
                RenderGraphUtils.AddBlitPass(renderGraph, source, tempTex, Vector2.one, Vector2.zero);

                // PASS 2: Draw from Temp back to Screen using the Pixelation Shader
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("Pixelation Effects", out var passData))
                {
                    passData.material = m_Material;
                    passData.inputTexture = tempTex;
                    passData.w = pixelation.widthPixelation.value;
                    passData.h = pixelation.heightPixelation.value;
                    passData.precision = pixelation.colorPrecision.value;

                    builder.UseTexture(passData.inputTexture);
                    builder.SetRenderAttachment(source, 0);

                    // Crucial: Allows us to change Material floats inside the RenderFunc
                    builder.AllowGlobalStateModification(true);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        data.material.SetFloat("_WidthPixelation", data.w);
                        data.material.SetFloat("_HeightPixelation", data.h);
                        data.material.SetFloat("_ColorPrecision", data.precision);

                        // In Unity 6, Blitter.BlitTexture automatically binds the source 
                        // texture to _BlitTexture in the shader.
                        Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), data.material, 0);
                    });
                }

                //Debug.Log("Pixelation end.");
            }

            private class PassData
            {
                public Material material;
                public TextureHandle inputTexture;
                public float w, h, precision;
            }
        }

        [SerializeField] public Shader pixelationShader;
        private PixelationPass m_Pass;
        public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

        public override void Create()
        {
            if (pixelationShader == null) return;
            Material mat = CoreUtils.CreateEngineMaterial(pixelationShader);
            m_Pass = new PixelationPass(mat, injectionPoint);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_Pass != null && renderingData.cameraData.cameraType == CameraType.Game)
                renderer.EnqueuePass(m_Pass);
        }
    }

    /*public class PixelationRenderFeature : ScriptableRendererFeature
    {
        class PixelationPass : ScriptableRenderPass
        {
            private static readonly string shaderPath = "PostEffect/Pixelation";
            static readonly string k_RenderTag = "Render Pixelation Effects";
            static readonly int MainTexId = Shader.PropertyToID("_MainTex");
            static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelation");

            //PROPERTIES
            static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
            static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
            static readonly int ColorPrecison = Shader.PropertyToID("_ColorPrecision");


            Pixelation pixelation;
            Material pixelationMaterial;

            public PixelationPass(RenderPassEvent evt)
            {
                renderPassEvent = evt;
                var shader = Shader.Find(shaderPath);
                if (shader == null)
                {
                    Debug.LogError("Shader not found.");
                    return;
                }
                this.pixelationMaterial = CoreUtils.CreateEngineMaterial(shader);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                Debug.Log("Pixelation Pass Running (RenderGraph)");

                if (this.pixelationMaterial == null)
                {
                    Debug.LogError("Material not created.");
                    return;
                }

                var cameraData = frameData.Get<UniversalCameraData>();
                var resourceData = frameData.Get<UniversalResourceData>();

                if (!cameraData.postProcessEnabled)
                    return;

                var stack = VolumeManager.instance.stack;
                this.pixelation = stack.GetComponent<Pixelation>();

                if (this.pixelation == null)
                    return;

                if (!this.pixelation.IsActive())
                    return;

                // Source (camera color)
                var source = resourceData.activeColorTexture;

                // Create temp texture
                TextureDesc textureDesc = resourceData.activeColorTexture.GetDescriptor(renderGraph);
                textureDesc.name = "_TempPixelationTexture"; // Give it a descriptive name for debugging
                textureDesc.clearBuffer = true; // Optionally clear the buffer

                TextureHandle tempTexture = renderGraph.CreateTexture(textureDesc);

                using (var builder = renderGraph.AddRasterRenderPass<PassData>(k_RenderTag, out var passData))
                {
                    passData.source = source;
                    passData.destination = tempTexture;
                    passData.material = this.pixelationMaterial;

                    passData.width = this.pixelation.widthPixelation.value;
                    passData.height = this.pixelation.heightPixelation.value;
                    passData.colorPrecision = this.pixelation.colorPrecision.value;

                    // Tell RG what we read/write
                    builder.UseTexture(passData.source);
                    builder.SetRenderAttachment(passData.destination, 0);
                    //builder.SetRenderAttachment(passData.destination, TempTargetId);

                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        var cmd = context.cmd;

                        Debug.Log($"Pixelation width - {data.width}, height - {data.height}.");

                        // Set material properties
                        data.material.SetFloat(WidthPixelation, data.width);
                        data.material.SetFloat(HeightPixelation, data.height);
                        data.material.SetFloat(ColorPrecison, data.colorPrecision);

                        cmd.SetGlobalTexture(MainTexId, source);

                        // First blit: source → temp
                        Blitter.BlitTexture(cmd, data.source, Vector4.one, 0, false);

                        // Second blit: temp → source (with effect)
                        Blitter.BlitTexture(cmd, data.destination, Vector4.one, data.material, 0);
                    });
                }

                cameraData.camera.depthTextureMode = cameraData.camera.depthTextureMode | DepthTextureMode.Depth;

                var blitParams = new RenderGraphUtils.BlitMaterialParameters(tempTexture, source, pixelationMaterial, 0);

                // Add the blit pass
                renderGraph.AddBlitPass(blitParams, "Final Blit Pixelation");

                Debug.Log("Pixelation Pass End");
            }
        }

        PixelationPass pixelationPass;
        public RenderPassEvent injuctionPoint;

        public override void Create()
        {
            pixelationPass = new PixelationPass(injuctionPoint);
        }

        //ScripstableRendererFeature is an abstract class, you need this method
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //pixelationPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(pixelationPass);
        }
    }

    class PassData
    {
        public TextureHandle source;
        public TextureHandle destination;
        public Material material;

        public float width;
        public float height;
        public float colorPrecision;
    }*/
}