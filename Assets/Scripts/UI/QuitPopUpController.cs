using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopUpController : MonoBehaviour {

    private GameMaster m_gameMaster;

    private void Start()
    {
        m_gameMaster = GameMaster.Instance();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
        m_gameMaster.Play();
    }
}
