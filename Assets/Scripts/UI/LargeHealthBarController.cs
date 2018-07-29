using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LargeHealthBarController : MonoBehaviour
{
    private Color m_highHealth;
    private Color m_midHealth;
    private Color m_lowHealth;

    private RectTransform healthBarRect;
    private Image m_healthBarImage;

    private float m_healthBarMaxWidth; // The width of the health bar when at 100%
    private float m_healthBarCurrentWidth; // The current width of the healthbar

    private float m_maxHealth; // The max health of the ghost
    private float m_currentHealth; // The current health of the ghost
    private float m_currentHealthBar; // The current health the bar is set to
    private float m_healthToBarWidth; // The constant that converts the current health to bar length

    public void Initalize(float maxHealth, Color highHealth, Color midHealth, Color lowHealth)
    {
        m_highHealth = highHealth;
        m_midHealth = midHealth;
        m_lowHealth = lowHealth;

        m_maxHealth = maxHealth;
        m_currentHealth = m_maxHealth;
        m_currentHealthBar = m_maxHealth;

        m_healthBarMaxWidth = GetComponent<RectTransform>().rect.width;
        m_healthBarImage = GetComponent<Image>();
        healthBarRect = GetComponent<RectTransform>();
        m_healthToBarWidth = GetComponent<RectTransform>().rect.width / m_maxHealth;
    }

    // Update the ghosts health bar
    public void UpdateHealthBar(float newHealth)
    {
        m_currentHealth = newHealth;
        StartCoroutine(LerpHealthBar(1.5f));
        //StartCoroutine(LerpHealthBarColor(1.5f));
    }

    private IEnumerator LerpHealthBarColor(float time)
    {
        // The health bars are currently in the same colour zone. No need to lerp colour
        if ((m_currentHealth > m_maxHealth * 0.5f && m_currentHealthBar > m_maxHealth * 0.5f)
        || ((m_currentHealth > m_maxHealth * 0.25f && m_currentHealthBar > m_maxHealth * 0.25f) && (m_currentHealth <= m_maxHealth * 0.5f && m_currentHealthBar <= m_maxHealth * 0.5f))
        || (m_currentHealth <= m_maxHealth * 0.25f && m_currentHealthBar <= m_maxHealth * 0.25f))
            yield return null;

        Color newBarColor;
        Color oldBarColor = m_healthBarImage.color;
        if (m_currentHealth > m_maxHealth * 0.5f)
            newBarColor = m_highHealth;
        else if (m_currentHealth > m_maxHealth * 0.25f)
            newBarColor = m_midHealth;
        else
            newBarColor = m_lowHealth;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            Color lerpColor = Color.Lerp(oldBarColor, newBarColor, (elapsedTime / time));
            m_healthBarImage.color = lerpColor;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator LerpHealthBar(float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            float width = Mathf.Lerp(m_currentHealthBar, m_currentHealth, (elapsedTime / time)) / m_maxHealth;
            m_healthBarImage.fillAmount = width;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_currentHealthBar = m_currentHealth;
    }
}
