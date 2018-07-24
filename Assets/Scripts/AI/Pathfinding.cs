using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    PathRequestManager requestManager;
    NodeGrid grid;
    bool m_findingPath;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public Vector3 RequestRandomLocation(int _clearance)
    {
        return grid.RequestRandomLocation(_clearance);
    }

    public Vector3 RequestCloseLocation(int _clearance, Vector3 _target)
    {
        return grid.RequestCloseLocation(_clearance, _target);
    }

    public Vector3 RequestCirclingLocation(int _clearance, Vector3 _position, Vector3 _rightDirection)
    {
        return grid.RequestCirclingLocation(_clearance, _position, _rightDirection);
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos, int clearance)
    {
        StartCoroutine(FindPath(startPos, targetPos, clearance));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 endPos, int clearance)
    {
        m_findingPath = true;

        // Wait for grid to be ready
        while (!grid.GridReady())
            yield return null;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        startPos = transform.InverseTransformPoint(startPos);
        endPos = transform.InverseTransformPoint(endPos);

        //Debug.Log("Start Position: " + startPos);
        //Debug.Log("End Position  : " + endPos);

        Vector3[] wayPoints = new Vector3[0];
        bool pathFound = false;

        //Convert co-ords to nodes
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(endPos);

        // NOTE, this means path can start from non-walkable state
        Debug.Log("GOT HERE");
        if (targetNode.Walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)  //Path found!
                {
                    pathFound = true;
                    break;
                }

                foreach (Node n in grid.GetNeighbours(currentNode))
                {
                    if (!n.Walkable || closedSet.Contains(n) || (n.clearance < clearance && n != targetNode))
                    {
                        continue;
                    }
                    
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, n);

                    if (newMovementCostToNeighbour < n.gCost || !openSet.Contains(n))
                    {
                        n.gCost = newMovementCostToNeighbour;
                        n.hCost = GetDistance(n, targetNode);
                        n.parent = currentNode;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }
                        else
                        {
                            openSet.UpdateItem(n);
                        }
                    }
                }
            }

        }

        if (pathFound)
            wayPoints = RetracePath(startNode, targetNode, clearance, endPos);

        requestManager.FinishedProcessingPath(wayPoints, pathFound);
        m_findingPath = false;

        sw.Stop();
        //Debug.Log("Time to find path: " + sw.ElapsedMilliseconds + "ms.");
        yield return null;
    }

    Vector3[] RetracePath(Node startNode, Node endNode, int _agentClearance, Vector3 _endPos)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] wayPoints = SimplifyPath(path, _agentClearance);
        Array.Reverse(wayPoints);

        return wayPoints;
    }

    Vector3[] SimplifyPath(List<Node> path, int _agentClearance)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        //Offset due to clearance requirements of agent
        float offset = grid.m_nodeRadius * (float)(_agentClearance - 1);
        for (int i = 0; i < path.Count; i++)
        {
            //Vector2 directionNew = new Vector2(path[i].gridX - path[i - 1].gridX, path[i].gridY - path[i - 1].gridY);

            //if (directionNew != directionOld //Going new direction now
            //    || clearanceOld != path[i-1].clearance
            //    || i == path.Count - 1)       // or last node in list
            //{
            //    // Add way points
            //    Vector3 newPoint = path[i - 1].clearance > 1 ? path[i - 1].WorldPosition + new Vector3(offset, 0, offset) : path[i - 1].WorldPosition;
            //    wayPoints.Add(newPoint);
            //    directionOld = directionNew;
            //    clearanceOld = path[i - 1].clearance;
            //}

            Vector3 newPoint = path[i].clearance > 1 ? path[i].WorldPosition + new Vector3(offset, 0, offset) : path[i].WorldPosition;
            wayPoints.Add(newPoint);
        }

        return wayPoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

    public bool FindingPath() { return m_findingPath; }

    public Vector3[] FindPathImmediate(Vector3 startPos, Vector3 targetPos, int clearance)
    {
        Vector3[] wayPoints = new Vector3[0];

        // Wait for grid to be ready
        while (!grid.GridReady())
            return wayPoints;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        startPos = transform.InverseTransformPoint(startPos);
        targetPos = transform.InverseTransformPoint(targetPos);

        //Debug.Log("Start Position: " + startPos);
        //Debug.Log("End Position  : " + endPos);

        bool pathFound = false;

        //Convert co-ords to nodes
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (targetNode.Walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)  //Path found!
                {
                    pathFound = true;
                    break;
                }

                foreach (Node n in grid.GetNeighbours(currentNode))
                {
                    if (!n.Walkable || closedSet.Contains(n) || (n.clearance < clearance && n != targetNode))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, n);

                    if (newMovementCostToNeighbour < n.gCost || !openSet.Contains(n))
                    {
                        n.gCost = newMovementCostToNeighbour;
                        n.hCost = GetDistance(n, targetNode);
                        n.parent = currentNode;

                        if (!openSet.Contains(n))
                        {
                            openSet.Add(n);
                        }
                        else
                        {
                            openSet.UpdateItem(n);
                        }
                    }
                }
            }

        }

        if (pathFound)
            wayPoints = RetracePath(startNode, targetNode, clearance, targetPos);

        sw.Stop();
        return wayPoints;
    }
}
