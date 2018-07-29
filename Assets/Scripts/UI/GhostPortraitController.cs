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
    public GameObject m_portrait;
    public GameObject m_specialAbilityIcon;
    private LargeHealthBarController largeHealthBarController;

    private Image m_healthBarImage;

    private Image m_specialAbilityImage;
    private Image m_largeHealthBarImage;
    public Image m_GhostFaceImage;
    private Image m_highlightImage;
    private Image m_portraitImage;

    private float m_healthBarMaxWidth; // The width of the health bar when at 100%
    private float m_healthBarCurrentWidth; // The current width of the healthbar

    private float m_maxHealth; // The max health of the ghost
    private float m_currentHealth; // The current health of the ghost
    private float m_currentHealthBar; // The current health the bar is set to
    private float m_healthToBarWidth; // The constant that converts the current health to bar length

    public Sprite m_defaultGhostPortrait; // The alive portrait for the ghost
    public Sprite m_respawnGhostPortrait; // The portrait when the ghost is dead and returning to their crystal
    public Sprite m_deadGhostPortrait; // The portrait when the ghosts crystal is destroyed

    public Sprite m_SpecialAbilityIconSprite; // The icon for the ghosts special ability

    private GameMaster m_gameMaster;

    // Use this for initialization
    void Start() {
        // Set as in active if the ghost does not exist
        if (m_ghost == null)
        {
            gameObject.SetActive(false);
            return;
        }

        m_maxHealth = m_ghost.GetComponent<GhostController>().m_maxHealth;
        m_currentHealth = m_maxHealth;
        m_currentHealthBar = m_maxHealth;

        m_healthBarMaxWidth = m_healthBar.GetComponent<RectTransform>().rect.width;
        m_healthBarImage = m_healthBar.GetComponent<Image>();
        m_healthToBarWidth = m_healthBar.GetComponent<RectTransform>().rect.width / m_maxHealth;

        largeHealthBarController = m_largeHealthBar.GetComponent<LargeHealthBarController>();
        largeHealthBarController.Initalize(m_maxHealth, m_highHealth, m_midHealth, m_lowHealth);

        m_portraitImage = m_portrait.GetComponent<Image>();
        m_highlightImage = m_highlight.GetComponent<Image>();
        m_specialAbilityImage = m_specialAbilityIcon.GetComponent<Image>();
        m_largeHealthBarImage = m_largeHealthBar.GetComponent<Image>();

        m_originalPosition = transform.position;

        m_gameMaster = GameMaster.Instance();
    }

    // Update the ghosts health bar
    public void UpdateHealthBar(int newHealth)
    {
        m_currentHealth = newHealth;
        StartCoroutine(LerpHealthBar(1.5f));
        //StartCoroutine(LerpHealthBarColor(1.5f));
        StartCoroutine(ShakeUI(0.5f));

        largeHealthBarController.UpdateHealthBar(m_currentHealth);
    }

    public void OnDeselected()
    {
        m_highlightImage.enabled = false;
        m_largeHealthBarImage.enabled = false;
        m_GhostFaceImage.enabled = false;
    }

    public void OnSelected()
    {
        m_highlightImage.enabled = true;
        m_largeHealthBarImage.enabled = true;
        m_GhostFaceImage.enabled = true;
        m_specialAbilityImage.sprite = m_SpecialAbilityIconSprite;
    }

    // The Ghost hits 0 hp but has not died
    public void OnGhostSoftDeath()
    {
        m_portraitImage.sprite = m_respawnGhostPortrait;
    }

    // The ghosts crystal has been destroyed, ghost fully died
    public void OnGhostHardDeath()
    {
        m_portraitImage.sprite = m_deadGhostPortrait;
    }

    // The ghost has respawned again
    public void OnGhostRespawn()
    {
        m_portraitImage.sprite = m_defaultGhostPortrait;
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

    private IEnumerator ShakeUI(float time)
    {
        m_originalPosition = transform.position;
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

    public void SelectGhostWithMouseClick()
    {
        if(m_gameMaster.m_playersTurn)
            m_gameMaster.UpdateSelectedGhost(m_ghost);
    }
}
