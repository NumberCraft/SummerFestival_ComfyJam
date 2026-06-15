// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PostEffect/Fog"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            // Re-include your noise library here
            // #include "Assets/Materials/RetroEffect/PS1/V3/Shaders/cginc/voronoi.cginc"
            // Note: If cnoise is missing, ensure the path above is correct or define a simple noise here.

            float _FogDensity;
            float _FogDistance;
            float4 _FogColor;
            float _NoiseScale;
            float _NoiseStrength;

            // Simple noise fallback if your include fails
            float hash(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123); }
            float noise(float2 p) {
                float2 i = floor(p); float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(hash(i + float2(0,0)), hash(i + float2(1,0)), u.x),
                            lerp(hash(i + float2(0,1)), hash(i + float2(1,1)), u.x), u.y);
            }

            half4 Frag (Varyings i) : SV_Target
            {
                float2 uv = i.texcoord;
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                float depth = SampleSceneDepth(uv);
                float eyeDepth = LinearEyeDepth(depth, _ZBufferParams);

                // --- FOG CALCULATION ---
                // 1. Calculate base fog based on distance. 
                // If eyeDepth is 50 and _FogDistance is 100, fogFactor is 0.5.
                // Prevent division by zero if _FogDistance is 0
                float fogFactor = saturate(eyeDepth / max(_FogDistance, 0.0001));

                // 2. Apply Density. 
                // If Density is 1.0, it stays at 0.5. If Density is 2.0, it becomes 1.0 (Full Fog).
                float finalFog = saturate(fogFactor * _FogDensity);

                // 3. Add Noise
                float screenNoise = noise(uv * _NoiseScale) * _NoiseStrength;
                finalFog = saturate(finalFog + screenNoise);

                // --- PREVENT SKYBOX FOG ---
                #if UNITY_REVERSED_Z
                    if (depth <= 0.00001) return color;
                #else
                    if (depth >= 0.99999) return color;
                #endif

                // --- FINAL COLOR ---
                // Remove the * 0.1 so you can actually see the color you picked in the inspector!
                float3 fogColor = _FogColor.rgb; 

                return float4(lerp(color.rgb, fogColor, finalFog), color.a);
            }
            ENDHLSL
        }
    }
}
