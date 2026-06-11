using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name = "Sound";

    public AudioClip clip;

    [Space(20)]
    [Range(0f, 3f)] public float volume = 1f;
    public float minVolume = 0.5f;
    public float maxVolume = 1.5f;

    [Space(20)]
    [Range(0.1f, 3f)] public float pitch = 1f;
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    [Space(20)]
    [Range(0f, 1f)] public float spatialBlend = 0f;

    [Space(20)]
    [Range(0f, 360f)] public float spread;
    public float minDistance = 1f;
    public float maxDistance = 200f;

    public bool loop = false;
    public bool playOnAwake = false;
    public bool Global = false;

    public AudioMixerGroup audioMixerGroup;

    [HideInInspector]
    public AudioSource source;
}
