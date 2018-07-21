using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AimingDirection
{
    North = 0,
    East = 90,
    South = 180,
    West = 270
};

public enum GhostActionState
{
    NONE,
    BOO,
    OVERSPOOK,
    HIDE,
    ABILITY
}

public class GhostAbilityBehaviour : MonoBehaviour
{
    PathRequestManager m_pathRequestManager;
    GhostController m_ghostController;
    GameMaster m_gameMaster;

    public Transform m_attackTilePrefab;
    List<Transform> m_attackTileList;

    GhostActionState m_actionState;
    AimingDirection m_aimingDirection; // The direction the ghost is currently facing
    public bool m_abilityUsed; // Abilities can only be used if the character has not used this one this turn
    //public bool m_abilityUsed; // Abilities can only be used if the character has not used this one this turn
    bool m_aimingAbility; // If the player is aiming their ability

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

    public GameObject m_UIPortrait;
    public GameObject m_UIAbilityBar;

    // The grid squares the ghost attacks in
    [Header("Base affected tiles")]
    public List<Vector3> m_attackSquares;
    public List<Vector3> m_overspookSquares;
    List<Vector3> m_currentAffectedSquares;
    List<Vector3> m_rotatedAffectedSquares;

    private void Awake()
    {
        m_attackTileList = new List<Transform>();
        m_ghostController = GetComponent<GhostController>();
        m_rotatedAffectedSquares = new List<Vector3>();
        m_actionState = GhostActionState.NONE;
        m_currentAffectedSquares = m_attackSquares;
        m_aimingAbility = false;
    }

    private void Start()
    {
        m_pathRequestManager = PathRequestManager.Instance();
        m_aimingDirection = AimingDirection.North;
        m_gameMaster = GameMaster.Instance();

        // Convert the attack squares to be equal to the length of a grid square
        for (int i = 0; i < m_attackSquares.Count; ++i)
        {
            m_attackSquares[i] = m_attackSquares[i] * m_pathRequestManager.GridSize() * 2;
        }
    }

    private void Update()
    {
        // Used for aiming ability
        if (!m_aimingAbility)
            return;

        // Update point to world
        Vector3 objectPosition = Camera.main.WorldToScreenPoint(m_ghostController.GetDestinationPosition());
        objectPosition.z = 0;
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        UpdateDirection((objectPosition - mousePosition));

        // Confirm target location
        if (Input.GetButtonDown("Fire1"))
        {
            ConfirmDirection();
        }
    }

    // Sets the ability to be used. Tells the GameMaster to check if all abilities have been used 
    void AbilityUsed()
    {
        m_ghostController.m_abilityUsed = true;
        m_gameMaster.CheckAllPlayerActionsUsed();
    }

    void AbilityUnused()
    {
        m_ghostController.m_abilityUsed = false;
        m_gameMaster.ResetEndTurnPrompt();
    }

    public void OnSelected()
    {
        // Reset aiming direction
        m_aimingDirection = AimingDirection.North;
        m_UIPortrait.GetComponent<GhostPortraitController>().OnSelected();
        m_UIAbilityBar.GetComponent<AbilityBarController>().OnSelected(m_attackCooldownTimer, m_hideCooldownTimer, m_overwatchCooldownTimer, m_specialCooldownTimer, false, false);
    }

    // Reset action variables if one has not been chosen yet
    public void OnDeselect()
    {
        if (m_abilityUsed)
            return;

        ResetAction();
    }

    public void UpdateDirection(Vector3 _newDir)
    {
        // Use dot product and figure out which direction direct facing
        AimingDirection newDirection = UpdateAimingFromDirection(_newDir);

        if (newDirection == m_aimingDirection) // No need to update
            return;

        // Else, update attack tiles and visuals
        m_aimingDirection = newDirection;
        UpdateAttackTiles();
        UpdateAtackVisuals();
    }

    AimingDirection UpdateAimingFromDirection(Vector3 _dir)
    {
        float angle = Mathf.Atan2(_dir.x, _dir.y) * Mathf.Rad2Deg;
        if (angle < 0)
            angle += 360.0f;

        if (angle >= 45 && angle <= 135)
            return AimingDirection.West;
        else if (angle >= 135 && angle <= 225)
            return AimingDirection.North;
        else if (angle >= 225 && angle <= 315)
            return AimingDirection.East;
        else
            return AimingDirection.South;
    }

    public void ConfirmDirection()
    {
        m_ghostController.ClearChoosingPath();
        m_ghostController.EndMovement();
        MousePicker.Instance().FinishAimingAbility();
        m_attackCooldownTimer = m_attackCooldown; // Update the timer
        AbilityUsed();
        m_aimingAbility = false;
    }

    void UpdateAtackVisuals()
    {
        ClearAttackVisuals();
        foreach (Vector3 v3 in m_rotatedAffectedSquares)
        {
            m_attackTileList.Add(Instantiate(m_attackTilePrefab, v3, Quaternion.identity));
        }
    }

    void UpdateAttackTiles()
    {
        //Vector3 rotationModifier = new Vector3(1, 1, 1);

        //// Modify the attack grid squares dependant on the direction the ghost is facing
        //if (m_aimingDirection == AimingDirection.East)
        //    rotationModifier.z = -1;
        //else if (m_aimingDirection == AimingDirection.South)
        //{
        //    rotationModifier.z = -1;
        //    rotationModifier.x = -1;
        //}
        //else if (m_aimingDirection == AimingDirection.West)
        //    rotationModifier.x = -1;

        Vector3 pointAtWhichUseSkill = m_ghostController.GetDestinationPosition();

        if (Input.GetKeyDown(KeyCode.T))
            Debug.Log(m_aimingDirection);

        // Set all the attack positions to world space
        m_rotatedAffectedSquares.Clear();
        for (int i = 0; i < m_currentAffectedSquares.Count; ++i)
        {
            //m_rotatedAffectedSquares.Add(new Vector3((m_currentAffectedSquares[i].x * rotationModifier.x) + pointAtWhichUseSkill.x, pointAtWhichUseSkill.y,
            //                                (m_currentAffectedSquares[i].z * rotationModifier.z) + pointAtWhichUseSkill.z));
            m_rotatedAffectedSquares.Add(Quaternion.AngleAxis((float)m_aimingDirection, Vector3.up) * m_currentAffectedSquares[i] + pointAtWhichUseSkill);
        }
    }

    void ClearAttackVisuals()
    {
        foreach (Transform t in m_attackTileList)
            Destroy(t.gameObject);
        m_attackTileList.Clear();
    }

    public void PerformAttack()
    {
        ClearAttackVisuals();
    }

    public void StartOfTurn()
    {
        AbilityUnused();
        m_aimingAbility = false;
        m_actionState = GhostActionState.NONE;
    }

    public void EndOfTurn()
    {
        // Perform attack
    }

    public void ResetAction()
    {
        ClearAttackVisuals();
        m_actionState = GhostActionState.NONE;
        m_abilityUsed = false;
        m_aimingAbility = false;
        AbilityUnused();
    }

    public void ChooseAttack()
    {
        m_actionState = GhostActionState.BOO;
        m_ghostController.ClearChoosingPath();
        MousePicker.Instance().PausePicking();
        m_currentAffectedSquares = m_attackSquares;
        m_aimingAbility = true;

        // Spawn intial values
        UpdateAttackTiles();
        UpdateAtackVisuals();
    }

    public void Hide()
    {
        Debug.Log("Ghost Hide");
        m_actionState = GhostActionState.BOO;
        m_ghostController.ClearChoosingPath();
        m_ghostController.EndMovement();
        m_hideCooldownTimer = m_hideCooldown; // Update the timer
        AbilityUsed();
    }

    public void Overwatch()
    {
        Debug.Log("Ghost Overwatch");
        m_aimingAbility = true;
        m_overwatchCooldownTimer = m_overwatchCooldown; // Update the timer
    }

    public virtual void Special()
    {
        Debug.Log("Ghost Special");
        m_specialCooldown = m_specialCooldownTimer; // Update the timer
    }
}
