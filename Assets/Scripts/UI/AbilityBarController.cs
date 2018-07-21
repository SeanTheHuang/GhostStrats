using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityBarController : MonoBehaviour {

    public GameObject m_attackUIImage;
    public GameObject m_hideUIImage;
    public GameObject m_overwatchUIImage;
    public GameObject m_specialUIImage;

    public GameObject m_moveUIHighlightImage;
    public GameObject m_undoUIHighlightImage;
    public GameObject m_attackUIHighlightImage;
    public GameObject m_hideUIHighlightImage;
    public GameObject m_overwatchUIHighlightImage;
    public GameObject m_specialUIHighlightImage;

    void updateAbilityIcon(bool abilityUsed, int coolDownTimer, GameObject abilityUIObject, GameObject highlightImage)
    {
        if (coolDownTimer > 0)
        {
            EnableCoolDownUIEffects(abilityUIObject, coolDownTimer);
            highlightImage.GetComponent<Image>().enabled = false;
        }
        else
        {
            DisableCoolDownUIEffects(m_attackUIImage);
            highlightImage.GetComponent<Image>().enabled = !abilityUsed;
        }
    }

    public void OnSelected(int attackCooldownTimer, int hideCooldownTimer, int overwatchCooldownTimer, int specialCooldownTimer, bool moveUsed, bool abilityUsed)
    {
        updateAbilityIcon(abilityUsed, attackCooldownTimer, m_attackUIImage, m_attackUIHighlightImage);
        updateAbilityIcon(abilityUsed, hideCooldownTimer, m_hideUIImage, m_hideUIHighlightImage);
        updateAbilityIcon(abilityUsed, overwatchCooldownTimer, m_overwatchUIImage, m_overwatchUIHighlightImage);
        updateAbilityIcon(abilityUsed, specialCooldownTimer, m_specialUIImage, m_specialUIHighlightImage);

        if(!moveUsed)
        {
            m_moveUIHighlightImage.GetComponent<Image>().enabled = true;
        }
    }         
    
    public void EnableCoolDownUIEffects(GameObject uIAbility, int coolDownNumber)
    {
        uIAbility.transform.GetChild(0).GetComponent<Image>().enabled = true;
        uIAbility.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(coolDownNumber.ToString());
    }

    void DisableCoolDownUIEffects(GameObject uIAbility)
    {
        uIAbility.transform.GetChild(0).GetComponent<Image>().enabled = false;
        uIAbility.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("");
    }

    // Called when the ghost has used their move. Sets the highlight to turn off
    public void MoveUsed(bool allMovementUsed)
    {
        if(allMovementUsed)
            m_moveUIHighlightImage.GetComponent<Image>().enabled = false;
        m_undoUIHighlightImage.GetComponent<Image>().enabled = true;
    }

    // Called when the ghost has used their ability. Sets the highlight to turn off
    public void AbilityUsed()
    {
        m_attackUIHighlightImage.GetComponent<Image>().enabled = false;
        m_hideUIHighlightImage.GetComponent<Image>().enabled = false;
        m_overwatchUIHighlightImage.GetComponent<Image>().enabled = false;
        m_specialUIHighlightImage.GetComponent<Image>().enabled = false;
        m_undoUIHighlightImage.GetComponent<Image>().enabled = true;
    }

    public void ResetTurn()
    {
       m_attackUIHighlightImage.GetComponent<Image>().enabled = true;
       m_hideUIHighlightImage.GetComponent<Image>().enabled = true;
       m_overwatchUIHighlightImage.GetComponent<Image>().enabled = true;
       m_specialUIHighlightImage.GetComponent<Image>().enabled = true;

        m_moveUIHighlightImage.GetComponent<Image>().enabled = true;
        m_undoUIHighlightImage.GetComponent<Image>().enabled = false;
    }
}
