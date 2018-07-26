using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is also responsible for dealing with
// all requests involving the grid

public class PathRequestManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;
    NodeGrid m_nodeGrid;

    bool isProcessingPath;

    public static PathRequestManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
        m_nodeGrid = GetComponent<NodeGrid>();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public int clearance;
        public Action<Vector3[], bool> callBack;

        public PathRequest(Vector3 _start, Vector3 _end, int _clearance, Action<Vector3[], bool> _callBack)
        {
            pathStart = _start;
            pathEnd = _end;
            clearance = _clearance;
            callBack = _callBack;
        }
    }

    public static void RequestPath(Vector3 worldStartPosition, Vector3 worldEndPosition, int clearance, Action<Vector3[], bool> callBack)
    {
        PathRequest newRequest = new PathRequest(worldStartPosition, worldEndPosition, clearance, callBack);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.clearance);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callBack(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    // Returns the width/height of 1 tile
    public float GridSize()
    {
        return m_nodeGrid.m_nodeRadius;
    }

    public Node NodeFromWorldPoint(Vector3 _position)
    {
        return m_nodeGrid.NodeFromWorldPoint(_position);
    }

    public bool SetNodeState(NodeState _newState, Transform _entityOnTile)
    {
        Node currentNode = m_nodeGrid.NodeFromWorldPoint(_entityOnTile.position);

        if (currentNode == null)
            return false;

        currentNode.m_nodeState = _newState;
        currentNode.m_entityOnTile = _entityOnTile;
        return true;
    }

    public bool PointIsInGrid(Vector3 _worldPoint)
    {
        return (m_nodeGrid.NodeFromWorldPoint(_worldPoint) != null);
    }

    public NodeState GetNodeState(Vector3 _worldPosition)
    {
        return m_nodeGrid.NodeFromWorldPoint(_worldPosition).m_nodeState;
    }

    public Vector3 GridPointFromWorldPos(Vector3 _position)
    {
        Vector2 point2D = m_nodeGrid.WorldPosToGrid(_position);
        return new Vector3(point2D.x, _position.y, point2D.y);
    }

    public Vector3[] GetPathImmediate(Vector3 _startPos, Vector3 _endPos, int _clearance)
    {
        return pathfinding.FindPathImmediate(_startPos, _endPos, _clearance);
    }

    public bool PositionIsWalkable(Vector3 _position)
    {
        Node node = m_nodeGrid.NodeFromWorldPoint(_position);

        if (node == null)
            return false;

        return (node.Walkable);
    }

    public void TogglePositionWalkable(Vector3 _position, bool _newState)
    {
        Node node = m_nodeGrid.NodeFromWorldPoint(_position);

        if (node == null)
            return;

        //Debug.Log("GOT HERE");

        if (!node.WalkabilityIsLocked)
        {
            node.Walkable = _newState;
        }
    }

    //public List<T> GetObjectsFromListOfPositions<T>(List<Vector3> _worldPositionList, NodeState _type)
    //{
    //    List<T> m_outputList = new List<T>();

    //    foreach (Vector2 v3 in _worldPositionList) {
    //        Node nodeToCheck = m_nodeGrid.NodeFromWorldPoint(v3);

    //        if (nodeToCheck != null) {
    //            if (nodeToCheck.m_nodeState == _type) {
    //                // NOTE: Assuming entity has a controller we are looking for
    //                m_outputList.Add(nodeToCheck.m_entityOnTile.GetComponent<T>());
    //            }
    //        }
    //    }
    //    return m_outputList;
    //}
}
