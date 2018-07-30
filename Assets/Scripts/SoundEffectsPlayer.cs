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
    GHOST_WALL,
    GHOST_HURT,
    PUNK_ATTACK,
    PUNK_HURT,
    PUNK_DEAD
}

public class SoundEffectsPlayer : MonoBehaviour {

    public static SoundEffectsPlayer Instance
    { get; private set; }

    AudioSource m_source;
    public SoundLibrary[] m_sounds;

    private void Awake()
    {
        // Shitty singleton baby
        Instance = this;
        m_source = GetComponent<AudioSource>();
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
}
