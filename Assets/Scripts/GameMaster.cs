using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public GameObject[] m_startGhostArray; // All ghosts that will be in the game
    public GameObject[] m_startPunkArray; // All punks at start of game
    PlayerKeyboardInput m_KeyBoardInput;

    List<GhostController> m_ghostList; // List containing all ghosts still alive
    List<PunkController> m_punkList; // List containing all punks still alive
    GhostController m_currentlySelectedGhost;
    // TODO: List of traps

    // The delegate events for when the game is played/paused
    public delegate void OnPlay();
    public delegate void OnPause();
    public event OnPlay m_onPlay;
    public event OnPause m_onPause;

    public GameObject m_AnnouncementBannerImage;
    public GameObject m_EndTurnPromptImage;

    static GameMaster instance;

    public static GameMaster Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        m_ghostList = new List<GhostController>();
        m_punkList = new List<PunkController>();

        foreach (GameObject go in m_startGhostArray)
            m_ghostList.Add(go.GetComponent<GhostController>());

        foreach (GameObject go in m_startPunkArray)
            m_punkList.Add(go.GetComponent<PunkController>());
    }

    void Start()
    {
        m_KeyBoardInput = PlayerKeyboardInput.Instance();

        // Set list of ghost for keyboard
        m_KeyBoardInput.SetGhostList(new List<GameObject>(m_startGhostArray));
    }

    private void Update()
    {
        // TEST: Just a button to start the game
        if (Input.GetKeyDown(KeyCode.P))
            StartPlayersTurn();

        // TEST: Make all player ghosts move
        if (Input.GetKeyDown(KeyCode.O))
            RunPlayersTurn();
    }

    void StartGame()
    {
        // TEMP: start game with players turn first
        StartPlayersTurn();
    }

    void StartPlayersTurn()
    {
        // Start game by first selecting first ghost in list
        m_KeyBoardInput.UpdateSelectedGhost(m_startGhostArray[0]);
        m_ghostList[0].OnSelected();
        m_currentlySelectedGhost = m_ghostList[0];

        // Tell all ghosts its start of turn
        foreach (GhostController gc in m_ghostList)
            gc.OnStartOfTurn();

        // Trigger the announcement that its the player's turn to appear on the UI
        m_AnnouncementBannerImage.GetComponent<AnnouncementBannerController>().Appear();
    }

    void RunPlayersTurn()
    {
        foreach (GhostController gc in m_ghostList)
        {
            gc.OnEndOfTurn();
        }

        // Stop allowing player to select stuff
        MousePicker.Instance().StopPicking();
    }

    // Tells all the relevant systems that a new ghost has been selected
    public void UpdateSelectedGhost(GameObject newGhost)
    {
        // Don't update system if newly select ghost is the same
        if (newGhost == m_currentlySelectedGhost.gameObject)
            return;

        m_currentlySelectedGhost.OnDeselected();
        m_KeyBoardInput.UpdateSelectedGhost(newGhost);
        newGhost.GetComponent<GhostController>().OnSelected();
        m_currentlySelectedGhost = newGhost.GetComponent<GhostController>();
    }

    public List<PunkController> GetPunksAtLocations(List<Vector3> _positions)
    {
        List<PunkController> punkList = new List<PunkController>();

        // Check each punk against all locations, if they are within one, add to list
        foreach (PunkController pc in m_punkList)
        {
            Node punksNode = PathRequestManager.Instance().NodeFromWorldPoint(pc.transform.position);
            foreach (Vector3 v3 in _positions)
            {
                Node nodeAtPosition = PathRequestManager.Instance().NodeFromWorldPoint(v3);
                if (punksNode == nodeAtPosition)
                {
                    punkList.Add(pc);
                    break;
                }
            }
        }

        return punkList;
    }

    public List<GhostController> GetGhostsAtLocations(List<Vector3> _positions, bool _checkFuturePosition = false)
    {
        List<GhostController> ghostList = new List<GhostController>();

        // Check each punk against all locations, if they are within one, add to list
        foreach (GhostController gc in m_ghostList)
        {
            Node ghostNode = null;
            if (_checkFuturePosition)
                ghostNode = PathRequestManager.Instance().NodeFromWorldPoint(gc.TargetPoint());
            else
                ghostNode = PathRequestManager.Instance().NodeFromWorldPoint(gc.transform.position);

            foreach (Vector3 v3 in _positions)
            {
                Node pointNode = PathRequestManager.Instance().NodeFromWorldPoint(v3);
                if (ghostNode == pointNode)
                {
                    ghostList.Add(gc);
                    break;
                }
            }
        }

        return ghostList;
    }

    public void Play()
    {
        // Stop allowing player to select stuff
        if (MousePicker.Instance().m_currentGhost != null)
            MousePicker.Instance().PausePicking();

        // Play the game
        m_onPlay();
    }

    public void Pause()
    {
        // Stop allowing player to select stuff
        if (MousePicker.Instance().m_currentGhost != null)
            MousePicker.Instance().ResumePicking();

        // Pause the game
        m_onPause();
    }

    // Cycle over all the ghosts. If all their actions have been used display a prompt to the player to end the turn
    public void CheckAllPlayerActionsUsed()
    {
        foreach (GhostController gc in m_ghostList)
        {
            if(!gc.m_abilityUsed)
                return;
        }

        m_EndTurnPromptImage.GetComponent<EndTurnPromptController>().Appear();
    }

    // If the end turn prompt is visible, make it disappear
    public void ResetEndTurnPrompt()
    {
        if (m_EndTurnPromptImage.GetComponent<EndTurnPromptController>().m_visible)
            m_EndTurnPromptImage.GetComponent<EndTurnPromptController>().Disappear();
    }
}