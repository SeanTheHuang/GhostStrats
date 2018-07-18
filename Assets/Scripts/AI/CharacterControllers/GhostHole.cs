using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHole : EntityBase
 {
    
    private void Awake()
    {
        m_maxMoves = 0;
        m_moveSpeed = 0;
    }

    public override void OnDeath()
    {
        //despawn
    }
    public override void OnSpawn()
    {
    }
    public override void OnEntityHit()
    {
        //lose health
    }
    public override void OnSelected()
    {
        return; // doesnt move
    }
    public override void ChooseAction()
    {
        //no actions
    }

}
