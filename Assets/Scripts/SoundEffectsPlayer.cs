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
    RELIC_HURT
}

public class SoundEffectsPlayer : MonoBehaviour {

    public AudioClip m_selectSound;
    public AudioClip m_deselectSound;

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

    public void SelectSound()
    {
        m_source.clip = m_selectSound;
        m_source.volume = 1;
        m_source.pitch = 1;
        m_source.Play();
    }

    public void DeselectSound()
    {
        m_source.clip = m_deselectSound;
        m_source.volume = 1;
        m_source.pitch = 1;
        m_source.Play();
    }
}
