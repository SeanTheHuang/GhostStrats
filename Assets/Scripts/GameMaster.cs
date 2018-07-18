using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public GameObject[] m_ghostsArray; // All ghosts that will be in the game
    PlayerKeyboardInput m_KeyBoardInput;

    List<GhostController> m_ghostList; // List containing all ghosts still alive
    static GameMaster instance;

    public static GameMaster Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        m_ghostList = new List<GhostController>();

        foreach (GameObject go in m_ghostsArray)
            m_ghostList.Add(go.GetComponent<GhostController>());
    }

    void Start()
    {
        m_KeyBoardInput = PlayerKeyboardInput.Instance();
    }

    private void Update()
    {
        // TEST: Just a button to start the game
        if (Input.GetKeyDown(KeyCode.P))
            StartGame();
    }

    void StartGame()
    {
        // Set list of ghost for keyboard
        m_KeyBoardInput.SetGhostList(new List<GameObject>(m_ghostsArray));

        // TEMP: start game with players turn first
        StartPlayersTurn();
    }

    void StartPlayersTurn()
    {
        // Start game by first selecting first ghost in list
        m_KeyBoardInput.UpdateSelectedGhost(m_ghostsArray[0]);
        m_ghostsArray[0].GetComponent<GhostController>().SelectingWhereToMove();

        // Tell all ghosts its start of turn
        foreach (GhostController gc in m_ghostList)
            gc.OnStartOfTurn();
    }

    // Tells all the relevant systems that a new ghost has been selected
    public void UpdateSelectedGhost(GameObject newGhost)
    {
        m_KeyBoardInput.UpdateSelectedGhost(newGhost);
        newGhost.GetComponent<GhostController>().SelectingWhereToMove();
    }
}