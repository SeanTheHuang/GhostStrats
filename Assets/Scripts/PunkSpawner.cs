using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnLogic
{
    public int m_numTurnsUntilSpawn;
    public int m_numPunksToSpawn;
}

public class PunkSpawner : MonoBehaviour {

    public SpawnLogic[] m_spawns;
    public PunkSpawnPosition[] m_spawnPoints;
    public Transform m_punkPrefab;

    public bool m_running
    { get; private set; }

    int m_turnCounter = 0;
    int m_spawnLogicIndex = 0;

    public void PlayTurn()
    {
        if (m_spawnLogicIndex >= m_spawns.Length) // No more punks to spawn!!
            return;

        //m_turnCounter++;

        //if (m_turnCounter >= m_spawns[m_spawnLogicIndex].m_numTurnsUntilSpawn)
        //{
        //    m_running = true;
        //    StartCoroutine(SpawnAnimation());
        //}

        if (!GameMaster.Instance().ThereArePunksStillAlive())
        {
            m_running = true;
            StartCoroutine(SpawnAnimation());
        }
    }

    IEnumerator SpawnAnimation()
    {
        // NOTE: IF THERE ARE MORE PUNKS SPAWNING THAN SPAWN POINTS, THIS WILL FREEZE FOREVER!!!
        // FOREVERRRRRRRRRRRRRR
        List<int> takenSpots = new List<int>();

        for (int i = 0; i < m_spawns[m_spawnLogicIndex].m_numPunksToSpawn; i++)
        {
            int randSpawnPoint = Random.Range(0, m_spawnPoints.Length);
            while (takenSpots.Contains(randSpawnPoint))
                randSpawnPoint = Random.Range(0, m_spawnPoints.Length);

            takenSpots.Add(randSpawnPoint);
            Quaternion rotation = Quaternion.AngleAxis((float)m_spawnPoints[randSpawnPoint].m_startDirection, Vector3.up);
            Transform temp = Instantiate(m_punkPrefab, m_spawnPoints[randSpawnPoint].transform.position, rotation);
            CameraControl.Instance.SetFollowMode(temp); // Look at dat new punk
            GameMaster.Instance().NewPunk(temp);
            SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.PUNK_SPAWN);
            yield return new WaitForSeconds(0.5f);
        }

        takenSpots.Clear();
        m_spawnLogicIndex++; // Next spawn wave
        m_turnCounter = 0;
        m_running = false;
        yield return null;
    }

    public bool SpawnerFinished()
    { return m_spawnLogicIndex >= m_spawns.Length; }
}
