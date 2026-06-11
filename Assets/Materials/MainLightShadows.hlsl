void MainLightShadows_float(float3 WorldPos, out float3 LightDir, out float3 LightColor, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        LightDir = normalize(float3(0.5, 0.5, 0.5));
        LightColor = float3(1, 1, 1);
        ShadowAtten = 1.0;
    #else
        // Calculate shadow coordinate from world position
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        
        // Get the primary directional light data
        Light mainLight = GetMainLight(shadowCoord);
        
        LightDir = mainLight.direction;
        LightColor = mainLight.color;
        ShadowAtten = mainLight.shadowAttenuation;
    #endif
}
