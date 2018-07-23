using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour {

    public static SoundEffectsPlayer Instance
    { get; private set; }

    AudioSource m_source;
    public SoundLibrary m_testGhostSounds;

    private void Awake()
    {
        // Shitty singleton baby
        Instance = this;
        m_source = GetComponent<AudioSource>();
    }

    public void PlayTestGhostSound()
    {
        if (m_testGhostSounds)
            m_testGhostSounds.PlaySound(m_source);
    }
}
