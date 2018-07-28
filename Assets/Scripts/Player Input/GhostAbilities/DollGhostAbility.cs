using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollGhostAbility : GhostAbilityBehaviour {

    public Transform m_dollPrefab;

    protected override void SetGhostType()
    {
        m_ghostType = GhostType.DOLLER;
    }

    public override void ChooseSpecial()
    {
        base.ChooseSpecial();
        ImmediateConfirmAbility();

        if (m_ghostController.m_aimModel)
        {
            m_ghostController.m_aimModel.transform.position = m_ghostController.TargetPoint();
            m_ghostController.m_aimModel.SetText("Drop Doll");
            m_ghostController.m_aimModel.m_locked = true;
        }
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();

        // Drop a "doll" where you are,
        // TODO: visuals
        Debug.Log("Doll was dropped at position:" + transform.position.ToString());
        Transform droppedDrop = Instantiate(m_dollPrefab, transform.position, m_dollPrefab.rotation);
        PathRequestManager.Instance().SetNodeState(NodeState.GHOST_TRAP, droppedDrop);
    }
}
