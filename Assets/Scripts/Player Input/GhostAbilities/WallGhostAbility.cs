using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGhostAbility : GhostAbilityBehaviour {

    public override void ChooseSpecial()
    {
        base.ChooseSpecial();
        m_currentAffectedSquares = m_specialSkillSquares;
        StartAimingAbility();
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();

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
