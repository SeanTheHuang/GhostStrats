using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Room> m_connectedRooms;
    [HideInInspector]
    public bool m_targeted;


}
