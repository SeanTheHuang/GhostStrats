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
    public GhostHole m_ghostSpawner;
    private GhostUi m_ghostUI;
    private Animator m_ghostAnimator;

    // Current stats at start of turn
    Vector3 m_positionAtStartOfTurn;
    public int m_numMovesLeft;
    public bool m_abilityUsed;
    
    public bool m_performing
    { get; private set; }
    
    public bool GhostIsAlive
    {
        get; private set;
    }

    bool m_OutofSight;

    // Path finding
    Vector3 m_spawnLocation;
    Vector3 m_currentStopPoint;
    List<Vector3> m_pathToFollow;
    bool m_pathFound;

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
        m_ghostUI = GetComponent<GhostUi>();
        m_pathToFollow = new List<Vector3>();
        m_performing = false;
        m_previousNode = null;
        m_spawnLocation = transform.position;
        m_currentHealth = m_maxHealth;
        m_OutofSight = true;
        m_ghostAnimator = transform.Find("Model").GetComponent<Animator>();
    }

    #region EVENT_FUNCTIONS

    public override void OnDeath()
    {
        if (m_ghostSpawner)
            m_ghostSpawner.OnGhostDeath();
    }

    public override void OnSpawn()
    {
    }

    public void MoveToPositionImmediate(Vector3 _targetPosition)
    {
        StartCoroutine(MoveToLocation(_targetPosition));
    }

    IEnumerator MoveToLocation(Vector3 _targetPosition)
    {
        Vector3[] path = PathRequestManager.Instance().GetPathImmediate(transform.position, _targetPosition, 1);

        // Initial rotation
        if (path.Length > 0)
        {
            // Update rotation
            Vector3 dir = path[0] - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        for (int i = 0; i < path.Length; i++)
        {
            while (true)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, path[i], m_moveSpeed * Time.deltaTime);
                if (newPosition == transform.position) // Reached point, move onto next!
                {
                    if (i + 1 < path.Length)
                    {
                        // Update rotation
                        Vector3 dir = path[i + 1] - transform.position;
                        dir.y = 0;
                        transform.rotation = Quaternion.LookRotation(dir);
                    }
                    break;
                }
                else
                    transform.position = newPosition; // Not reached, move the player!

                yield return null; // Move over a number of frames
            }
        }

        m_ghostSpawner.m_respawnAnimationDone = true;
        yield return null;
    }

    public void HideGhost()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        GhostIsAlive = false;

        foreach (MeshRenderer rend in renderers)
            rend.enabled = false;
    }

    public void ShowGhost()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        GhostIsAlive = true;

        foreach (MeshRenderer rend in renderers)
            rend.enabled = true;
    }

    public override void OnEntityHit(int _damage)
    {
        Debug.Log(transform.name + " has been hit for " + _damage.ToString() + " damage.");

        m_currentHealth -= _damage;

        if (m_currentHealth < 0)
            m_currentHealth = 0;

        if (m_currentHealth == 0)
        {
            if (m_ghostAnimator != null)
                m_ghostAnimator.SetTrigger("Death");
            GhostIsAlive = false;
        }
        else
        {
            if (m_ghostAnimator != null)
                m_ghostAnimator.SetTrigger("Damaged");
        }

        m_ghostUI.updateHealthbar(m_currentHealth);
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

        m_ghostUI.MoveUsed(m_numMovesLeft);
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

        if (_ifPathFound)
        {
            // Only take max amount of moves possible
            // Make sure list is clear
            foreach (Transform t in m_choosingPathBallsList)
                Destroy(t.gameObject);
            m_choosingPathBallsList.Clear();
            foreach (Vector3 pathNode in _path)
            {
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
        //CameraControl.Instance.SetFreeMode(transform.position);

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
        m_ghostUI.OnDeselected();
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

    public void OnEndOfTurn()
    {
        // Move here, and perform chosen action
        StartCoroutine(PerformAction());
    }

    public void ClearChoosingPath()
    {
        // Get rid of potential path
        foreach (Transform t in m_choosingPathBallsList)
            Destroy(t.gameObject);

        m_choosingPathBallsList.Clear();
    }

    #endregion

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

    

    public override void ChooseAction()
    {
    }

    IEnumerator PerformAction()
    {
        if (m_performing) // Just keep doing nothing while performing
            yield return null;

        m_performing = true;

        // Clean up visual stuff
        foreach (Transform t in m_confirmedPathBallsList)
            Destroy(t.gameObject);
        m_confirmedPathBallsList.Clear();

        foreach (Transform t in m_choosingPathBallsList)
            Destroy(t.gameObject);
        m_choosingPathBallsList.Clear();

        // Character intial rotation
        if (m_pathToFollow.Count > 0)
        {
            Vector3 dir = m_pathToFollow[0] - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Start by moving to target location
        m_ghostState = CharacterStates.MOVING;
        for (int i = 0; i < m_pathToFollow.Count; i++) {
            while (true) {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, m_pathToFollow[i], m_moveSpeed * Time.deltaTime);
                if (newPosition == transform.position) // Reached point, move onto next!
                {
                    if (i + 1 < m_pathToFollow.Count)
                    {
                        // Update rotation
                        Vector3 dir = m_pathToFollow[i+1] - transform.position;
                        dir.y = 0;
                        transform.rotation = Quaternion.LookRotation(dir);
                    }
                    break;
                }
                else
                    transform.position = newPosition; // Not reached, move the player!

                yield return null; // Move over a number of frames
            }
        }

        // Clean previous stored path
        m_pathToFollow.Clear();

        // Perform selected action (maybe pause and rotate towards target first)
        m_abilities.EndOfTurn();

        m_performing = false;
        yield return null;
    }

    public void SeenbyPunk()
    {
        m_OutofSight = false;
    }
}
