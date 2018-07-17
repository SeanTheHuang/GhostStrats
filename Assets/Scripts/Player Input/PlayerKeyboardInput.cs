using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardInput : MonoBehaviour {

    // Abilities can only be used if the character is selected
    public GameObject m_selectedGhost;
    GhostAbilityBehaviour m_GhostAbilityScript;

    public GameObject m_gameMaster;

    // The keycodes that let the player use a ghosts abilities
    public KeyCode m_attack;
    public KeyCode m_hide;
    public KeyCode m_overwatch;
    public KeyCode m_special;

    // The keycodes that let the player select each ghost
    public KeyCode m_ghostKey1;
    public KeyCode m_ghostKey2;
    public KeyCode m_ghostKey3;

    // The ghosts associated to the keyboard buttons 1,2,3
    public GameObject m_ghost1;
    public GameObject m_ghost2;
    public GameObject m_ghost3;

    void Start()
    {
        UpdateSelectedGhost(m_selectedGhost);
    }

    void Update()
    {
        // Keyboard input to select ghosts
        if (Input.GetKeyDown(m_ghostKey1))
            m_gameMaster.GetComponent<GameMaster>().UpdateSelectedGhost(m_ghost1);
        if (Input.GetKeyDown(m_ghostKey2))
            m_gameMaster.GetComponent<GameMaster>().UpdateSelectedGhost(m_ghost2);
        if (Input.GetKeyDown(m_ghostKey3))
            m_gameMaster.GetComponent<GameMaster>().UpdateSelectedGhost(m_ghost3);

        // Keyboard input to use ghost abilities
        if (m_selectedGhost && !m_GhostAbilityScript.m_abilityUsed) // Check to see if a ghost is selected and they haven't already used an ability
        {
            // Attack ability used if key pressed and cooldown at 0
            if (Input.GetKeyDown(m_attack) && m_GhostAbilityScript.m_attackCooldownTimer == 0)
            {
                m_GhostAbilityScript.Attack();
            }
            // Hide ability used if key pressed and cooldown at 0
            else if (Input.GetKeyDown(m_hide) && m_GhostAbilityScript.m_hideCooldownTimer == 0)
            {
                m_GhostAbilityScript.Hide();
            }
            // Overwatch ability used if key pressed and cooldown at 0
            else if (Input.GetKeyDown(m_overwatch) && m_GhostAbilityScript.m_overwatchCooldownTimer == 0)
            {
                m_GhostAbilityScript.Overwatch();
            }
            // Special ability used if key pressed and cooldown at 0
            else if (Input.GetKeyDown(m_special) && m_GhostAbilityScript.m_specialCooldownTimer == 0)
            {
                m_GhostAbilityScript.Special();
            }
        }
    }

    public void UpdateSelectedGhost(GameObject newGhost)
    {
        m_selectedGhost = newGhost;
        m_GhostAbilityScript = newGhost.GetComponent<GhostAbilityBehaviour>();
    }
}
