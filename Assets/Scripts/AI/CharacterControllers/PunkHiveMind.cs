using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunkHiveMind : MonoBehaviour
{
    public class CNoise
    {
        public Transform m_center;
        public int m_CoolDown;
        
    }

    // Use this for initialization
    public List<Transform> m_HouseLocations;
    public List<Transform> m_PunkLocations;

    [HideInInspector]
    public List<Transform> m_KnownGhostHoles;
    [HideInInspector]
    public List<CNoise> m_Noises;


    private void Awake()
    {
        m_KnownGhostHoles = new List<Transform> { };
        m_Noises = new List<CNoise> { };
    }

    public void OnStartTurn()
    {
        for (int i = 0; i < m_Noises.Count; i++)
        {
            m_Noises[i].m_CoolDown -= 1;

            if (m_Noises[i].m_CoolDown == 0) 
            {
                m_Noises.RemoveAt(i);
                i--;
            }
        }
    }

    public void AddNoise(Transform tr)
    {
        CNoise n = new CNoise
        {
            m_center = tr,
            m_CoolDown = Random.Range(2, 4)
        };
    }
}
