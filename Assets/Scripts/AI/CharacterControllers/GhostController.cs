using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GhostController : EntityBase {

    private CharacterStates m_ghostState, m_oldGhostState;

    private Vector3 m_positionAtStartOfTurn;
    private int m_numMovesLeft;

    List<Vector3> m_pathToFollow;
    bool m_pathFound;
    bool m_performing;

    private void Awake()
    {
        Initilaize();   
    }
    private void Initilaize()
    {
        m_ghostState = m_oldGhostState = CharacterStates.IDLE;
        m_pathToFollow = new List<Vector3>();
        m_performing = false;
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

        PathRequestManager.RequestPath(transform.position, _position, 1, OnPathFound);
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
                // Can't move more than this amount
                if (m_numMovesLeft < 1)
                    break;
            }
        }
    }

    public override void Move()
    {
        // This Ghost's turn to move!, get where the player wants to move
        m_ghostState = CharacterStates.CHOOSING_WHERE_TO_MOVE;
        MousePicker.Instance().StartPicking(transform.position, OnTargetLocation);
    }

    public void OnStartOfTurn()
    {
        // Clear the path
        m_pathToFollow.Clear();
        m_performing = false;
        m_positionAtStartOfTurn = transform.position;
        m_numMovesLeft = m_maxMoves;
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
