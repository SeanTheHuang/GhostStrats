﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GhostHole : EntityBase
{
    [Range(1, 10)]
    public int m_baseReviveTime = 2; // Number of turns to revive
    public int m_currentReviveTime
    { get; private set; }

    public bool m_respawnAnimationDone;

    public GhostController m_linkedGhost;
    public Transform m_targetPosition;
    bool m_ghostIsAlive;

    public GameObject m_damagedParticleEffect;

    TextMeshPro m_respawnText;
    public bool HoleIsAlive
    {
        get { return m_currentHealth > 0; }
    }

    private void Awake()
    {
        m_maxMoves = 0;
        m_moveSpeed = 0;
        m_ghostIsAlive = false;
        m_currentReviveTime = 0;
        m_currentHealth = m_maxHealth;
        m_respawnText = transform.Find("RespawnText").GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        if (m_linkedGhost)
            m_linkedGhost.HideGhost(); // Start ghost in off mode
    }

    public override void OnDeath()
    {
        // TODO: Gamemaster should be notified a ghost spawner has died

        
    }
    public override void OnSpawn()
    {
    }

    public bool GhostSpawnLogic()
    {
        if (m_ghostIsAlive) // Don't spawn ghost if they are already alive
            return false;

        m_currentReviveTime -= 1;
        UpdateRespawnText();

        if (m_currentReviveTime > 0) // Still waiting to revive
            return false;

        // Spawn ghost and make them move to target position
        // Create poof particles
        TextEffectController.Instance.GetComponent<EffectsSpawner>().SpawnRespawnPoofPrefab(transform.position);
        m_ghostIsAlive = true;
        m_linkedGhost.ShowGhost();
        m_respawnAnimationDone = false;
        m_linkedGhost.MoveToPositionImmediate(m_targetPosition.position);
        m_linkedGhost.resetUIHealth();
        CameraControl.Instance.SetFollowMode(m_linkedGhost.transform);

        return true;
    }

    public void OnGhostDeath()
    {
        m_ghostIsAlive = false;
        m_currentReviveTime = m_baseReviveTime+1;
        // Create poof particles
        TextEffectController.Instance.GetComponent<EffectsSpawner>().SpawnRespawnPoofPrefab(m_linkedGhost.transform.position);
        m_linkedGhost.transform.position = transform.position;
        m_linkedGhost.transform.rotation = transform.rotation;
        m_linkedGhost.HideGhost();
        UpdateRespawnText();
    }

    public override void OnEntityHit(int _damage, Vector3 _positionOfHitter)
    {
        //lose health
        m_currentHealth -= _damage;
        Debug.Log(transform.name + " has been hit for " + _damage.ToString() + " damage.");
        SoundEffectsPlayer.Instance.PlaySound(SoundCatagory.RELIC_HURT);
        TextEffectController.Instance.PlayEffectText(transform.position, TextEffectTypes.PUNK_DAMAGE, _damage);

        Instantiate(m_damagedParticleEffect, transform.position, m_damagedParticleEffect.transform.rotation);
        //TODO: death actions
        if (m_currentHealth < 1)
        {
            m_linkedGhost.OnHardDeath();
            // Create poof particles
            TextEffectController.Instance.GetComponent<EffectsSpawner>().SpawnRespawnPoofPrefab(m_linkedGhost.transform.position);
            GameMaster.Instance().RemoveGhostHole(this);
            Destroy(gameObject);
            if (m_linkedGhost)
                m_linkedGhost.HideGhost(true);
        }
    }
    public override void OnSelected()
    {
        return; // doesnt move
    }
    public override void ChooseAction()
    {
        //no actions
    }

    private void UpdateRespawnText()
    {
        if(m_currentReviveTime > 0)
            m_respawnText.SetText(m_currentReviveTime.ToString());
        else
            m_respawnText.SetText("");
    }
}
