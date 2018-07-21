using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main point to interact with a ghost object
// Also controls the movement of the ghost,
// along with movement & health stats

[SelectionBase]
public class GhostController : EntityBase {

    // States and objects
    Node m_previousNode;
    CharacterStates m_ghostState, m_oldGhostState;
    GhostAbilityBehaviour m_abilities;

    // Current stats at start of turn
    Vector3 m_positionAtStartOfTurn;
    int m_numMovesLeft;
    public bool m_abilityUsed;

    bool m_OutofSight;

    // Path finding
    Vector3 m_spawnLocation;
    Vector3 m_currentStopPoint;
    List<Vector3> m_pathToFollow;
    bool m_pathFound;
    bool m_performing;

    // TEST: stuff to visialize path
    public Transform m_confirmedPathPrefab;
    public Transform m_choosingPathPrefab;
    public List<Transform> m_confirmedPathBallsList;
    public List<Transform> m_choosingPathBallsList;

    private void Awake()
    {
        Initilaize();
    }

    private void Initilaize()
    {
        // TEST: intialize waypoint lists
        m_confirmedPathBallsList = new List<Transform>();
        m_choosingPathBallsList = new List<Transform>();

        m_ghostState = m_oldGhostState = CharacterStates.IDLE;
        m_abilities = GetComponent<GhostAbilityBehaviour>();
        m_pathToFollow = new List<Vector3>();
        m_performing = false;
        m_previousNode = null;
        m_spawnLocation = transform.position;
        m_currentHealth = m_maxHealth;
        m_OutofSight = true;
    }

    public void ClearChoosingPath()
    {
        // Get rid of potential path
        foreach (Transform t in m_choosingPathBallsList)
            Destroy(t.gameObject);

        m_choosingPathBallsList.Clear();
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

    public void ResetChoosingPathNodes()
    {
        foreach (Transform t in m_choosingPathBallsList)
            Destroy(t.gameObject);

        m_choosingPathBallsList.Clear();
    }

    public void UpdateSkillDirection(Vector3 _point)
    {
        m_abilities.UpdateDirection(_point);
    }

    public void ConfirmSkillDirection()
    {
        m_abilities.ConfirmDirection();
    }

    public Vector3 GetDestinationPosition()
    {
        if (m_pathToFollow.Count > 0)
            return m_pathToFollow[m_pathToFollow.Count - 1];
        else
            return transform.position;
    }

    public void EndMovement()
    {
        m_numMovesLeft = 0;
    }

    public void OnConfirmTargetPosition()
    {
        m_numMovesLeft -= m_choosingPathBallsList.Count; // Lower total amount of moves for future

        foreach (Transform t in m_choosingPathBallsList)
        {
            // Add point for each choosing ball
            m_pathToFollow.Add(t.position);
            m_currentStopPoint = t.position;

            // Temp, spawn some nice balls at confirmed location
            m_confirmedPathBallsList.Add(Instantiate(m_confirmedPathPrefab, t.position, Quaternion.identity));

            // Delete temp path
            Destroy(t.gameObject);
        }
        m_choosingPathBallsList.Clear();

        // Force a new path to be found
        m_previousNode = null;

        // Update the UI
        if(m_numMovesLeft == 0)
            GetComponent<GhostAbilityBehaviour>().m_UIAbilityBar.GetComponent<AbilityBarController>().MoveUsed(true);
        else
            GetComponent<GhostAbilityBehaviour>().m_UIAbilityBar.GetComponent<AbilityBarController>().MoveUsed(false);
    }

    public void OnTargetLocation(Vector3 _position)
    {
        if (m_numMovesLeft < 1) // Do not look for new path if already selected path to take
            return;

        Node newNode = PathRequestManager.Instance().NodeFromWorldPoint(_position);
        if (newNode == m_previousNode) // No need to update path
            return;

        m_previousNode = newNode;

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

        // Safe to ask path request manager!
        PathRequestManager.RequestPath(m_currentStopPoint, _position, 1, OnPathFound);
    }

    void OnPathFound(Vector3[] _path, bool _ifPathFound)
    {
        if (m_performing) // Do not do anything if still performing
            return;

        m_pathFound = _ifPathFound;
        int possibleMoveCount = m_numMovesLeft;

        if (_ifPathFound) {
            // Only take max amount of moves possible
            // Make sure list is clear
            foreach (Transform t in m_choosingPathBallsList)
                Destroy(t.gameObject);
            m_choosingPathBallsList.Clear();
            foreach (Vector3 pathNode in _path) {
                m_choosingPathBallsList.Add(Instantiate(m_choosingPathPrefab, pathNode, Quaternion.identity));
                possibleMoveCount -= 1;

                // Can't move more than this amount
                if (possibleMoveCount < 1)
                    break;
            }
        }
    }

    public override void OnSelected()
    {
        // This Ghost's turn to move!, get where the player wants to move
        m_ghostState = CharacterStates.CHOOSING_WHERE_TO_MOVE;
        MousePicker.Instance().StartPicking(transform.position, this);

        // Update the UI
        GetComponent<GhostAbilityBehaviour>().OnSelected();
    }

    public void OnDeselected()
    {
        ClearChoosingPath();
        m_abilities.OnDeselect();

        // Update the UI
        GetComponent<GhostAbilityBehaviour>().m_UIPortrait.GetComponent<GhostPortraitController>().OnDeselected();
    }

    public void OnStartOfTurn()
    {
        // Reset variables
        m_performing = false;
        m_positionAtStartOfTurn = transform.position;
        m_abilityUsed = false;
        ResetPath();

        // Tell ability manager
        m_abilities.StartOfTurn();
    }

    void ResetPath()
    {
        m_pathToFollow.Clear();
        m_previousNode = null;
        m_currentStopPoint = transform.position;
        m_numMovesLeft = m_maxMoves;

        // TEST: destroy all previous path nodes left

        foreach (Transform t in m_choosingPathBallsList)
            Destroy(t.gameObject);
        m_choosingPathBallsList.Clear();

        foreach (Transform t in m_confirmedPathBallsList)
            Destroy(t.gameObject);
        m_confirmedPathBallsList.Clear();
    }
    public void ResetAction()
    {
        ResetPath();
        m_abilities.ResetAction();
    }

    public Vector3 TargetPoint()
    {
        return m_currentStopPoint;
    }

    public void ResetChosenActions()
    {
        ResetAction();
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

        // Clean up visual stuff
        foreach (Transform t in m_confirmedPathBallsList)
            Destroy(t.gameObject);
        m_confirmedPathBallsList.Clear();

        foreach (Transform t in m_choosingPathBallsList)
            Destroy(t.gameObject);
        m_choosingPathBallsList.Clear();

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

        // Clean previous stored path
        m_pathToFollow.Clear();

        // Perform selected action (maybe pause and rotate towards target first)
        // TODO:
        m_abilities.EndOfTurn();

        m_performing = false;
        yield return null;
    }

    public void SeenbyPunk()
    {
        m_OutofSight = false;
    }
}
