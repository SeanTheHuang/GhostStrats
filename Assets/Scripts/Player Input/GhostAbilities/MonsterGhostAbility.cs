using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGhostAbility : GhostAbilityBehaviour {

    [Range (0,10)]
    public int m_monsterDamage = 4;

    protected override void SetGhostType()
    {
        m_ghostType = GhostType.MONSTER;
    }

    public override void ChooseSpecial()
    {
        // Start picking direction to attack.
        base.ChooseSpecial();
        m_currentAffectedSquares = m_specialSkillSquares;
        StartAimingAbility();
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();
        RotateTowardsAbilityDir();

        // Check if any punks on tiles, affected tiles and damage them
        List<PunkController> punkList = GameMaster.Instance().GetPunksAtLocations(m_rotatedAffectedSquares);
        foreach (PunkController pc in punkList)
            pc.OnEntityHit(m_monsterDamage, transform.position);
    }
}
