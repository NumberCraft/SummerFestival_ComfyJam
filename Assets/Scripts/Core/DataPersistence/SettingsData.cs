using System.Collections.Generic;
using System;
using UnityEngine;

public class SettingsData
{
    public long lastUpdated;

    public float masterVolume;
    public float musicVolume;
    public float soundVolume;

    public int shadowQuality;
    public int screenMode;
    public int resolution;

    public float brightness;

    public bool invertYAxis;
    public bool invertXAxis;

    public SettingsData()
    {
        this.masterVolume = 0f;
        this.musicVolume = 0f;
        this.soundVolume = 0f;

        this.shadowQuality = 2;
        this.screenMode = 0;
        this.resolution = 0;

        this.brightness = 0f;

        this.invertYAxis = false;
        this.invertXAxis = false;
    }

    public SettingsData(float masterVolume, float musicVolume, float soundVolume, int shadowQuality, int screenMode, int resolution, float brightness,
        bool invertYAxis, bool invertXAxis)
    {
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.soundVolume = soundVolume;

        this.shadowQuality = shadowQuality;
        this.screenMode = screenMode;
        this.resolution = resolution;

        this.brightness = brightness;

        this.invertYAxis = invertYAxis;
        this.invertXAxis = invertXAxis;
    }
}
