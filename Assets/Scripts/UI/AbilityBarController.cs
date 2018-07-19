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

    public void OnSelected(int attackCooldownTimer, int hideCooldownTimer, int overwatchCooldownTimer, int specialCooldownTimer)
    {
        if (attackCooldownTimer > 0)
            EnableCoolDownUIEffects(m_attackUIImage, attackCooldownTimer);
        else
            DisableCoolDownUIEffects(m_attackUIImage);

        if (hideCooldownTimer > 0)
            EnableCoolDownUIEffects(m_hideUIImage, hideCooldownTimer);
        else
            DisableCoolDownUIEffects(m_hideUIImage);

        if (overwatchCooldownTimer > 0)
            EnableCoolDownUIEffects(m_overwatchUIImage, overwatchCooldownTimer);
        else
            DisableCoolDownUIEffects(m_overwatchUIImage);

        if (specialCooldownTimer > 0)
            EnableCoolDownUIEffects(m_specialUIImage, specialCooldownTimer);
        else
            DisableCoolDownUIEffects(m_specialUIImage);
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
}
