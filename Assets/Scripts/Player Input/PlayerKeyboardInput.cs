using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardInput : MonoBehaviour {

    static PlayerKeyboardInput instance;
    const int s_numGhosts = 3;

    // Abilities can only be used if the character is selected
    public GameObject m_selectedGhost;
    GhostAbilityBehaviour m_GhostAbilityScript;
    GhostController m_GhostController;

    GameMaster m_gameMaster;
    HelpPopupController m_helpPopUp;

    // The keycodes that let the player use a ghosts abilities
    public KeyCode m_attack;
    public KeyCode m_hide;
    public KeyCode m_overwatch;
    public KeyCode m_special;

    // The keycodes that let the player select each ghost
    public KeyCode m_ghostKey1;
    public KeyCode m_ghostKey2;
    public KeyCode m_ghostKey3;
    
    // The keycode for opening the pause menu
    public KeyCode m_openPauseMenu;
    public GameObject m_pauseMenu;
    private bool m_gamePaused;

    // The ghosts associated to the keyboard buttons 1,2,3
    private List<GameObject> m_ghostList;

    public static PlayerKeyboardInput Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        m_ghostList = new List<GameObject>();
    }

    void Start()
    {
        m_gameMaster = GameMaster.Instance();
        m_helpPopUp = HelpPopupController.Instance();
        m_gameMaster.m_onPause += onPause;
        m_gameMaster.m_onPlay += onPlay;
    }
    
    void Update()
    {
        // Pause the game and open the pause menu
        if (Input.GetKeyDown(m_openPauseMenu))
        {
            if (m_gamePaused)
                m_gameMaster.Play();
            else
                m_gameMaster.Pause();
        }
        
        if (!m_gameMaster.m_playersTurn || m_gamePaused) // Don't update if no ghost is selected or the game is paused
            return;

        // Keyboard input to select ghosts, only select ghost if they exist
        if (Input.GetKeyDown(m_ghostKey1) && m_ghostList.Count > 0)
            m_gameMaster.UpdateSelectedGhost(m_ghostList[0]);

        else if (Input.GetKeyDown(m_ghostKey2) && m_ghostList.Count > 1)
            m_gameMaster.UpdateSelectedGhost(m_ghostList[1]);

        else if (Input.GetKeyDown(m_ghostKey3) && m_ghostList.Count > 2)
            m_gameMaster.UpdateSelectedGhost(m_ghostList[2]);

        // Keyboard input to use ghost abilities
        if (!m_GhostController.m_abilityUsed) // Check to see if the selected ghost has not already used an ability
        {
            // Attack ability used if key pressed and cooldown at 0
            if (Input.GetKeyDown(m_attack) && m_GhostAbilityScript.m_attackCooldownTimer == 0)
            {
                m_GhostAbilityScript.ChooseAttack();
            }
            // Hide ability used if key pressed and cooldown at 0
            else if (Input.GetKeyDown(m_hide) && m_GhostAbilityScript.m_hideCooldownTimer == 0)
            {
                m_GhostAbilityScript.ChooseHide();
            }
            // Overwatch ability used if key pressed and cooldown at 0
            else if (Input.GetKeyDown(m_overwatch) && m_GhostAbilityScript.m_overwatchCooldownTimer == 0)
            {
                m_GhostAbilityScript.ChooseOverwatch();
            }
            // Special ability used if key pressed and cooldown at 0
            else if (Input.GetKeyDown(m_special) && m_GhostAbilityScript.m_specialCooldownTimer == 0)
            {
                m_GhostAbilityScript.ChooseSpecial();
            }
        }
    }

    public void SetGhostList(List<GameObject> ghostList)
    {
        // Start with clear list
        m_ghostList.Clear();

        for (int i = 0; i < ghostList.Count; i++)
        {
            if (i >= s_numGhosts) // Only store up to max amount of supported ghosts
                break;

            m_ghostList.Add(ghostList[i]);
        }
    }

    public void UpdateSelectedGhost(GameObject newGhost)
    {
        m_selectedGhost = newGhost;
        m_GhostAbilityScript = newGhost.GetComponent<GhostAbilityBehaviour>();
        m_GhostController = newGhost.GetComponent<GhostController>();
        m_helpPopUp.UpdateGhostSelected(newGhost);
    }

    void onPlay()
    {
        m_pauseMenu.SetActive(false);
        m_gamePaused = false;
    }

    void onPause()
    {
        m_pauseMenu.SetActive(true);
        m_gamePaused = true;
    }
}
