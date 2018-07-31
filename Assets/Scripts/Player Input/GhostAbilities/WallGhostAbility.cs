using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGhostAbility : GhostAbilityBehaviour {

    public Transform m_wallModel;

    protected override void Initialize()
    {
        m_wallModel.SetParent(null); // Toss kid out
        ToggleWallMode(false);
        base.Initialize();
    }

    protected override void SetGhostType()
    {
        m_ghostType = GhostType.WALLER;
    }

    public override void ChooseSpecial()
    {
        base.ChooseSpecial();
        m_currentAffectedSquares = m_specialSkillSquares;
        StartAimingAbility(false);

        if (m_ghostController.m_aimModel)
        {
            m_ghostController.m_aimModel.transform.position = m_ghostController.TargetPoint();
            m_ghostController.m_aimModel.SetText("Wall");
        }
    }

    protected override void PerformSpecialAbility()
    {
        base.PerformSpecialAbility();
        SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.GHOST_WALL);
        transform.rotation = Quaternion.AngleAxis((float)m_aimingDirection, Vector3.up);

        StartCoroutine(TransformAnimation(true));

        // Block off current path
        foreach (Vector3 v3 in m_rotatedAffectedSquares)
            PathRequestManager.Instance().TogglePositionWalkable(v3, false);
    }

    public override void StartOfTurn()
    {
        // Clear up path where wall was
        if (m_actionState == GhostActionState.ABILITY)
        {
            // Clean up ability
            foreach (Vector3 v3 in m_rotatedAffectedSquares)
                PathRequestManager.Instance().TogglePositionWalkable(v3, true);

            SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.GHOST_WALL);
            StartCoroutine(TransformAnimation(false));
        }

        base.StartOfTurn();
    }

    public override void ConfirmDirection()
    {
        if (m_actionState == GhostActionState.ABILITY)
            m_ghostController.EndMovement();

        base.ConfirmDirection();
    }

    IEnumerator TransformAnimation(bool _becomeWall)
    {
        TextEffectController.Instance.GetComponent<EffectsSpawner>().SpawnPoofPrefab(transform.position);
        yield return new WaitForSeconds(0.3f);

        if (_becomeWall)
        {
            m_wallModel.position = transform.position;
            m_wallModel.rotation = transform.rotation;
        }
        ToggleWallMode(_becomeWall);
        yield return null;
    }

    void ToggleWallMode(bool _becomeWall)
    {
        // Shit way of doing stuff, turn stuff off, turn stuff on i want :4)
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        Renderer[] wallRend = m_wallModel.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in rend)
        {
            if (r.name == "SelectedCircle")
                continue;
            r.enabled = !_becomeWall;
        }

        foreach (Renderer r in wallRend)
            r.enabled = _becomeWall;
    
        BoxCollider bodyBox = GetComponent<BoxCollider>();
        if (bodyBox)
            bodyBox.enabled = !_becomeWall;

        BoxCollider wallBox = m_wallModel.GetComponent<BoxCollider>();
        if (wallBox)
            wallBox.enabled = _becomeWall;
    }
}
