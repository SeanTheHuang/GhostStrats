using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingTitle : MonoBehaviour {

    [Header("Animation Variables")]
    public float m_fadeInTime = 0.3f;
    public float m_fadeOutTime = 0.3f;
    public float m_waitPeriod = 1;
    public Vector3 m_moveOffset; // How much the text will move over animation period
    public Vector3 m_driftVelocity; // How text will move while being revealed
    bool m_animationStarted = false;

    Image[] m_images;
    
    private void Awake()
    {
        m_images = GetComponentsInChildren<Image>();

        foreach (Image img in m_images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
    }

    // Starting point
    public void StartAnimation()
    {
        m_animationStarted = true;
        StartCoroutine(TextAnimation());
    }

    IEnumerator TextAnimation()
    {
        Vector3 startPoint = transform.position - m_moveOffset;
        Vector3 midPoint = transform.position;

        // Fade in and move to midPoint
        while (m_images[0].color.a < 1)
        {
            float newAlpha = m_images[0].color.a + (Time.deltaTime / m_fadeInTime);
            foreach (Image img in m_images)
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);

            transform.position = Vector3.Lerp(startPoint, midPoint, newAlpha);
            yield return null;
        }

        // Pause in the spawn position
        float startWaitTime = Time.time;
        foreach (Image img in m_images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        transform.position = midPoint;

        while (Time.time - startWaitTime < m_waitPeriod)
        {
            transform.position += m_driftVelocity * Time.deltaTime;
            yield return null;
        }

        // Fade out and over away from mid point
        midPoint = transform.position;
        Vector3 endPoint = transform.position + m_moveOffset;
        while (m_images[0].color.a > 0)
        {
            float newAlpha = m_images[0].color.a - (Time.deltaTime / m_fadeInTime);
            foreach (Image img in m_images)
                img.color = new Color(img.color.r, img.color.g, img.color.b, newAlpha);
            transform.position = Vector3.Lerp(endPoint, midPoint, newAlpha);
            yield return null;
        }

        // Destroy object next frame after animation is done
        Destroy(gameObject);
        yield return null;
    }
}
