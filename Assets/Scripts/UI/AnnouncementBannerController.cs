using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnnouncementBannerController : MonoBehaviour {

    public float m_fadeInTime; // The time it takes for the object to fade in
    public float m_moveOutDelay; // The delay in time it takes for the object to being fading out after it has fully faded in
    public float m_fadeOutTime; // The time it takes for the object to fade out
    public float m_moveSpeed; // The speed which the banner moves each frame when fading in/out
    private float m_startPosition;

    Image m_image;
    RectTransform m_rectTransform;
    private float m_centreScreenPosition;
    private TextMeshProUGUI m_text;

    private void Start()
    {
        m_image = GetComponent<Image>();
        m_rectTransform = GetComponent<RectTransform>();
        m_startPosition = m_rectTransform.position.y;
        m_text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Triggers the events for the banner to fade in and fade out again
    public void Appear()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        m_centreScreenPosition = (Screen.height / 2) + (Screen.height / 4);

        while (elapsedTime < m_fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float yPosition = Mathf.Lerp(m_rectTransform.position.y, m_centreScreenPosition, (elapsedTime / m_fadeOutTime));
            m_rectTransform.position = new Vector2(m_rectTransform.position.x, yPosition);

            float alpha = Mathf.Lerp(m_image.color.a, 1, (elapsedTime / m_fadeInTime));
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, alpha);
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);

            yield return new WaitForEndOfFrame();
        }

        m_rectTransform.position = new Vector2(m_rectTransform.position.x, m_centreScreenPosition);
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 1);
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 1);

        while (elapsedTime < (m_fadeInTime + m_moveOutDelay))
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;

        while (elapsedTime < m_fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float yPosition = Mathf.Lerp(m_rectTransform.position.y, m_startPosition, (elapsedTime / m_fadeOutTime));
            m_rectTransform.position = new Vector2(m_rectTransform.position.x, yPosition);
            float alpha = Mathf.Lerp(m_image.color.a, 0, (elapsedTime / m_fadeOutTime));
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, alpha);
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alpha);

            yield return new WaitForEndOfFrame();
        }

        m_rectTransform.position = new Vector2(m_rectTransform.position.x, m_startPosition);
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 0);
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 0);
    }
}