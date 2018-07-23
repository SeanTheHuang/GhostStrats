using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

using UnityEngine;

public class NodeGrid : MonoBehaviour {

    [SerializeField]
    private bool DisplayGridGizmos;

    [Range (5, 20)]
    public int m_closeWanderDistance = 10;

    [Range(1, 10)]
    public int m_circlingDistance = 4;

    public LayerMask m_unwalkableMask;
    public Vector2 m_gridWorldSize;
    public float m_nodeRadius;
    
    Node[,] m_mainGrid;

    Pathfinding m_pathfinding;
    float m_nodeDiameter;
    int m_gridSizeX, m_gridSizeY;
    bool m_gridReady;

    private void Awake()
    {
        m_pathfinding = GetComponent<Pathfinding>();
        m_nodeDiameter = m_nodeRadius * 2;
        m_gridSizeX = Mathf.RoundToInt(m_gridWorldSize.x / m_nodeDiameter);
        m_gridSizeY = Mathf.RoundToInt(m_gridWorldSize.y / m_nodeDiameter);
        StartCoroutine(InitializeGrid());
    }

    public int MaxSize
    {
        get {
            return m_gridSizeX * m_gridSizeY;  
        }
    }

    IEnumerator InitializeGrid()
    {
        // Wait for pathfinding to finish current path search
        while (m_pathfinding.FindingPath())
            yield return null;

        m_gridReady = false;
        CreateGrid();
        SetClearanceAll();
        m_gridReady = true;
        yield return null;
    }

    void CreateGrid()
    {
        // Creating nodes from [bottom left] => [top right]

        m_mainGrid = new Node[m_gridSizeX, m_gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right*m_gridWorldSize.x/2 - Vector3.forward * m_gridWorldSize.y/2;

        //Debug.Log("Bottom Left = (" + worldBottomLeft.x + "," + worldBottomLeft.z + ")");
        //Debug.Log("Top Right = (" + (worldBottomLeft.x + ((GridSizeX-1) * nodeDiameter + NodeRadius)) + "," + (worldBottomLeft.z + ((GridSizeY - 1) * nodeDiameter + NodeRadius)) + ")");

        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int x = 0; x < m_gridSizeX; x++)
        {
            for (int y = 0; y < m_gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius)
                                                     + Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);

                bool walkable = !Physics.CheckSphere(worldPoint, m_nodeRadius-0.001f, m_unwalkableMask);
                m_mainGrid[x, y] = new Node(walkable, worldPoint, x, y);
            }

        }
        sw.Stop();   
    }

    void SetClearanceAll()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int y = 0; y < m_gridSizeY; y++)
        {
            for (int x = 0; x < m_gridSizeX; x++)
            {
                SetClearance(m_mainGrid[x,y]);
            }
        }
        sw.Stop();
    }

    void SetClearance(Node _node)
    {
        //CURRENTLY INEFFICENTLY, checks middle nodes over and over each iteration
        //TODO: Make this less shit. unless people can't tell

        if (!_node.Walkable) //No point setting Clearance on unwalkable terrain
        {
            return;
        }

        bool stillRunning = true;
        int iteration = 0;

        while (stillRunning) 
        {
            iteration++;

            for (int i = 0; i < 1 + iteration; i++)
            {
                int xVal = _node.gridX + iteration;
                int yVal = _node.gridY + iteration;

                if (!LocationIsWalkable(xVal, _node.gridY) || !LocationIsWalkable(_node.gridX, yVal)
                    || !LocationIsWalkable(xVal, _node.gridY + iteration) || !LocationIsWalkable(_node.gridX + iteration, yVal))
                {
                    stillRunning = false;
                    break;
                }
            }

            //for (int y = _node.gridY-iteration; y < _node.gridY + iteration + 1; y++)
            //{
            //    for (int x = _node.gridX-iteration; x < _node.gridX + iteration + 1; x++)
            //    {
            //        //First, check index within range
            //        if (x < 0 || x >= m_gridSizeX)
            //        {
            //            stillRunning = false;
            //            break;
            //        }

            //        if (y < 0 || y >= m_gridSizeY)
            //        {
            //            stillRunning = false;
            //            break;
            //        }

            //        if (m_mainGrid[x, y] == _node) //Skip if same node
            //            continue;

            //        if (!m_mainGrid[x, y].Walkable)
            //        {
            //            stillRunning = false;
            //            break;
            //        }

            //        //Reach here: valid location
            //    }
            //}
        }
        _node.clearance = iteration;
    }

    bool LocationIsWalkable(int _x, int _y)
    {
        // Check if location is outside of grid
        if (_x < 0 || _y < 0
            || _x >= m_gridSizeX || _y >= m_gridSizeY)
            return false;

        // Node exists
        return m_mainGrid[_x, _y].Walkable;
    }

    public Vector3 RequestCloseLocation(int _clearance, Vector3 _targetPosition)
    {
        //Make sure target position is local to arena
        Node targetNode = NodeFromWorldPoint(transform.InverseTransformPoint(_targetPosition));

        int xIndex = targetNode.gridX;
        int yIndex = targetNode.gridY;

        int numAttempts = 50;

        while (true)
        {
            numAttempts--;

            if (numAttempts < 0)
            {
                //Can't find place around player? Run into player :L
                return (m_mainGrid[targetNode.gridX, targetNode.gridY].WorldPosition);
            }

            xIndex = Random.Range(targetNode.gridX - m_closeWanderDistance, targetNode.gridX + m_closeWanderDistance+1);
            yIndex = Random.Range(targetNode.gridY - m_closeWanderDistance, targetNode.gridY + m_closeWanderDistance+1);

            //Ensure index in range
            if (xIndex < 0 || xIndex >= m_gridSizeX
                || yIndex < 0 || yIndex >= m_gridSizeY)
            {
                continue;
            }
            //Ensure enough clearance
            else if (m_mainGrid[xIndex, yIndex].clearance < _clearance)
            {
                continue;
            }
            else
            {
                //Valid output!
                break;
            }
        }

        return (m_mainGrid[xIndex, yIndex].WorldPosition);
    }

    public Vector3 RequestRandomLocation(int _clearance)
    {
        int xIndex = 0;
        int yIndex = 0;

        while (true)
        {
            xIndex = Random.Range(0, m_gridSizeX);
            yIndex = Random.Range(0, m_gridSizeY);


            if (m_mainGrid[xIndex, yIndex].clearance >= _clearance)
                break;
        }

        return (m_mainGrid[xIndex, yIndex].WorldPosition);
    }

    public Vector3 RequestCirclingLocation(int _clearance, Vector3 _position, Vector3 _rightDirection)
    {
        //Make sure target position is local to arena
        int direction = Random.Range(0, 2);
        Vector3 offset = _rightDirection * 1.5f;

        if (direction == 0) // Flip direction
        {
            offset *= -1; 
        }

        Vector3 wanderCenter = _position + offset;
        Node centerNode = NodeFromWorldPoint(transform.InverseTransformPoint(wanderCenter));


        int xIndex = centerNode.gridX;
        int yIndex = centerNode.gridY;

        int numAttempts = 50;

        while (true)
        {
            numAttempts--;

            if (numAttempts < 0)
            {
                //Can't find place around player? Just don't move!
                Node startNode = NodeFromWorldPoint(transform.InverseTransformPoint(_position));
                return (m_mainGrid[startNode.gridX, startNode.gridY].WorldPosition);
            }

            xIndex = Random.Range(centerNode.gridX - m_circlingDistance, centerNode.gridX + m_circlingDistance + 1);
            yIndex = Random.Range(centerNode.gridY - m_circlingDistance, centerNode.gridY + m_circlingDistance + 1);

            //Ensure index in range
            if (xIndex < 0 || xIndex >= m_gridSizeX
                || yIndex < 0 || yIndex >= m_gridSizeY)
            {
                continue;
            }
            //Ensure enough clearance
            else if (m_mainGrid[xIndex, yIndex].clearance < _clearance)
            {
                continue;
            }
            else
            {
                //Valid output!
                break;
            }
        }

        return (m_mainGrid[xIndex, yIndex].WorldPosition);
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        Vector2 gridPosition = WorldPosToGrid(worldPos);

        if (gridPosition.x < 0 || (gridPosition.x > m_gridSizeX - 1)
            || gridPosition.y < 0 || (gridPosition.y > m_gridSizeY - 1))
            return null;

        return (m_mainGrid[(int)gridPosition.x, (int)gridPosition.y]);
    }

    public Vector2 WorldPosToGrid(Vector3 _worldPos)
    {
        float percentX = (_worldPos.x + m_gridWorldSize.x / 2) / m_gridWorldSize.x;
        float percentY = (_worldPos.z + m_gridWorldSize.y / 2) / m_gridWorldSize.y;
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((m_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSizeY - 1) * percentY);

        return new Vector2(x, y);
    }

    public List<Node> GetNeighbours(Node node)
    {
        int checkX, checkY;
        List<Node> neighbours = new List<Node>();

        // Add adjacent nodes
        for (int i = -1; i < 2; i+=2)
        {
            checkX = node.gridX + i;
            checkY = node.gridY + i;

            if (checkX >= 0 && checkX < m_gridSizeX)
            {
                neighbours.Add(m_mainGrid[checkX, node.gridY]);
            }

            if (checkY >= 0 && checkY < m_gridSizeY)
            {
                neighbours.Add(m_mainGrid[node.gridX, checkY]);
            }
        }

        // Add diagonal nodes, check for walls
        // Currently not working :c
        for (int i = -1; i < 2; i+=2)
        {
            for (int j = -1; j < 2; j+=2)
            {
                checkX = node.gridX + i;
                checkY = node.gridY + j;

                // Check spot is inside grid
                if (checkX >= 0 && checkX < m_gridSizeX
                    && checkY >= 0 && checkY < m_gridSizeY)
                {
                    // Check if adjacent nodes are walkable
                    if (m_mainGrid[checkX, node.gridY].Walkable
                        && m_mainGrid[node.gridX, checkY].Walkable)
                    {
                        neighbours.Add(m_mainGrid[checkX, checkY]);
                    }
                }
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x, 1, m_gridWorldSize.y));

        if (m_mainGrid != null && DisplayGridGizmos)
        {

            foreach (Node n in m_mainGrid)
            {
                if (!n.Walkable)
                    Gizmos.color = Color.red;
                else
                {
                    if (n.clearance == 1)
                        Gizmos.color = Color.white;
                    else
                        Gizmos.color = Color.blue;
                }
                Gizmos.DrawCube(n.WorldPosition - Vector3.up * 0.1f, Vector3.one * (m_nodeDiameter - 0.05f));
            }
        }
    }

    public bool GridReady() { return m_gridReady; }
}
