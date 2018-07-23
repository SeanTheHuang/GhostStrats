using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleText : MonoBehaviour {

    [Header("Animation Variables")]
    public float m_fadeInTime = 0.3f;
    public float m_fadeOutTime = 0.3f;
    public float m_waitPeriod = 1;
    public Vector3 m_moveOffset; // How much the text will move over animation period
    public Vector3 m_driftVelocity; // How text will move while being revealed

    TextMeshProUGUI m_textMesh;
    bool m_animationStarted = false;

    private void Awake()
    {
        m_textMesh = GetComponent<TextMeshProUGUI>();
        m_textMesh.color = new Color(m_textMesh.color.r, m_textMesh.color.g, m_textMesh.color.b, 0);
    }

    // Starting point
    public void InitializeAndStart(string _text = "")
    {
        if (m_animationStarted)
            return;

        if (_text != "")
            m_textMesh.text = _text;

        m_animationStarted = true;
        StartCoroutine(TextAnimation());
    }

    IEnumerator TextAnimation()
    {
        Vector3 startPoint = transform.position - m_moveOffset;
        Vector3 midPoint = transform.position;

        // Fade in and move to midPoint
        while (m_textMesh.color.a < 1)
        {
            float newAlpha = m_textMesh.color.a + (Time.deltaTime / m_fadeInTime);
            m_textMesh.color = new Color(m_textMesh.color.r, m_textMesh.color.g, m_textMesh.color.b, newAlpha);
            transform.position = Vector3.Lerp(startPoint, midPoint, newAlpha);
            yield return null;
        }

        // Pause in the spawn position
        float startWaitTime = Time.time;
        m_textMesh.color = new Color(m_textMesh.color.r, m_textMesh.color.g, m_textMesh.color.b, 1);
        transform.position = midPoint;

        while (Time.time - startWaitTime < m_waitPeriod)
        {
            transform.position += m_driftVelocity * Time.deltaTime;
            yield return null;
        }

        // Fade out and over away from mid point
        midPoint = transform.position;
        Vector3 endPoint = transform.position + m_moveOffset;
        while (m_textMesh.color.a > 0)
        {
            float newAlpha = m_textMesh.color.a - (Time.deltaTime / m_fadeInTime);
            m_textMesh.color = new Color(m_textMesh.color.r, m_textMesh.color.g, m_textMesh.color.b, newAlpha);
            transform.position = Vector3.Lerp(endPoint, midPoint, newAlpha);
            yield return null;
        }

        // Destroy object next frame after animation is done
        Destroy(gameObject);
        yield return null;
    }
}
