using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostState
{
    IDLE,
    OVERSPOOK,
    MOVING,
    STUNNED,
    DEAD,
    TRANSFORMED
}

public class GhostController : EntityBase {

    private GhostState m_ghostState, m_oldGhostState;

    private void Awake()
    {
        Initilaize();   
    }
    private void Initilaize()
    {
        m_ghostState = GhostState.IDLE;
    }

    public override void OnDeath()
    {
    }
    public override void OnSpawn()
    {
    }
    public override void OnEntityHit()
    {
    }
    public override void Move()
    {
    }
    public override void ChooseAction()
    {
    }
}
