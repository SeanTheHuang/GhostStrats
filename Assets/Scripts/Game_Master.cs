using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Master : MonoBehaviour {

    public GameObject m_KeyBoardInput;

    // Tells all the relevant systems that a new ghost has been selected
    public void UpdateSelectedGhost(GameObject newGhost)
    {
        m_KeyBoardInput.GetComponent<Player_Keyboard_Input>().UpdateSelectedGhost(newGhost);
    }
}
