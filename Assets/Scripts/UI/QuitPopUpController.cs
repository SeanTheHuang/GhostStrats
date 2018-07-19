using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPopUpController : MonoBehaviour {

    public void QuitGame()
    {
        Application.Quit();
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
