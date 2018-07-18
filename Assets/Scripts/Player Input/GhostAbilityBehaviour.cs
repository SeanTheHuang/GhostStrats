using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection
{
    North,
    East,
    South,
    West
};

public enum GhostActionState
{
    NONE,
    OVERSPOOK,
    HIDE,
    ABILITY
}

public class GhostAbilityBehaviour : MonoBehaviour
{
    PathRequestManager m_pathRequestManager;

    // The direction the ghost is currently facing
    public FacingDirection m_facingDirection;

    // Abilities can only be used if the character has not used this one this turn
    public bool m_abilityUsed;

    // The number of turns the ghost must wait before they can use the ability again.
    [Header("Attack Cooldowns")]
    public int m_attackCooldown;
    public int m_hideCooldown;
    public int m_overwatchCooldown;
    public int m_specialCooldown;

    // The current number of turns the ghost must wait before they can use the ability again.
    [Header("Attack Countdown Timers")]
    public int m_attackCooldownTimer;
    public int m_hideCooldownTimer;
    public int m_overwatchCooldownTimer;
    public int m_specialCooldownTimer;

    // The grid squares the ghost attacks in
    public List<Vector3> m_attackSquares;

    private void Start()
    {
        m_pathRequestManager = PathRequestManager.Instance();
        m_facingDirection = FacingDirection.North;

        // Convert the attack squares to be equal to the length of a grid square
        for (int i = 0; i < m_attackSquares.Count; ++i)
        {
            m_attackSquares[i] = m_attackSquares[i] * m_pathRequestManager.GridSize();
        }
    }

    public void Attack()
    {
        Vector3 rotationModifier = new Vector3(1, 1, 1);

        // Modify the attack grid squares dependant on the direction the ghost is facing
        if (m_facingDirection == FacingDirection.East)
            rotationModifier.z = -1;
        else if (m_facingDirection == FacingDirection.South)
        {
            rotationModifier.z = -1;
            rotationModifier.x = -1;
        }
        else if (m_facingDirection == FacingDirection.West)
            rotationModifier.x = -1;

        // Set all the attack positions to world space
        List<Vector3> attackpositions = new List<Vector3>();
        for(int i = 0; i < m_attackSquares.Count; ++i)
        {
            attackpositions.Add(new Vector3((m_attackSquares[i].x * rotationModifier.x) + transform.position.x, transform.position.y,
                                            (m_attackSquares[i].z * rotationModifier.z) + transform.position.z));
        }

        // Check the grid squares to see if there are punks in them
        //List<GameObject> m_punks = m_pathRequestManager.GetObjectsFromListOfPositions<GameObject>(attackpositions, NodeState.PUNK);

        // TODO ADD DAMAGING THE LIST OF PUNKS

        m_attackCooldownTimer = m_attackCooldown; // Update the timer
        m_abilityUsed = true;
    }

    public void Hide()
    {
        Debug.Log("Ghost Hide");
        m_hideCooldownTimer = m_hideCooldown; // Update the timer
        m_abilityUsed = true;
    }

    public void Overwatch()
    {
        Debug.Log("Ghost Overwatch");
        m_overwatchCooldownTimer = m_overwatchCooldown; // Update the timer
        m_abilityUsed = true;
    }

    public virtual void Special()
    {
        Debug.Log("Ghost Special");
        m_specialCooldown = m_specialCooldownTimer; // Update the timer
        m_abilityUsed = true;
    }
}
