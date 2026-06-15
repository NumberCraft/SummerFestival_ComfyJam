using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class Pixelation : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, overrideState: true);

        //PIXELATION
        public FloatParameter widthPixelation = new FloatParameter(512);
        public FloatParameter heightPixelation = new FloatParameter(512);
        
        //COLOR PRECISION 
        public FloatParameter colorPrecision = new FloatParameter(32.0f);
        
        //INTERFACE REQUIREMENT 
        public bool IsActive() => intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}