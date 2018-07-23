using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreamGhostAbility : GhostAbilityBehaviour {
    [Range(0, 10)]
    public int m_screamDamage = 1;

    protected override void SetGhostType()
    {
        m_ghostType = GhostType.SCREAMER;
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();
        // TODO: Get all punks and deal X damage to them

        // TODO: Notify all punks of scream
    }

    public override void ChooseSpecial()
    {
        base.ChooseSpecial();
        ImmediateConfirmAbility();
    }
}
