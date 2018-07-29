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


    public bool m_displayGizmos = false;

    // Use this for initialization
    public List<Room> m_RoomList;
    public List<Transform> m_PunkLocations;
    public int m_TrapDamage = 4;

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
        m_Noises.Add(n);
    }

    public void RemovePunk(Transform tr)
    {
        m_PunkLocations.Remove(tr);
    }

    public Room ChooseRoom(Room _r)
    {
        int count = 0;
        while (true)
        {
            int t = Random.Range(0, _r.m_connectedRooms.Count);
            if(_r.m_connectedRooms[t].m_targeted == false)
            {
                return _r.m_connectedRooms[t];
            }

            if(count > 30)
            {//safety as some room only connect to two others.
                return ChooseFirstRoom();
            }
        }
    }

    public Room ChooseFirstRoom()
    {
        while (true)
        {
            int temp = Random.Range(0, m_RoomList.Count);
            if (m_RoomList[temp].m_targeted == false)
            {
                return m_RoomList[temp];
            }

            bool check = false;
            foreach(Room rr in m_RoomList)
            {
                if(rr.m_targeted == false)
                {
                    check = true;
                    break;
                }
            }

            if(check == false)
            {
                return m_RoomList[Random.Range(0, m_RoomList.Count)];
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(m_displayGizmos == false)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        for(int i = 0; i< m_RoomList.Count; i++)
        {
            Gizmos.color = Color.black;
            Room r = m_RoomList[i];
            for(int j=0; j < r.m_connectedRooms.Count; j++)
            {
                Gizmos.DrawLine(r.transform.position, r.m_connectedRooms[j].transform.position);
            }
        }
    }
}
