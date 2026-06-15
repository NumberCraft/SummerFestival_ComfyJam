Shader "PostEffect/Dithering"
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

            uint _PatternIndex;
            float _DitherThreshold;
            float _DitherStrength; // You can multiply this by your final dither for intensity
            float _DitherScale;

            // Dither pattern function (Keep your logic, just clean up types)
            float4x4 GetDitherPattern(uint index)
            {
                if(index == 0) return float4x4(0,1,0,1, 1,0,1,0, 0,1,0,1, 1,0,1,0);
                if(index == 1) return float4x4(0.23,0.2,0.6,0.2, 0.2,0.43,0.2,0.77, 0.88,0.2,0.87,0.2, 0.2,0.46,0.2,0);
                if(index == 2) return float4x4(-4,0,-3,1, 2,-2,3,-1, -3,1,-4,0, 3,-1,2,-2);
                if(index == 3) return float4x4(1,0,0,1, 0,1,1,0, 0,1,1,0, 1,0,0,1);
                return 1;
            }

            half4 Frag (Varyings i) : SV_Target
            {
                // In Unity 6 Blitter, i.texcoord is the standard UV
                float2 uv = i.texcoord;
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // Calculate screen-space coordinates for the dither grid
                // We use _ScreenParams (current render target size)
                float2 screenPos = uv * _ScreenParams.xy;
                uint2 ditherCoord = (uint2)(screenPos / max(0.1, _DitherScale)) % 4;

                // Brightness calc
                float brightness = (color.r + color.g + color.b) / 3.0;
                
                float4x4 pattern = GetDitherPattern(_PatternIndex);
                float threshold = pattern[ditherCoord.x][ditherCoord.y];

                // Dither logic
                float ditherResult = ((brightness * _DitherThreshold) < threshold) ? 0.0 : 1.0;

                // Mix based on strength (1.0 = full dither, 0.0 = no dither)
                float3 finalRGB = lerp(color.rgb, color.rgb * ditherResult, _DitherStrength);

                return float4(finalRGB, color.a);
            }
            ENDHLSL
        }
    }
}
