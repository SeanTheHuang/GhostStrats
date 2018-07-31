using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsSpawner : MonoBehaviour {

    public Transform m_poofParticlesPrefab;
    public Transform m_respawnParticlesPrefab;
    public Transform m_hideParticlesPrefab;
    public void SpawnPoofPrefab(Vector3 _location)
    {
        Instantiate(m_poofParticlesPrefab, _location, m_poofParticlesPrefab.rotation);
    }

    public void SpawnRespawnPoofPrefab(Vector3 _location)
    {
        Instantiate(m_respawnParticlesPrefab, _location, m_respawnParticlesPrefab.rotation);
    }

    public void SpawnHidePoofPrefab(Vector3 _location)
    {
        Instantiate(m_hideParticlesPrefab, _location, m_respawnParticlesPrefab.rotation);
    }
}
