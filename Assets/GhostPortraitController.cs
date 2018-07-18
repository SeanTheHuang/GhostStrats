using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostPortraitController : MonoBehaviour {

    public GameObject m_ghost;
    public GameObject m_healthBar;

    float m_healthBarMaxWidth; // The width of the health bar when at 100%
    float m_healthBarCurrentWidth; // The current width of the healthbar

    float m_maxHealth; // The max health of the ghost
    float m_currentHealth; // The max health of the ghost

    // Use this for initialization
    void Start () {
        m_healthBarMaxWidth = m_healthBar.GetComponent<RectTransform>().rect.width;
        m_maxHealth = m_ghost.GetComponent<GhostController>().m_maxHealth;
        m_currentHealth = m_maxHealth;
    }


    // Update the ghosts health bar
    public void UpdateHealthBar()
    {
        m_currentHealth = m_ghost.GetComponent<GhostController>().GetCurrentHealth();
    }
}
