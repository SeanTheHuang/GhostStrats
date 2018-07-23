using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollGhostAbility : GhostAbilityBehaviour {

    protected override void SetGhostType()
    {
        m_ghostType = GhostType.DOLLER;
    }

    public override void ChooseSpecial()
    {
        base.ChooseSpecial();
        ImmediateConfirmAbility();
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();

        // Drop a "doll" where you are,
        // TODO: visuals
        Debug.Log("Doll was dropped at position:" + transform.position.ToString());
        PathRequestManager.Instance().SetNodeState(NodeState.GHOST_TRAP, transform);
    }
}
