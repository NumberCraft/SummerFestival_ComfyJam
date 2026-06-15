Shader "PostEffect/Pixelation"
{
    Properties
    {
        // This MUST be named _BlitTexture for Unity 6
        [HideInInspector] _BlitTexture("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _WidthPixelation;
            float _HeightPixelation;
            float _ColorPrecision;

            half4 Frag (Varyings i) : SV_Target
            {
                // Unity 6 UVs can sometimes be flipped. 
                // Using i.texcoord is the standard for Blitter.
                float2 uv = i.texcoord;

                // 1. Pixelation Math
                // Ensure we don't have 0/0 errors which cause black screens
                float w = (_WidthPixelation > 0) ? _WidthPixelation : 1;
                float h = (_HeightPixelation > 0) ? _HeightPixelation : 1;
                
                uv.x = floor(uv.x * w) / w;
                uv.y = floor(uv.y * h) / h;

                // 2. Sampling the screen
                // We use SAMPLE_TEXTURE2D_X which is the most robust for URP 
                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // 3. Color Precision (The PS1 "Crunch")
                float precision = (_ColorPrecision > 0) ? _ColorPrecision : 1;
                color.rgb = floor(color.rgb * precision) / precision;

                // If the screen is still black, let's force Alpha to 1 
                // to make sure it's not a transparency issue
                color.a = 1.0;

                return color;
            }
            ENDHLSL
        }
    }
}
