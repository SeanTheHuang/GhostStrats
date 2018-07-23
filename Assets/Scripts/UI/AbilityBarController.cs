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

    public GameObject m_attackUIAbilityUsedHighlight;
    public GameObject m_hideUIAbilityUsedHighlight;
    public GameObject m_overwatchUIAbilityUsedHighlight;
    public GameObject m_specialUIAbilityUsedHighlight;

    private Image m_moveUIHighlightImage;
    private Image m_undoUIHighlightImage;
    private Image m_attackUIHighlightImage;
    private Image m_hideUIHighlightImage;
    private Image m_overwatchUIHighlightImage;
    private Image m_specialUIHighlightImage;

    private Image m_attackUIAbilityUsedHighlightImage;
    private Image m_hideUIAbilityUsedHighlightImage;
    private Image m_overwatchUIAbilityUsedHighlightImage;
    private Image m_specialUIAbilityUsedHighlightImage;

    private void Start()
    {
        m_moveUIHighlightImage = m_moveUIHighlight.GetComponent<Image>();
        m_undoUIHighlightImage = m_undoUIHighlight.GetComponent<Image>();
        m_attackUIHighlightImage = m_attackUIHighlight.GetComponent<Image>();
        m_hideUIHighlightImage = m_hideUIHighlight.GetComponent<Image>();
        m_overwatchUIHighlightImage = m_overwatchUIHighlight.GetComponent<Image>();
        m_specialUIHighlightImage = m_specialUIHighlight.GetComponent<Image>();

        m_attackUIAbilityUsedHighlightImage = m_attackUIAbilityUsedHighlight.GetComponent<Image>();
        m_hideUIAbilityUsedHighlightImage = m_hideUIAbilityUsedHighlight.GetComponent<Image>();
        m_overwatchUIAbilityUsedHighlightImage = m_overwatchUIAbilityUsedHighlight.GetComponent<Image>();
        m_specialUIAbilityUsedHighlightImage = m_specialUIAbilityUsedHighlight.GetComponent<Image>();
    }

    void updateAbilityIcon(GhostActionState ghostActionState, int coolDownTimer, GameObject abilityUIObject, Image highlightImage)
    {
        if (coolDownTimer > 0)
        {
            EnableCoolDownUIEffects(abilityUIObject, coolDownTimer);
            highlightImage.enabled = false;
        }
        else
        {
            DisableCoolDownUIEffects(m_attackUIImage);
            if(ghostActionState != GhostActionState.NONE)
            {
                highlightImage.enabled = false;
                setAbilityHighlight(ghostActionState);
            }
            else
                highlightImage.enabled = true;
        }
    }

    // Sets the highlight on a ability that has been used by the ghost
    void setAbilityHighlight(GhostActionState actionState)
    {
        if (actionState == GhostActionState.BOO)
            m_attackUIAbilityUsedHighlightImage.enabled = true;
        else if (actionState == GhostActionState.HIDE)
            m_hideUIAbilityUsedHighlightImage.enabled = true;
        else if (actionState == GhostActionState.OVERSPOOK)
            m_overwatchUIAbilityUsedHighlightImage.enabled = true;
        else if (actionState == GhostActionState.ABILITY)
            m_specialUIAbilityUsedHighlightImage.enabled = true;
    }

    public void OnSelected(int attackCooldownTimer, int hideCooldownTimer, int overwatchCooldownTimer, int specialCooldownTimer, bool moveUsed, bool someMoveUsed, GhostActionState actionState)
    {
        // Reset ability used highlight
        m_attackUIAbilityUsedHighlightImage.enabled = false;
        m_hideUIAbilityUsedHighlightImage.enabled = false;
        m_overwatchUIAbilityUsedHighlightImage.enabled = false;
        m_specialUIAbilityUsedHighlightImage.enabled = false;

        // Reset Ability Highlights
        updateAbilityIcon(actionState, attackCooldownTimer, m_attackUIImage, m_attackUIHighlightImage);
        updateAbilityIcon(actionState, hideCooldownTimer, m_hideUIImage, m_hideUIHighlightImage);
        updateAbilityIcon(actionState, overwatchCooldownTimer, m_overwatchUIImage, m_overwatchUIHighlightImage);
        updateAbilityIcon(actionState, specialCooldownTimer, m_specialUIImage, m_specialUIHighlightImage);

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
    public void AbilityUsed(GhostActionState actionState)
    {
        // Stop the ability from being highlighted
        m_attackUIHighlightImage.enabled = false;
        m_hideUIHighlightImage.enabled = false;
        m_overwatchUIHighlightImage.enabled = false;
        m_specialUIHighlightImage.enabled = false;
        m_undoUIHighlightImage.enabled = true;

        // Apply the highlight on the ability that has been used
        setAbilityHighlight(actionState);
    }

    public void ResetTurn()
    {
       m_attackUIHighlightImage.enabled = true;
       m_hideUIHighlightImage.enabled = true;
       m_overwatchUIHighlightImage.enabled = true;
       m_specialUIHighlightImage.enabled = true;

       m_attackUIAbilityUsedHighlightImage.enabled = false;
       m_hideUIAbilityUsedHighlightImage.enabled = false;
       m_overwatchUIAbilityUsedHighlightImage.enabled = false;
       m_specialUIAbilityUsedHighlightImage.enabled = false;

       m_moveUIHighlightImage.enabled = true;
       m_undoUIHighlightImage.enabled = false;
    }
}
