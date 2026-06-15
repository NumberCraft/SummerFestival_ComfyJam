using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

public class PassData
{
    public Material material;
    public TextureHandle inputTexture;
    public int patternIndex;
    public float threshold, strength, scale, density, noiseScale, noiseStrength, distance;
    public Color color;
}
