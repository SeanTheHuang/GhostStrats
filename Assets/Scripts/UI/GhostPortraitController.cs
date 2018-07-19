using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostPortraitController : MonoBehaviour {

    public Color m_highHealth;
    public Color m_midHealth;
    public Color m_lowHealth;

    private Vector3 m_originalPosition; // Used during 'camera shake' of the UI element
    public float m_shakeIntensity;

    public GameObject m_ghost;
    public GameObject m_healthBar;
    public GameObject m_largeHealthBar;
    public GameObject m_highlight;
    private LargeHealthBarController largeHealthBarController;

    private RectTransform healthBarRect;
    private Image m_HealthBarImage;

    private float m_healthBarMaxWidth; // The width of the health bar when at 100%
    private float m_healthBarCurrentWidth; // The current width of the healthbar

    private float m_maxHealth; // The max health of the ghost
    private float m_currentHealth; // The current health of the ghost
    private float m_currentHealthBar; // The current health the bar is set to
    private float m_healthToBarWidth; // The constant that converts the current health to bar length

    // Use this for initialization
    void Start() {
        m_maxHealth = m_ghost.GetComponent<GhostController>().m_maxHealth;
        m_currentHealth = m_maxHealth;
        m_currentHealthBar = m_maxHealth;

        m_healthBarMaxWidth = m_healthBar.GetComponent<RectTransform>().rect.width;
        m_HealthBarImage = m_healthBar.GetComponent<Image>();
        healthBarRect = m_healthBar.GetComponent<RectTransform>();
        m_healthToBarWidth = m_healthBar.GetComponent<RectTransform>().rect.width / m_maxHealth;

        largeHealthBarController = m_largeHealthBar.GetComponent<LargeHealthBarController>();
        largeHealthBarController.Initalize(m_maxHealth, m_highHealth, m_midHealth, m_lowHealth);

        m_currentHealth /= 2;
        m_originalPosition = transform.position;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            UpdateHealthBar();
        }
    }

    // Update the ghosts health bar
    public void UpdateHealthBar()
    {
        //m_currentHealth = m_ghost.GetComponent<GhostController>().GetCurrentHealth();
        StartCoroutine(LerpHealthBar(1.5f));
        StartCoroutine(LerpHealthBarColor(1.5f));
        StartCoroutine(ShakeUI(1.5f));

        largeHealthBarController.UpdateHealthBar(m_currentHealth);
    }

    public void OnDeselected()
    {
        m_highlight.GetComponent<Image>().enabled = false;
        m_largeHealthBar.GetComponent<Image>().enabled = false;
    }

    public void OnSelected()
    {
        m_highlight.GetComponent<Image>().enabled = true;
        m_largeHealthBar.GetComponent<Image>().enabled = true;
    }

    private IEnumerator LerpHealthBarColor(float time)
    {
        // The health bars are currently in the same colour zone. No need to lerp colour
        if ((m_currentHealth > m_maxHealth * 0.5f && m_currentHealthBar > m_maxHealth * 0.5f)
        || ((m_currentHealth > m_maxHealth * 0.25f && m_currentHealthBar > m_maxHealth * 0.25f) && (m_currentHealth <= m_maxHealth * 0.5f && m_currentHealthBar <= m_maxHealth * 0.5f))
        || (m_currentHealth <= m_maxHealth * 0.25f && m_currentHealthBar <= m_maxHealth * 0.25f))
            yield return null;

        Color newBarColor;
        Color oldBarColor = m_HealthBarImage.color;
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
            m_HealthBarImage.color = lerpColor;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
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

    private IEnumerator ShakeUI(float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            Vector3 randomPosition = new Vector3(Random.Range(0, m_shakeIntensity), Random.Range(0, m_shakeIntensity), 0);

            transform.position = m_originalPosition + randomPosition;

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = m_originalPosition;
    }
}
