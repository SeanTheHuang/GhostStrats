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
    int m_layerMask;

    // Current stats at start of turn
    Vector3 m_positionAtStartOfTurn;
    public int m_numMovesLeft;
    public bool m_abilityUsed;

    [HideInInspector]
    public GhostAimModel m_aimModel;

    public bool m_performing
    { get; private set; }
    
    public bool GhostIsAlive
    {
        get; private set;
    }

    public bool m_OutofSight;

    // Path finding
    Vector3 m_spawnLocation;
    Vector3 m_currentStopPoint;
    List<Vector3> m_movePath1; // Movement before ability
    List<Vector3> m_movePath2; // Movement after ability
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
        // If aim model, throw it out of parent
        m_aimModel = GetComponentInChildren<GhostAimModel>();
        if (m_aimModel)
            m_aimModel.transform.SetParent(null);

        // TEST: intialize waypoint lists
        m_confirmedPathBallsList = new List<Transform>();
        m_choosingPathBallsList = new List<Transform>();

        m_ghostState = m_oldGhostState = CharacterStates.IDLE;
        m_abilities = GetComponent<GhostAbilityBehaviour>();
        m_ghostUI = GetComponent<GhostUi>();
        m_movePath1 = new List<Vector3>();
        m_movePath2 = new List<Vector3>();
        m_performing = false;
        m_previousNode = null;
        m_spawnLocation = transform.position;
        m_currentHealth = m_maxHealth;
        m_OutofSight = false;
        m_layerMask = gameObject.layer;
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
                    // Check of punks should be facing ghost
                    GameMaster.Instance().CheckGhostWalkPast(transform.position);
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
        // Make sure mouse can't hit dead ghost
        gameObject.layer = 2;

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] sm_renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        GhostIsAlive = false;

        foreach (MeshRenderer rend in renderers)
            rend.enabled = false;

        foreach (SkinnedMeshRenderer rend in sm_renderers)
            rend.enabled = false;
    }

    public void ShowGhost()
    {
        // ASSUMING THIS IS ONLY CALLED WHEN THEY RESPAWN
        m_currentHealth = m_maxHealth;

        gameObject.layer = m_layerMask; // Make sure mouse can hit ghost again
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] sm_renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        GhostIsAlive = true;

        foreach (MeshRenderer rend in renderers)
            rend.enabled = true;

        foreach (SkinnedMeshRenderer rend in sm_renderers)
            rend.enabled = true;
    }

    public override void OnEntityHit(int _damage, Vector3 _positionOfHitter)
    {
        Debug.Log(transform.name + " has been hit for " + _damage.ToString() + " damage.");

        m_currentHealth = Mathf.Clamp(m_currentHealth - _damage, 0, 1000);

        Vector3 dir = _positionOfHitter - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
        m_abilities.OnHit();
        TextEffectController.Instance.PlayEffectText(transform.position, TextEffectTypes.PUNK_DAMAGE, _damage);
        if (m_currentHealth < 1)
        {
            if (m_ghostAnimator != null)
                m_ghostAnimator.SetTrigger("Death");

            if (m_ghostSpawner)
                m_ghostSpawner.OnGhostDeath();

            Debug.Log(name + " has died.");
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
            if (m_abilities.m_actionState == GhostActionState.NONE)
                m_movePath1.Add(t.position);
            else
                m_movePath2.Add(t.position);

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

        if (m_aimModel && !m_aimModel.m_locked)
            m_aimModel.transform.position = m_choosingPathBallsList[m_choosingPathBallsList.Count-1].position;
    }

    public override void OnSelected()
    {
        CameraControl.Instance.SetFreeMode(transform.position);

        // This Ghost's turn to move!, get where the player wants to move
        m_ghostState = CharacterStates.CHOOSING_WHERE_TO_MOVE;
        MousePicker.Instance().StartPicking(transform.position, this);

        // Update the UI
        m_abilities.OnSelected();

        if (m_aimModel)
            m_aimModel.ShowAimModel();
    }

    public void OnDeselected()
    {
        ClearChoosingPath();
        m_abilities.OnDeselect();

        // Update the UI
        m_ghostUI.OnDeselected();

        if (m_aimModel)
            if (!m_aimModel.m_locked)
                m_aimModel.HideAimModel();
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
        // Set aim model real far out initially
        if (m_aimModel)
        {
            m_aimModel.transform.position = Vector3.up * 1000;
            m_aimModel.transform.rotation = transform.rotation;
        }
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

    public bool IsOverwatchingPosition(PunkController _punk)
    {
        if (m_abilities.IsOverwatchingPosition(_punk))
        {
            if (m_ghostAnimator)
                m_ghostAnimator.SetTrigger("Attack");

            return true;
        }

        return false;
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
        if (m_movePath1.Count > 0)
            return m_movePath1[m_movePath1.Count - 1];
        else
            return transform.position;
    }

    public void EndMovement()
    {
        m_numMovesLeft = 0;
    }

    void ResetPath()
    {
        m_movePath1.Clear();
        m_movePath2.Clear();
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
        // Make aim model go away
        if (m_aimModel)
        {
            m_aimModel.transform.position = Vector3.up * 1000;
            m_aimModel.transform.rotation = transform.rotation;
            m_aimModel.ResetAimModel();
        }

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

        if (m_aimModel)
        {
            m_aimModel.ResetAimModel();
            m_aimModel.HideAimModel();
        }

        // Character intial rotation
        if (m_movePath1.Count > 0)
        {
            Vector3 dir = m_movePath1[0] - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Start by moving to target location
        m_ghostState = CharacterStates.MOVING;
        for (int i = 0; i < m_movePath1.Count; i++) {
            while (true) {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, m_movePath1[i], m_moveSpeed * Time.deltaTime);
                if (newPosition == transform.position) // Reached point, move onto next!
                {
                    if (i + 1 < m_movePath1.Count)
                    {
                        // Update rotation
                        Vector3 dir = m_movePath1[i+1] - transform.position;
                        dir.y = 0;
                        transform.rotation = Quaternion.LookRotation(dir);
                    }
                    // Check of punks should be facing ghost
                    GameMaster.Instance().CheckGhostWalkPast(transform.position);
                    break;
                }
                else
                    transform.position = newPosition; // Not reached, move the player!

                yield return null; // Move over a number of frames
            }
        }

        // Clean previous stored path
        m_movePath1.Clear();

        // Perform selected action (maybe pause and rotate towards target first)
        m_abilities.EndOfTurn();

        if (m_abilities.m_actionState != GhostActionState.NONE)
            yield return new WaitForSeconds(0.8f); // Pause for a bit for animation

        // Start moving again if theres more
        for (int i = 0; i < m_movePath2.Count; i++)
        {
            while (true)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, m_movePath2[i], m_moveSpeed * Time.deltaTime);
                if (newPosition == transform.position) // Reached point, move onto next!
                {
                    if (i + 1 < m_movePath2.Count)
                    {
                        // Update rotation
                        Vector3 dir = m_movePath2[i + 1] - transform.position;
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

        if (m_movePath2.Count > 0)
            yield return new WaitForSeconds(0.5f);

        m_performing = false;
        yield return null;
    }

    public void SeenbyPunk()
    {
        m_OutofSight = false;
    }

    public void resetUIHealth()
    {
        m_ghostUI.updateHealthbar(m_currentHealth);
    }

    // Called when the ghosts spawner gets destroyed
    public void OnHardDeath()
    {
        m_ghostUI.OnGhostHardDeath();
    }
}
