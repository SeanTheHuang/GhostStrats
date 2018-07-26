using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsSpawner : MonoBehaviour {

    public Transform m_poofParticlesPrefab;

    public void SpawnPoofPrefab(Vector3 _location)
    {
        Instantiate(m_poofParticlesPrefab, _location, m_poofParticlesPrefab.rotation);
    }
}
