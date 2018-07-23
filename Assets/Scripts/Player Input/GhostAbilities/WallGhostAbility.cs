using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGhostAbility : GhostAbilityBehaviour {

    protected override void SetGhostType()
    {
        m_ghostType = GhostType.WALLER;
    }

    public override void ChooseSpecial()
    {
        base.ChooseSpecial();
        m_currentAffectedSquares = m_specialSkillSquares;
        StartAimingAbility();
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();

        // Apply shitty direction correction
        switch (m_aimingDirection)
        {
            case AimingDirection.North:
                transform.rotation = Quaternion.LookRotation(Vector3.forward);
                break;
            case AimingDirection.East:
                transform.rotation = Quaternion.LookRotation(Vector3.right);
                break;
            case AimingDirection.South:
                transform.rotation = Quaternion.LookRotation(Vector3.back);
                break;
            case AimingDirection.West:
                transform.rotation = Quaternion.LookRotation(Vector3.left);
                break;
            default:
                break;

        }

        // TODO: Turn into ghost wall

        // Block off current path
        foreach (Vector3 v3 in m_rotatedAffectedSquares)
            PathRequestManager.Instance().TogglePositionWalkable(v3, false);
    }

    public override void StartOfTurn()
    {
        base.StartOfTurn();

        // TODO: Turn back to normal ghost

        // Clear up path where wall was
        foreach (Vector3 v3 in m_rotatedAffectedSquares)
            PathRequestManager.Instance().TogglePositionWalkable(v3, true);
    }
}
