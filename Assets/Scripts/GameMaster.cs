using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

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

    // Tells all the relevant systems that a new ghost has been selected
    public void UpdateSelectedGhost(GameObject newGhost)
    {
        m_KeyBoardInput.GetComponent<PlayerKeyboardInput>().UpdateSelectedGhost(newGhost);
    }
}
