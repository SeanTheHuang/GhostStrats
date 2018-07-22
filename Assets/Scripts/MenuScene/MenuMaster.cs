using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMaster : MonoBehaviour {

    public GameObject m_popUpMenu;

    public KeyCode m_closePopup;

    private void Update()
    {
        if (!m_popUpMenu.activeSelf)
            return;

        if(Input.GetKeyDown(m_closePopup))
            ShowControlsPopup(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowControlsPopup(bool showMenu)
    {
        m_popUpMenu.SetActive(showMenu);
    }
}
