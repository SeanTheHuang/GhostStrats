using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundCatagory
{
    GHOST_ATTACK,
    GHOST_HIDE,
    GHOST_OVERSPOOK,
    GHOST_MONSTER,
    GHOST_DROPDOLL,
    GHOST_ACTIVATE_DOLL,
    GHOST_WALL,
    GHOST_HURT,
    PUNK_ATTACK,
    PUNK_HURT,
    PUNK_DEAD,
    RELIC_HURT,
    GHOST_SPAWN,
    PUNK_SPAWN
}

[System.Serializable]
public struct SoundClip
{
    public AudioClip m_audioClip;
    public string m_name;
}

public class SoundEffectsPlayer : MonoBehaviour {

    public SoundClip[] m_initalClips;
    Dictionary<string, AudioClip> m_soundDict;

    public static SoundEffectsPlayer Instance
    { get; private set; }

    AudioSource m_source;
    public SoundLibrary[] m_sounds;

    private void Awake()
    {
        // Shitty singleton baby
        Instance = this;
        m_source = GetComponent<AudioSource>();

        // Initialize soundDict
        m_soundDict = new Dictionary<string, AudioClip>();
        foreach (SoundClip sc in m_initalClips)
        {
            m_soundDict.Add(sc.m_name, sc.m_audioClip);
        }
    }

    public void PlaySound(SoundCatagory _catagory)
    {
        foreach (SoundLibrary sl in m_sounds)
        {
            if (sl.m_catagory == _catagory)
            {
                sl.PlaySound(m_source);
                break;
            }
        }
    }

    public void PlaySound(string _name)
    {
        m_source.clip = m_soundDict[_name];
        m_source.volume = 1;
        m_source.pitch = 1;
        m_source.Play();
    }
}
