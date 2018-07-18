using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostPortraitController : MonoBehaviour {

    public GameObject m_ghost;
    public GameObject m_healthBar;

    private RectTransform healthBarRect;

    private float m_healthBarMaxWidth; // The width of the health bar when at 100%
    private float m_healthBarCurrentWidth; // The current width of the healthbar
  
    private float m_maxHealth; // The max health of the ghost
    private float m_currentHealth; // The current health of the ghost
    private float m_currentHealthBar; // The current health the bar is set to
    private float m_healthToBarWidth; // The constant that converts the current health to bar length

    // Use this for initialization
    void Start () {
        m_healthBarMaxWidth = m_healthBar.GetComponent<RectTransform>().rect.width;

        m_maxHealth = m_ghost.GetComponent<GhostController>().m_maxHealth;
        m_currentHealth = m_maxHealth;
        m_currentHealthBar = m_maxHealth;

        healthBarRect = m_healthBar.GetComponent<RectTransform>();
        m_healthToBarWidth = m_healthBar.GetComponent<RectTransform>().rect.width / m_maxHealth;
    }

    // Update the ghosts health bar
    public void UpdateHealthBar()
    {
        //m_currentHealth = m_ghost.GetComponent<GhostController>().GetCurrentHealth();
        StartCoroutine(LerpHealthBar(3.0f));
    }

    private IEnumerator LerpHealthBar(float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            float width = Mathf.Lerp(m_currentHealthBar, m_currentHealth, (elapsedTime / time)) * m_healthToBarWidth;
            m_healthBar.transform.position = new Vector2(m_healthBar.transform.position.x - ((healthBarRect.rect.width - width) / 2), m_healthBar.transform.position.y);
            healthBarRect.sizeDelta = new Vector2(width, healthBarRect.rect.height);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
