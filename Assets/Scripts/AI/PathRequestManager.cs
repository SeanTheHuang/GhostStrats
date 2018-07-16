using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    public static PathRequestManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
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
}
