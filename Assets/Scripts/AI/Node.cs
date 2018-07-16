using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public int clearance;

    public Node parent;

    
    public bool Walkable;
    public Vector3 WorldPosition;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        Walkable = _walkable;
        WorldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
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
