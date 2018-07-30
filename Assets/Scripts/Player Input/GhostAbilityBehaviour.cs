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

public enum GhostType
{
    NULL,
    WALLER,
    DOLLER,
    MONSTER,
    SCREAMER
}

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
    protected GhostController m_ghostController;
    GameMaster m_gameMaster;
    private Animator m_animator;

    public Transform m_attackTilePrefab;
    List<Transform> m_attackTileList;

    public GhostType m_ghostType
    { get; protected set; }

    public GhostActionState m_actionState
    { get; protected set; }

    protected AimingDirection m_aimingDirection; // The direction the ghost is currently facing
    public bool m_abilityUsed; // Abilities can only be used if the character has not used this one this turn
    //public bool m_abilityUsed; // Abilities can only be used if the character has not used this one this turn
    bool m_aimingAbility; // If the player is aiming their ability

    private GhostUi m_ghostUI;

    private GameObject m_ghostNormalModel;
    private GameObject m_ghostHideModel;

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
    [Header("Base affected tiles")]
    public List<Vector3> m_attackSquares;
    public List<Vector3> m_overspookSquares;
    public List<Vector3> m_specialSkillSquares;
    protected List<Vector3> m_currentAffectedSquares;
    protected List<Vector3> m_rotatedAffectedSquares;

    [Header("Ability strength")]
    public int m_baseAttackDamage = 2;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        // Convert the attack squares to be equal to the length of a grid square
        m_pathRequestManager = PathRequestManager.Instance();
        m_gameMaster = GameMaster.Instance();
        for (int i = 0; i < m_attackSquares.Count; ++i)
        {
            m_attackSquares[i] = m_attackSquares[i] * m_pathRequestManager.GridSize() * 2;
        }
        SetGhostType();

        m_ghostUI = GetComponent<GhostUi>();
        m_animator = transform.Find("Model").GetComponent<Animator>();
        m_ghostNormalModel = transform.Find("Model").gameObject;
        m_ghostHideModel = transform.Find("HideModel").gameObject;
    }

    protected virtual void Initialize()
    {
        m_attackTileList = new List<Transform>();
        m_ghostController = GetComponent<GhostController>();
        m_rotatedAffectedSquares = new List<Vector3>();
        m_actionState = GhostActionState.NONE;
        m_currentAffectedSquares = m_attackSquares;
        m_aimingAbility = false;

        m_aimingDirection = AimingDirection.North;
    }

    protected virtual void SetGhostType()
    {
        m_ghostType = GhostType.NULL;
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
        else if (Input.GetButtonDown("Fire2"))
        {
            // Right click = cancel ability selection
            ResetAction();
            MousePicker.Instance().FinishAimingAbility();
        }
    }

    // Sets the ability to be used. Tells the GameMaster to check if all abilities have been used 
    void AbilityUsed()
    {
        m_abilityUsed = true;
        m_ghostController.m_abilityUsed = true;
        m_gameMaster.CheckAllPlayerActionsUsed();
        m_ghostUI.AbilityUsed(m_actionState);
    }

    void AbilityUnused()
    {
        m_ghostController.m_abilityUsed = false;
        m_gameMaster.ResetEndTurnPrompt();
        OnSelected();
    }

    public void OnSelected()
    {

        // Reset Ability Bar
        bool movedUsed = true;
        if (m_ghostController.m_numMovesLeft > 0)
            movedUsed = false;

        bool someMoveUsed = false;
        if (m_ghostController.m_maxMoves > m_ghostController.m_numMovesLeft)
            someMoveUsed = true;

        m_ghostUI.OnSelected(m_attackCooldownTimer, m_hideCooldownTimer, m_overwatchCooldownTimer, m_specialCooldownTimer, movedUsed, someMoveUsed, m_actionState);
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
        {
            if (m_ghostController.m_aimModel)
                m_ghostController.m_aimModel.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
            return AimingDirection.West;
        }
        else if (angle >= 135 && angle <= 225)
        {
            if (m_ghostController.m_aimModel)
                m_ghostController.m_aimModel.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            return AimingDirection.North;
        }
        else if (angle >= 225 && angle <= 315)
        {
            if (m_ghostController.m_aimModel)
                m_ghostController.m_aimModel.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
            return AimingDirection.East;
        }
        else
        {
            if (m_ghostController.m_aimModel)
                m_ghostController.m_aimModel.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            return AimingDirection.South;
        }
    }

    public virtual void ConfirmDirection()
    {
        if (m_actionState != GhostActionState.ABILITY)
            m_ghostController.EndMovement();

        MousePicker.Instance().FinishAimingAbility();
        //Debug.Log("Confirmed ability targets!");
        AbilityUsed();
        m_aimingAbility = false;

        if (m_ghostController.m_aimModel)
            m_ghostController.m_aimModel.m_locked = true;
    }

    protected void StartAimingAbility()
    {
        m_ghostController.ClearChoosingPath();
        MousePicker.Instance().PausePicking();
        m_aimingAbility = true;

        // Spawn intial values
        UpdateAttackTiles();
        UpdateAtackVisuals();
    }

    protected void ImmediateConfirmAbility()
    {
        MousePicker.Instance().FinishAimingAbility(); // Just incase aiming doesn't come back
        m_ghostController.ClearChoosingPath();
        m_aimingAbility = false;
        ClearAttackVisuals();
        AbilityUsed();
    }

    public bool IsOverwatchingPosition(PunkController _punk)
    {
        if (m_actionState != GhostActionState.OVERSPOOK)
            return false;

        float gridSize = PathRequestManager.Instance().GridSize();
        if ((_punk.transform.position - m_rotatedAffectedSquares[0]).sqrMagnitude < gridSize * gridSize)
        {
            // Check punk not directly staring at ghost
            Vector3 punkToGhost = transform.position - _punk.transform.position;
            if (Vector3.Dot(_punk.transform.forward, punkToGhost.normalized) < 0.9f)
            {
                // Attack punk and return true
                _punk.OnEntityHit(m_baseAttackDamage, transform.position);
                m_actionState = GhostActionState.NONE; // No more overwatching
                SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.GHOST_ATTACK);
                return true;
            }
        }

        return false;
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

    public virtual void StartOfTurn()
    {
        if (m_actionState == GhostActionState.HIDE)
        {
            m_ghostController.m_OutofSight = false;
            m_ghostHideModel.SetActive(false);
            m_ghostNormalModel.SetActive(true);
        }

        m_aimingAbility = false;
        m_actionState = GhostActionState.NONE;

        LowerCooldown();
    }

    void LowerCooldown()
    {
        m_attackCooldownTimer = Mathf.Clamp(m_attackCooldownTimer - 1, 0, 1000);
        m_hideCooldownTimer = Mathf.Clamp(m_hideCooldownTimer - 1, 0, 1000);
        m_overwatchCooldownTimer = Mathf.Clamp(m_overwatchCooldownTimer - 1, 0, 1000);
        m_specialCooldownTimer = Mathf.Clamp(m_specialCooldownTimer - 1, 0, 1000);
    }

    public void OnHit()
    {
        if(m_ghostController.GetCurrentHealth() == 0)
            m_animator.SetTrigger("Death");
        else
            m_animator.SetTrigger("Damaged");

        if (m_actionState == GhostActionState.OVERSPOOK)
            // Stop overwatching
            m_actionState = GhostActionState.NONE;

        else if (m_actionState == GhostActionState.HIDE)
        {
            m_actionState = GhostActionState.NONE;
            PathRequestManager.Instance().SetNodeState(NodeState.EMPTY, transform);
            // TODO, play unhide animation
            m_ghostHideModel.SetActive(false);
            m_ghostNormalModel.SetActive(true);
        }
    }

    public void EndOfTurn()
    {
        switch (m_actionState)
        {
            case GhostActionState.BOO:
                PerformAttack();

                break;
            case GhostActionState.OVERSPOOK:
                PerformOverwatch();

                break;
            case GhostActionState.HIDE:
                PerformHide();

                break;
            case GhostActionState.ABILITY:          
                PerformSpecialAbility();
                m_animator.SetTrigger("Special");

                break;
            default:
                break;
        }
    }

    public void ResetAction()
    {
        ClearAttackVisuals();
        m_actionState = GhostActionState.NONE;
        m_abilityUsed = false;
        m_aimingAbility = false;
        AbilityUnused();
    }

    protected void RotateTowardsAbilityDir()
    {
        // Rotate player visuals and then attack these squares
        Vector3 attackDir = AverageAimDirection();
        transform.rotation = Quaternion.LookRotation(attackDir);
    }


    #region PERFORM_ABILTY_REGION

    void PerformAttack()
    {
        SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.GHOST_ATTACK);
        m_animator.SetTrigger("Attack");
        ClearAttackVisuals();
        m_attackCooldownTimer = m_attackCooldown;

        RotateTowardsAbilityDir();

        TextEffectController.Instance.PlayEffectText(transform.position, TextEffectTypes.BOO, 0);
        List<PunkController> affectedPunks = GameMaster.Instance().GetPunksAtLocations(m_rotatedAffectedSquares);
        foreach (PunkController pc in affectedPunks)
            pc.OnEntityHit(m_baseAttackDamage, transform.position);
    }

    void PerformHide()
    {
        SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.GHOST_HIDE);
        m_ghostHideModel.SetActive(true);
        m_ghostNormalModel.SetActive(false);
        m_ghostController.m_OutofSight = true;
        m_hideCooldownTimer = m_hideCooldown; // Update the timer
        // PUT IT HERE HUGO
        PathRequestManager.Instance().SetNodeState(NodeState.GHOST_HIDE, transform);
        TextEffectController.Instance.PlayEffectText(transform.position, TextEffectTypes.HIDE, 0);
    }

    void PerformOverwatch()
    {
        SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.GHOST_OVERSPOOK);
        ClearAttackVisuals();
        m_overwatchCooldownTimer = m_overwatchCooldown;

        // Rotate player visuals
        Vector3 lookDir = AverageAimDirection();
        transform.rotation = Quaternion.LookRotation(lookDir);

        TextEffectController.Instance.PlayEffectText(transform.position, TextEffectTypes.OVERSPOOK, 0);
    }

    protected virtual void PerformSpecialAbility()
    {
        ClearAttackVisuals();
        m_specialCooldownTimer = m_specialCooldown;
    }

    protected Vector3 AverageAimDirection()
    {
        Vector3 attackDir = Vector3.zero;

        if (m_rotatedAffectedSquares.Count < 1)
            return attackDir;

        // Collect all together
        foreach (Vector3 targetPos in m_rotatedAffectedSquares)
            attackDir += (targetPos - transform.position);

        // Remove y-component, and normalize
        attackDir.y = 0;
        return attackDir.normalized;
    }

    #endregion

    #region CHOOSE_ABILITY_REGION

    public void ChooseAttack()
    {
        m_actionState = GhostActionState.BOO;
        m_currentAffectedSquares = m_attackSquares;
        StartAimingAbility();

        if (m_ghostController.m_aimModel)
        {
            m_ghostController.m_aimModel.transform.position = m_ghostController.TargetPoint();
            m_ghostController.m_aimModel.SetText("Boo");
        }
    }

    public void ChooseHide()
    {
        //Debug.Log("Ghost Hide");
        m_actionState = GhostActionState.HIDE;
        m_ghostController.EndMovement();
        ImmediateConfirmAbility();

        if (m_ghostController.m_aimModel)
        {
            m_ghostController.m_aimModel.transform.position = m_ghostController.TargetPoint();
            m_ghostController.m_aimModel.SetText("Hide");
        }
    }

    public void ChooseOverwatch()
    {
        m_actionState = GhostActionState.OVERSPOOK;
        m_currentAffectedSquares = m_overspookSquares;
        StartAimingAbility();

        if (m_ghostController.m_aimModel)
        {
            m_ghostController.m_aimModel.transform.position = m_ghostController.TargetPoint();
            m_ghostController.m_aimModel.SetText("Over Spook");
        }
    }

    public virtual void ChooseSpecial()
    {
        //Debug.Log("Ghost Special");
        m_actionState = GhostActionState.ABILITY;
    }

    #endregion
}
