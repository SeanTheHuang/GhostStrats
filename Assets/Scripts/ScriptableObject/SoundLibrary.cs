using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject {


    public float m_pitchOffset = 0.1f;
    public AudioClip[] m_audioClips;

    public void PlaySound(AudioSource _source)
    {
        if (m_audioClips.Length < 1)
        {
            Debug.LogWarning("Warning: There are no sound clips to play from this Sound Library");
            return;
        }
        
        _source.pitch = Random.Range(1 - m_pitchOffset, 1 + m_pitchOffset);

        int index = Random.Range(0, m_audioClips.Length);
        _source.PlayOneShot(m_audioClips[index]);
    }


}
