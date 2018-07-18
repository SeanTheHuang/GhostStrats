using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public GameObject[] m_ghostsArray; 
    PlayerKeyboardInput m_KeyBoardInput;

    static GameMaster instance;

    public static GameMaster Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
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

        // Start game by first selecting first ghost in list
        m_KeyBoardInput.UpdateSelectedGhost(m_ghostsArray[0]);
        m_ghostsArray[0].GetComponent<GhostController>().SelectingWhereToMove();
    }

    // Tells all the relevant systems that a new ghost has been selected
    public void UpdateSelectedGhost(GameObject newGhost)
    {
        m_KeyBoardInput.UpdateSelectedGhost(newGhost);
    }
}