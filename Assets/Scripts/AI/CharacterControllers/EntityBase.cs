using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStates
{
    IDLE,
    OVERSPOOK,
    CHOOSING_WHERE_TO_MOVE,
    MOVING,
    STUNNED,
    DEAD,
    TRANSFORMED
}

public enum PunkStates
{
    IDLE,
    MOVING,
    ATTACK,
    DEAD
}

public abstract class EntityBase : MonoBehaviour {

    [Header("Base Stats")]
    [Range(1, 20)] public int m_maxHealth = 10;
    [Range(1, 10)] public int m_maxMoves = 5;
    [Range(1, 30)] public float m_moveSpeed = 5;

    protected int m_currentHealth;

    public abstract void OnDeath();
    public abstract void OnSpawn();
    public abstract void OnEntityHit(int _damage, Vector3 _positionOfHitter);
    public abstract void OnSelected();
    public abstract void ChooseAction();

    public int GetCurrentHealth() { return m_currentHealth; }

    /// <summary>
    /// Adjusts damage based on attack direction.
    /// Uses current transform's facing direction, and attack direction
    /// 50% Damage - If attack from the front
    /// 100% Damage - If attack from side
    /// 200% Damage - If attack from back
    /// </summary>
    protected int GetDamageBaseOffDirection(int _baseDamage, Vector3 _attackDir)
    {
        _attackDir = _attackDir.normalized;

        float dotProduct = Vector3.Dot(_attackDir, transform.forward);

        if (dotProduct >= 0.998) // Attack from behind
            return 2 * _baseDamage;
        else if (dotProduct < 0) // Attack from the front
            return Mathf.RoundToInt((0.5f * _baseDamage));
        else
            return _baseDamage; // Attack from the side
    }
}
