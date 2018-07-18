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

public abstract class EntityBase : MonoBehaviour {

    [Header("Base Stats")]
    [Range(1, 20)] public int m_maxHealth = 10;
    [Range(1, 10)] public int m_maxMoves = 5;
    [Range(1, 30)] public float m_moveSpeed = 5;

    protected float m_currentHealth;

    public abstract void OnDeath();
    public abstract void OnSpawn();
    public abstract void OnEntityHit();
    public abstract void OnSelected();
    public abstract void ChooseAction();

    public float GetCurrentHealth() { return m_currentHealth; }
}
