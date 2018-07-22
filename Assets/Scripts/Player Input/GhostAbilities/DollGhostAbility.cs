using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollGhostAbility : GhostAbilityBehaviour {

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();

        // Drop a "doll" where you are,
        // TODO: visuals

        PathRequestManager.Instance().SetNodeState(NodeState.GHOST_TRAP, transform);
    }
}
