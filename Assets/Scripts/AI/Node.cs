using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum NodeState
{
    EMPTY,
    GHOST_TRAP,
    PUNK_TRAP,
    OBSTACLE,
    GHOST_HIDE,
    GHOST_WALL
}

public class Node : IHeapItem<Node>
{
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public int clearance;

    public Node parent;

    public NodeState m_nodeState;
    public Transform m_entityOnTile;

    public bool WalkabilityIsLocked // This position is always blocked off
    { get; private set; }

    public bool Walkable;
    public Vector3 WorldPosition;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        Walkable = _walkable;
        WorldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        m_nodeState = NodeState.EMPTY;
        m_entityOnTile = null;
        WalkabilityIsLocked = !_walkable;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }

    }

    public int HeapIndex
    {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
