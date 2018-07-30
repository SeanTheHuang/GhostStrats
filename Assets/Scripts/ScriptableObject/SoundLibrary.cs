using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject {

    public SoundCatagory m_catagory;
    [MinMaxRange(0, 3)]
    public RangedFloat m_pitch;
    [MinMaxRange(0, 1)]
    public float m_volume = 1;
    public AudioClip[] m_audioClips;

    public void PlaySound(AudioSource _source)
    {
        if (m_audioClips.Length < 1)
        {
            Debug.LogWarning("Warning: There are no sound clips to play from this Sound Library");
            return;
        }
        
        _source.pitch = Random.Range(m_pitch.minValue, m_pitch.maxValue);
        _source.volume = m_volume;
        int index = Random.Range(0, m_audioClips.Length);
        _source.PlayOneShot(m_audioClips[index]);
    }


}
