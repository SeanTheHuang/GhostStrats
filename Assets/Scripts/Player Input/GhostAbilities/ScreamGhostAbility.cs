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
        SoundEffectsPlayer.Instance.PlaySound("Select");
        ImmediateConfirmAbility();

        if (m_ghostController.m_aimModel)
        {
            m_ghostController.m_aimModel.transform.position = m_ghostController.TargetPoint();
            m_ghostController.m_aimModel.SetText("Scream");
            m_ghostController.m_aimModel.m_locked = true;
        }
    }
}
