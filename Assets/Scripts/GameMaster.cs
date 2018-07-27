using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public GameObject[] m_startGhostArray; // All ghosts that will be in the game
    public GameObject[] m_startPunkArray; // All punks at start of game
    public GhostHole[] m_startGhostHoles; // All starting ghost holes
    PlayerKeyboardInput m_KeyBoardInput;

    List<GhostHole> m_ghostHoleList; // List containing all living ghost relics
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

    public bool m_playGhostInSequence = true;
    public bool m_playersTurn;

    private List<Vector3> m_tempUnwalkable;

    public static GameMaster Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        m_ghostList = new List<GhostController>();
        m_punkList = new List<PunkController>();
        m_ghostHoleList = new List<GhostHole>();
        m_tempUnwalkable = new List<Vector3>();

        foreach (GameObject go in m_startGhostArray)
            m_ghostList.Add(go.GetComponent<GhostController>());

        foreach (GameObject go in m_startPunkArray)
            m_punkList.Add(go.GetComponent<PunkController>());

        foreach (GhostHole gh in m_startGhostHoles)
            m_ghostHoleList.Add(gh);
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
            StartGame();

        // TEST: Make all player ghosts move
        if (Input.GetKeyDown(KeyCode.Space))
            RunPlayersTurn();
    }


    void StartGame()
    {
        // TEMP: start game with players turn first
        PunkStartTurn();
    }

    #region END_GAME_FUNCTIONS

    void CheckEndGameState()
    {
        bool ghostsAlive = false;
        bool punksAlive = m_punkList.Count > 0;

        // Check if theres any ghost spawners left
        foreach (GhostHole gh in m_ghostHoleList)
        {
            if (gh.HoleIsAlive)
            {
                ghostsAlive = true;
                break;
            }
        }

        if (!punksAlive && !ghostsAlive)
            OnGameDraw();
        else if (!punksAlive && ghostsAlive)
            OnPlayerWon();
        else if (punksAlive && !ghostsAlive)
            OnPlayerLose();
        // else, game is stil running
    }

    void OnGameDraw()
    {
        // TODO
        Debug.Log("GAME IS DRAW");
    }

    void OnPlayerWon()
    {
        // TODO
        Debug.Log("PLAYER WON");
    }

    void OnPlayerLose()
    {
        // TODO
        Debug.Log("PLAYER LOST");
    }

    #endregion  

    #region GHOST_TURN_FUNCTIONS

    void GhostStartTurn()
    {
        foreach (PunkController pc in m_punkList)
        {
            m_tempUnwalkable.Add(pc.transform.position);
        }

        StartCoroutine(GhostSpawnAnimation());
    }

    IEnumerator GhostSpawnAnimation()
    {
        // Tell all ghost relics, start of turn
        foreach (GhostHole gh in m_ghostHoleList)
        {
            if (gh.GhostSpawnLogic())
            {
                //TODO: Make camera follow spawned ghost

                while (!gh.m_respawnAnimationDone)
                    yield return null;

                yield return new WaitForSeconds(0.4f); // Small delay after respawn to gauge dafuq happened
            }
        }

        // After spawn animation, time to start players turn
        StartPlayerChoice();

        yield return null;
    }

    void StartPlayerChoice()
    {
        PathRequestManager.Instance().GetComponent<NodeGrid>().ShowNodeGrid();

        // Turn ghost tiles unwalkable
        foreach (GhostController gc in m_ghostList)
        {
            m_tempUnwalkable.Add(gc.transform.position);
        }
        StartTurnUnWalkable();

        m_playersTurn = true;
        bool ghostFound = false;

        foreach (GhostController gc in m_ghostList)
        {
            gc.OnStartOfTurn(); // Tell all ghosts its start of turn
            // Select the first alive ghost in the list
            if (!ghostFound && gc.GhostIsAlive)
            {
                ghostFound = true;
                m_KeyBoardInput.UpdateSelectedGhost(gc.gameObject);
                gc.OnSelected();
                m_currentlySelectedGhost = gc;
            }
        }

        // Trigger the announcement that its the player's turn to appear on the UI
        m_AnnouncementBannerImage.GetComponent<AnnouncementBannerController>().Appear();
    }

    void RunPlayersTurn()
    {
        // Stop allowing player to select stuff
        TextEffectController.Instance.PlayTitleText("GHOST TIME");
        m_playersTurn = false;
        m_currentlySelectedGhost.OnDeselected();
        MousePicker.Instance().StopPicking();
        PathRequestManager.Instance().GetComponent<NodeGrid>().HideNodeGrid();
        StartCoroutine(GhostEndTurnAnimation());
    }

    IEnumerator GhostEndTurnAnimation()
    {
        CameraControl.Instance.SetOverviewMode();
        yield return new WaitForSeconds(0.5f); // Wait a bit before playing animation

        foreach (GhostController gc in m_ghostList)
        {
            if (m_playGhostInSequence)
            {
                CameraControl.Instance.SetFollowMode(gc.transform);
                yield return new WaitForSeconds(0.5f);
            }
            gc.OnEndOfTurn();

            if (m_playGhostInSequence)
            {
                while (gc.m_performing)
                    yield return null;
            }
        }

        // Wait for all ghosts to finish action, then start punks turn
        while (!m_playGhostInSequence)
        {
            bool ghostsStillMoving = false;
            foreach (GhostController gc in m_ghostList)
            {
                if (gc.m_performing)
                {
                    ghostsStillMoving = true;
                    break;
                }
            }

            if (!ghostsStillMoving)
                break;
            else
                yield return null;
        }

        EndTurnWalkable();
        PunkStartTurn();
    }

    #endregion

    #region PUNK_TURN_FUNCTIONS

    void PunkStartTurn()
    {
        TextEffectController.Instance.PlayTitleText("PUNK TIME");
        foreach(GhostController gc in m_ghostList)
        {
            if(gc.m_OutofSight || !gc.GhostIsAlive)
            {
                continue;
            }
            m_tempUnwalkable.Add(gc.transform.position);
        }
        StartTurnUnWalkable();

        StartCoroutine(PunkAnimation());
    }

    IEnumerator PunkAnimation()
    {
        foreach (PunkController pc in m_punkList)
        {
            Camera.main.GetComponent<CameraControl>().SetFollowMode(pc.transform);
            pc.DoTurn();

            while (pc.m_finishedMoving == false)
            {
                yield return null;
            }
        }

        EndTurnWalkable();

        GhostStartTurn();
        yield return null;
    }

    #endregion

    #region ON_ENTITY_DEATH

    public void RemoveGhostHole(GhostHole _holeWhichDied)
    {
        m_ghostHoleList.Remove(_holeWhichDied);
    }

    public void RemovePunk(PunkController _punkController)
    {
        m_punkList.Remove(_punkController);
    }

    #endregion

    public bool PunkHitOverwatch(PunkController _pc)
    {
        bool overwatchHit = false;
        foreach (GhostController gc in m_ghostList)
        {
            if (gc.IsOverwatchingPosition(_pc))
                overwatchHit = true;
        }

        return overwatchHit;
    }

    // Tells all the relevant systems that a new ghost has been selected
    public void UpdateSelectedGhost(GameObject newGhost)
    {
        // Don't update system if newly select ghost is the same
        if (newGhost == m_currentlySelectedGhost.gameObject
         || newGhost == null)
            return;

        m_currentlySelectedGhost.OnDeselected();
        m_KeyBoardInput.UpdateSelectedGhost(newGhost);
        newGhost.GetComponent<GhostController>().OnSelected();
        m_currentlySelectedGhost = newGhost.GetComponent<GhostController>();
    }

    #region CHECK_FOR_ENTITIES

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

    public List<GhostHole> GetGhostsAtLocations(List<Vector3> _positions)
    {
        List<GhostHole> holeList = new List<GhostHole>();

        // Check each punk against all locations, if they are within one, add to list
        foreach (GhostHole gh in m_ghostHoleList)
        {
            Node holeNode = PathRequestManager.Instance().NodeFromWorldPoint(gh.transform.position);

            foreach (Vector3 v3 in _positions)
            {
                Node pointNode = PathRequestManager.Instance().NodeFromWorldPoint(v3);
                if (holeNode == pointNode)
                {
                    holeList.Add(gh);
                    break;
                }
            }
        }

        return holeList;
    }

    #endregion  

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
            if(!gc.m_abilityUsed && gc != null)
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

    void EndTurnWalkable()
    {
        foreach (Vector3 v3 in m_tempUnwalkable)
        {
            PathRequestManager.Instance().TogglePositionWalkable(v3, true);
        }
        m_tempUnwalkable.Clear();
    }

    void StartTurnUnWalkable()
    {
        foreach(Vector3 v3 in m_tempUnwalkable)
        {
            PathRequestManager.Instance().TogglePositionWalkable(v3, false);
        }
    }

}