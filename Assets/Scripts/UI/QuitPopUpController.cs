using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitPopUpController : MonoBehaviour {

    private GameMaster m_gameMaster;

    private void Start()
    {
        m_gameMaster = GameMaster.Instance();
    }

    public void QuitGame()
    {
        //Application.Quit();
        
        SceneManager.LoadScene("MenuScene");
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
        m_gameMaster.Play();
    }
}
