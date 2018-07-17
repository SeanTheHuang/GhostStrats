using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class GhostController : EntityBase {

    private CharacterStates m_ghostState, m_oldGhostState;

    private int m_numMovesLeft;

    private void Awake()
    {
        Initilaize();   
    }
    private void Initilaize()
    {
        m_ghostState = m_oldGhostState = CharacterStates.IDLE;
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
        // This Ghost's turn to move!, get where the player wants to move
        m_ghostState = CharacterStates.CHOOSING_WHERE_TO_MOVE;
        m_numMovesLeft = m_maxMoves;
    }
    public override void ChooseAction()
    {
    }
}
