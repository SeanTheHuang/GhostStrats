using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAbilityBehaviour : MonoBehaviour
{
    // Abilities can only be used if the character has not used this one this turn
    public bool m_abilityUsed;

    // The number of turns the ghost must wait before they can use the ability again.
    public int m_attackCooldown;
    public int m_hideCooldown;
    public int m_overwatchCooldown;
    public int m_specialCooldown;

    // The current number of turns the ghost must wait before they cna use the ability again.
    public int m_attackCooldownTimer;
    public int m_hideCooldownTimer;
    public int m_overwatchCooldownTimer;
    public int m_specialCooldownTimer;

    public void Attack()
    {
        Debug.Log("Ghost Attack");
        m_attackCooldownTimer = m_attackCooldown; // Update the timer
        m_abilityUsed = true;
    }

    public void Hide()
    {
        Debug.Log("Ghost Hide");
        m_hideCooldownTimer = m_hideCooldown; // Update the timer
        m_abilityUsed = true;
    }

    public void Overwatch()
    {
        Debug.Log("Ghost Overwatch");
        m_overwatchCooldownTimer = m_overwatchCooldown; // Update the timer
        m_abilityUsed = true;
    }

    public void Special()
    {
        Debug.Log("Ghost Special");
        m_specialCooldown = m_specialCooldownTimer; // Update the timer
        m_abilityUsed = true;
    }
}
