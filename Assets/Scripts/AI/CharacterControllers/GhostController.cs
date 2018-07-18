using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main point to interact with a ghost object
// Also controls the movement of the ghost,
// along with movement & health stats

[SelectionBase]
public class GhostController : EntityBase {

    // States and objects
    Vector3 m_previousTile;
    CharacterStates m_ghostState, m_oldGhostState;
    GhostAbilityBehaviour m_abilities;

    // Current stats at start of turn
    Vector3 m_positionAtStartOfTurn;
    int m_numMovesLeft;

    // Path finding
    Vector3 m_spawnLocation;
    Vector3 m_currentStopPoint;
    List<Vector3> m_pathToFollow;
    bool m_pathFound;
    bool m_performing;

    // TEST: stuff to visialize path
    public Transform m_wayPointPrefab;
    public Transform m_destinationPrefab;
    public List<Transform> m_wayPointNodeList;
    public List<Transform> m_destinationNodeList;

    private void Awake()
    {
        Initilaize();   
    }
    private void Initilaize()
    {
        // TEST: intialize waypoint lists
        m_wayPointNodeList = new List<Transform>();
        m_destinationNodeList = new List<Transform>();

        m_ghostState = m_oldGhostState = CharacterStates.IDLE;
        m_abilities = GetComponent<GhostAbilityBehaviour>();
        m_pathToFollow = new List<Vector3>();
        m_performing = false;
        m_spawnLocation = transform.position;
        m_currentHealth = m_maxHealth;
    }

    public override void OnDeath()
    {
    }
    public override void OnSpawn()
    {
    }
    public override void OnEntityHit()
    {
    }

    void OnTargetLocation(Vector3 _position)
    {
        if (m_numMovesLeft < 1) // Do not look for new path if already selected path to take
            return;

        // Check if possible to move to target position
        if (!PathRequestManager.Instance().PointIsInGrid(_position))
            return;

        List<Vector3> pointList = new List<Vector3>();
        pointList.Add(_position);

        // Check for any ghosts or punks at point
        if (GameMaster.Instance().GetGhostsAtLocations(pointList, true).Count > 0)
            return;

        if (GameMaster.Instance().GetPunksAtLocations(pointList).Count > 0)
            return;

        // Path is perfectly clear!
        PathRequestManager.RequestPath(m_currentStopPoint, _position, 1, OnPathFound);
    }

    void OnPathFound(Vector3[] _path, bool _ifPathFound)
    {
        if (m_performing) // Do not do anything if still performing
            return;

        m_pathFound = _ifPathFound;

        if (_ifPathFound) {
            // Only take max amount of moves possible
            foreach (Vector3 pathNode in _path) {
                m_pathToFollow.Add(pathNode);
                m_numMovesLeft -= 1;
                m_currentStopPoint = pathNode;

                // TEST: create path node on points where we wanna go
                if (m_destinationPrefab)
                {
                    // Only do this if destination prefabs exist ~
                    if (m_numMovesLeft < 1 || pathNode == _path[_path.Length - 1]) 
                        m_destinationNodeList.Add(Instantiate(m_destinationPrefab, pathNode, Quaternion.identity));
                    else
                        m_wayPointNodeList.Add(Instantiate(m_wayPointPrefab, pathNode, Quaternion.identity));
                }

                // Can't move more than this amount
                if (m_numMovesLeft < 1)
                    break;
            }
        }
    }

    public override void SelectingWhereToMove()
    {
        // This Ghost's turn to move!, get where the player wants to move
        m_ghostState = CharacterStates.CHOOSING_WHERE_TO_MOVE;
        MousePicker.Instance().StartPicking(transform.position, OnTargetLocation, ResetChosenActions);
    }

    public void OnStartOfTurn()
    {
        // Reset variables
        m_performing = false;
        m_positionAtStartOfTurn = transform.position;

        ResetPath();
    }

    void ResetPath()
    {
        m_pathToFollow.Clear();
        m_currentStopPoint = transform.position;
        m_numMovesLeft = m_maxMoves;

        // TEST: destroy all previous path nodes left
        
        foreach (Transform t in m_destinationNodeList)
            Destroy(t.gameObject);
        m_destinationNodeList.Clear();

        foreach (Transform t in m_wayPointNodeList)
            Destroy(t.gameObject);
        m_wayPointNodeList.Clear();
    }

    public Vector3 TargetPoint()
    {
        return m_currentStopPoint;
    }

    public void ResetChosenActions()
    {
        ResetPath();
        // TODO: Reset chosen skills
    }

    public void OnEndOfTurn()
    {
        // Move here, and perform chosen action
        StartCoroutine(PerformAction());
    }

    public override void ChooseAction()
    {
    }

    IEnumerator PerformAction()
    {
        if (m_performing) // Just keep doing nothing while performing
            yield return null;

        m_performing = true;
        // Start by moving to target location
        m_ghostState = CharacterStates.MOVING;
        foreach (Vector3 wayPoint in m_pathToFollow) {
            while (true) {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, wayPoint, m_moveSpeed * Time.deltaTime);
                if (newPosition == transform.position) // Reached point, move onto next!
                    break;
                else
                    transform.position = newPosition; // Not reached, move the player!

                yield return null;
            }
        }


        // Perform selected action (maybe pause and rotate towards target first)
        // TODO:

        m_performing = false;
        yield return null;
    }
}
