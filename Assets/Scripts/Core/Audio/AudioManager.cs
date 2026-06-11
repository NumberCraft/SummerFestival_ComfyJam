using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public SoundPack[] soundPacks;

    private List<AudioSource> sfxSources = new List<AudioSource>();

    public static AudioManager i;

    private void Awake()
    {
        if (i == null)
            i = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            if (s.Global)
            {
                //GameObject audioGameObject = new GameObject(s.name);
                //s.source = audioGameObject.AddComponent<AudioSource>();
                s.source = gameObject.AddComponent<AudioSource>();

                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;

                s.source.spatialBlend = s.spatialBlend;

                s.source.spread = s.spread;
                s.source.minDistance = s.minDistance;
                s.source.maxDistance = s.maxDistance;

                if (s.playOnAwake)
                {
                    s.source.playOnAwake = s.playOnAwake;
                    s.source.Play();
                }
                else
                    s.source.Stop();

                s.source.loop = s.loop;

                s.source.outputAudioMixerGroup = s.audioMixerGroup;
            }
        }
    }

    #region Sound

    public static void Play(string name)
    {
        Sound s = Array.Find(i.sounds, sound => sound.name == name);
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
    }

    public static void Stop(string name)
    {
        Sound s = Array.Find(i.sounds, sound => sound.name == name);
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }
    }

    public static void Play(string name, GameObject sourceGameObject)
    {
        AudioSource[] audioSources = sourceGameObject.GetComponents<AudioSource>();
        Sound s = Array.Find(i.sounds, sound => sound.name == name);

        if (!Array.Find(audioSources, source => source.clip == s.clip))
        {
            AudioSource source = sourceGameObject.AddComponent<AudioSource>();

            source.playOnAwake = s.playOnAwake;

            source.clip = s.clip;

            source.volume = s.volume;
            source.pitch = s.pitch;

            source.spatialBlend = s.spatialBlend;

            source.spread = s.spread;
            source.minDistance = s.minDistance;
            source.maxDistance = s.maxDistance;

            source.loop = s.loop;

            source.outputAudioMixerGroup = s.audioMixerGroup;

            if (!source.isPlaying)
                source.Play();
        }
        else
        {
            AudioSource source = Array.Find(audioSources, source => source.clip == s.clip);

            if (!source.isPlaying)
                source.Play();
        }
    }

    public static void Play(string name, AudioSource audioSource)
    {
        Sound s = Array.Find(i.sounds, sound => sound.name == name);

        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public static void Play(string name, int index, float pitch = 0f)
    {
        Sound s = Array.Find(i.sounds, sound => sound.name == name);

        AudioSource audioSource = s.source;

        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        if (pitch != 0f)
            audioSource.pitch = pitch;
        else
            audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public static void Play(string name, float pitch = 0f)
    {
        Sound s = Array.Find(i.sounds, sound => sound.name == name);

        s.source.playOnAwake = s.playOnAwake;

        s.source.clip = s.clip;

        s.source.volume = s.volume;
        if (pitch != 0f)
            s.source.pitch = pitch;
        else
            s.source.pitch = s.pitch;

        s.source.spread = s.spread;
        s.source.minDistance = s.minDistance;
        s.source.maxDistance = s.maxDistance;

        s.source.loop = s.loop;

        s.source.outputAudioMixerGroup = s.audioMixerGroup;

        s.source.spatialBlend = s.spatialBlend;

        if (!s.source.isPlaying)
            s.source.Play();
    }

    public static void Stop(string name, GameObject sourceGameObject)
    {
        AudioSource[] audioSources = sourceGameObject.GetComponents<AudioSource>();
        Sound s = Array.Find(i.sounds, sound => sound.name == name);

        if (Array.Find(audioSources, source => source.clip == s.clip))
        {
            AudioSource source = Array.Find(audioSources, source => source.clip == s.clip);

            if (source.isPlaying)
                source.Stop();
        }
    }

    public static void Stop(string name, Transform sourceTransform)
    {
        AudioSource[] audioSources = sourceTransform.gameObject.GetComponents<AudioSource>();
        Sound s = Array.Find(i.sounds, sound => sound.name == name);

        if (Array.Find(audioSources, source => source.clip == s.clip))
        {
            AudioSource source = Array.Find(audioSources, source => source.clip == s.clip);

            if (source.isPlaying)
                source.Stop();
        }
    }

    public static void Play(Sound s, AudioSource audioSource)
    {
        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    #endregion

    #region Sound Pack
    public static void PlayPack(string name, float pitch = 0f)
    {
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = null;

        int randomIndex = UnityEngine.Random.Range(0, soundPack.sounds.Length);
        s = soundPack.sounds[randomIndex];

        s.source.playOnAwake = s.playOnAwake;

        s.source.clip = s.clip;

        s.source.volume = s.volume;
        if (pitch != 0f)
            s.source.pitch = pitch;
        else
            s.source.pitch = s.pitch;

        s.source.spread = s.spread;
        s.source.minDistance = s.minDistance;
        s.source.maxDistance = s.maxDistance;

        s.source.loop = s.loop;

        s.source.outputAudioMixerGroup = s.audioMixerGroup;

        s.source.spatialBlend = s.spatialBlend;

        if (!s.source.isPlaying)
            s.source.Play();
    }

    public static void PlayPack(string name, GameObject sourceGameObject, float pitch = 0f)
    {
        AudioSource audioSource = sourceGameObject.GetComponent<AudioSource>();
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = null;

        int randomIndex = UnityEngine.Random.Range(0, soundPack.sounds.Length);
        s = soundPack.sounds[randomIndex];

        if (audioSource == null)
        {
            audioSource = sourceGameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        if (pitch != 0f)
            audioSource.pitch = pitch;
        else
            audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public static void PlayPack(string name, AudioSource audioSource, float pitch = 0f)
    {
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = null;

        int randomIndex = UnityEngine.Random.Range(0, soundPack.sounds.Length);
        s = soundPack.sounds[randomIndex];

        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        if (pitch != 0f)
            audioSource.pitch = pitch;
        else
            audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public static void PlayPack(string name, GameObject sourceGameObject, int index, float pitch = 0f)
    {
        AudioSource audioSource = sourceGameObject.GetComponent<AudioSource>();
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = null;

        s = soundPack.sounds[index];

        if (audioSource == null)
        {
            audioSource = sourceGameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        if (pitch != 0f)
            audioSource.pitch = pitch;
        else
            audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public static void PlayPack(string name, int index, float pitch = 0f)
    {
        AudioSource audioSource = i.gameObject.GetComponent<AudioSource>();
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = null;

        s = soundPack.sounds[index];

        if (audioSource == null)
        {
            audioSource = i.gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = s.playOnAwake;

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        if (pitch != 0f)
            audioSource.pitch = pitch;
        else
            audioSource.pitch = s.pitch;

        audioSource.spread = s.spread;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.loop = s.loop;

        audioSource.outputAudioMixerGroup = s.audioMixerGroup;

        audioSource.spatialBlend = s.spatialBlend;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public static void StopPack(string name)
    {
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = Array.Find(soundPack.sounds, sound => sound.name == name);
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }
    }

    public static void StopPack(string name, GameObject sourceGameObject)
    {
        AudioSource audioSource = sourceGameObject.GetComponent<AudioSource>();
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    #endregion

    #region Get
    public static Sound GetSound(string name, int index)
    {
        AudioSource[] audioSources = i.gameObject.GetComponents<AudioSource>();
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);
        Sound s = null;

        if (soundPack == null)
            return null;

        if (index >= soundPack.sounds.Length)
            return null;

        s = soundPack.sounds[index];

        if (s != null)
            return s;

        return null;
    }

    public static AudioSource GetAudioSource(string name, GameObject sourceGameObject)
    {
        AudioSource[] audioSources = sourceGameObject.GetComponents<AudioSource>();
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);

        foreach (var s in soundPack.sounds)
        {
            if (Array.Find(audioSources, source => source.clip == s.clip))
            {
                AudioSource source = Array.Find(audioSources, source => source.clip == s.clip);

                return source;
            }
        }

        return null;
    }

    public static Sound[] GetSoundArray(string name)
    {
        SoundPack soundPack = Array.Find(i.soundPacks, sound => sound.name == name);

        if (soundPack != null)
            return soundPack.sounds;

        return null;
    }

    #endregion

    public void PlaySoundAtPosition(string soundName, Vector3 position)
    {
        Sound s = Array.Find(i.sounds, sound => sound.name == soundName);
        AudioSource source = GetAvailableSource();

        source.playOnAwake = s.playOnAwake;

        source.clip = s.clip;

        source.spatialBlend = s.spatialBlend;

        source.spread = s.spread;
        source.minDistance = s.minDistance;
        source.maxDistance = s.maxDistance;

        source.loop = s.loop;

        source.outputAudioMixerGroup = s.audioMixerGroup;

        source.transform.position = position;

        source.pitch = UnityEngine.Random.Range(s.minPitch, s.maxPitch);
        source.volume = UnityEngine.Random.Range(s.minVolume, s.maxVolume);

        source.Play();
    }

    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying)
                return source;
        }

        // none available → create new
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        sfxSources.Add(newSource);

        return newSource;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR

        if (sounds != null && sounds.Length > 0)
        {
            foreach (var sound in sounds)
            {
                if (sound.name == "" && sound.clip != null)
                {
                    sound.name = sound.clip.name;
                }
            }
        }

        if (soundPacks != null && soundPacks.Length > 0)
        {
            foreach (var soundpack in soundPacks)
            {
                foreach (var sound in soundpack.sounds)
                {
                    if (sound.name == "" && sound.clip != null)
                    {
                        sound.name = sound.clip.name;
                    }
                }
            }
        }

#endif
    }
}
