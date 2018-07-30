using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostUi : MonoBehaviour
{
    public GameObject m_UIPortrait;
    public GameObject m_UIAbilityBar;
    private AbilityBarController m_UIAbilityBarCntrl;
    private GhostPortraitController ghostPortraitController;

    public Sprite m_specialAbilityIconImage; // The Icon image for the ghosts special ability
    public Sprite m_ghostPortraitSprite;

    private Image m_portraitImage;

    private SpriteRenderer m_selectedCircle;

    private void Awake()
    {
        m_UIAbilityBarCntrl = m_UIAbilityBar.GetComponent<AbilityBarController>();
        ghostPortraitController = m_UIPortrait.GetComponent<GhostPortraitController>();
        m_portraitImage = m_UIPortrait.transform.Find("PortraitImage").GetComponent<Image>();
        m_selectedCircle = transform.Find("SelectedCircle").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ghostPortraitController.m_SpecialAbilityIconSprite = m_specialAbilityIconImage;
        m_portraitImage.sprite = m_ghostPortraitSprite;
    }

    public void AbilityUsed(GhostActionState actionState)
    {
        m_UIAbilityBarCntrl.AbilityUsed(actionState);
    }

    public void OnSelected(int attackCooldownTimer, int hideCooldownTimer, int overwatchCooldownTimer, int specialCooldownTimer, bool moveUsed, bool someMoveUsed, GhostActionState actionState)
    {
        ghostPortraitController.OnSelected();
        m_UIAbilityBarCntrl.OnSelected(attackCooldownTimer, hideCooldownTimer, overwatchCooldownTimer, specialCooldownTimer, moveUsed, someMoveUsed, actionState);
        m_selectedCircle.enabled = true;
    }

    public void OnDeselected()
    {
        ghostPortraitController.OnDeselected();
        m_selectedCircle.enabled = false;
    }

    public void MoveUsed(int numMovesLeft)
    {
        // Update the UI
        if (numMovesLeft == 0)
            m_UIAbilityBarCntrl.MoveUsed(true);
        else
            m_UIAbilityBarCntrl.MoveUsed(false);
    }

    public void updateHealthbar(int currentHealth)
    {
        ghostPortraitController.UpdateHealthBar(currentHealth);
    }

    public void OnGhostHardDeath()
    {
        ghostPortraitController.OnGhostHardDeath();
    }
}