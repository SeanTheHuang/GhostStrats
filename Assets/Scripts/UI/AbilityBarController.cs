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

    public GameObject m_moveUIHighlight;
    public GameObject m_undoUIHighlight;
    public GameObject m_attackUIHighlight;
    public GameObject m_hideUIHighlight;
    public GameObject m_overwatchUIHighlight;
    public GameObject m_specialUIHighlight;

    private Image m_moveUIHighlightImage;
    private Image m_undoUIHighlightImage;
    private Image m_attackUIHighlightImage;
    private Image m_hideUIHighlightImage;
    private Image m_overwatchUIHighlightImage;
    private Image m_specialUIHighlightImage;

    private void Start()
    {
        m_moveUIHighlightImage = m_moveUIHighlight.GetComponent<Image>();
        m_undoUIHighlightImage = m_undoUIHighlight.GetComponent<Image>();
        m_attackUIHighlightImage = m_attackUIHighlight.GetComponent<Image>();
        m_hideUIHighlightImage = m_hideUIHighlight.GetComponent<Image>();
        m_overwatchUIHighlightImage = m_overwatchUIHighlight.GetComponent<Image>();
        m_specialUIHighlightImage = m_specialUIHighlight.GetComponent<Image>();
    }

    void updateAbilityIcon(bool abilityUsed, int coolDownTimer, GameObject abilityUIObject, Image highlightImage)
    {
        if (coolDownTimer > 0)
        {
            EnableCoolDownUIEffects(abilityUIObject, coolDownTimer);
            highlightImage.enabled = false;
        }
        else
        {
            DisableCoolDownUIEffects(m_attackUIImage);
            highlightImage.enabled = !abilityUsed;
        }
    }

    public void OnSelected(int attackCooldownTimer, int hideCooldownTimer, int overwatchCooldownTimer, int specialCooldownTimer, bool moveUsed, bool someMoveUsed, bool abilityUsed)
    {
        // Reset Ability Highlights
        updateAbilityIcon(abilityUsed, attackCooldownTimer, m_attackUIImage, m_attackUIHighlightImage);
        updateAbilityIcon(abilityUsed, hideCooldownTimer, m_hideUIImage, m_hideUIHighlightImage);
        updateAbilityIcon(abilityUsed, overwatchCooldownTimer, m_overwatchUIImage, m_overwatchUIHighlightImage);
        updateAbilityIcon(abilityUsed, specialCooldownTimer, m_specialUIImage, m_specialUIHighlightImage);

        // Reset Move Highlight
        if(!moveUsed)
            m_moveUIHighlightImage.enabled = true;
        else
            m_moveUIHighlightImage.enabled = false;

        // Reset Undo Highlight
        if(someMoveUsed || !m_attackUIHighlightImage.enabled)
            m_undoUIHighlightImage.enabled = true;
        else
            m_undoUIHighlightImage.enabled = false;
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
            m_moveUIHighlightImage.enabled = false;
        m_undoUIHighlightImage.enabled = true;
    }

    // Called when the ghost has used their ability. Sets the highlight to turn off
    public void AbilityUsed()
    {
        m_attackUIHighlightImage.enabled = false;
        m_hideUIHighlightImage.enabled = false;
        m_overwatchUIHighlightImage.enabled = false;
        m_specialUIHighlightImage.enabled = false;
        m_undoUIHighlightImage.enabled = true;
    }

    public void ResetTurn()
    {
       m_attackUIHighlightImage.enabled = true;
       m_hideUIHighlightImage.enabled = true;
       m_overwatchUIHighlightImage.enabled = true;
       m_specialUIHighlightImage.enabled = true;

       m_moveUIHighlightImage.enabled = true;
       m_undoUIHighlightImage.enabled = false;
    }
}
