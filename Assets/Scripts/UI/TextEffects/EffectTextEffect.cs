using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Animation:
// 1. Spawn, slightly angled
// 2. Move up until time to fade
// 3. Fade away while rotating

public class EffectTextEffect : MonoBehaviour {

    [Header("Animation Variables")]

    [MinMaxRange(-1500, 1500)]
    public RangedFloat m_rotationSpeed;

    [MinMaxRange(-180, 180)]
    public RangedFloat m_startRotation;

    [Range(0, 1)]
    public float m_lifePercentBeforeStartFadingOut = 0.3f;

    public float m_floatSpeed = 15;
    public float m_lifeTime = 1;
    public float m_shrinkRate = 0.3f;

    TextMeshProUGUI m_TMPro;

    private void Awake()
    {
        m_TMPro = GetComponent<TextMeshProUGUI>();
    }

    public void InitializeAndPlay(EffectTextTemplate _template, int _strength = 0)
    {
        UpdateValues(_template, _strength);
        StartCoroutine(TextAnimation());
    }

    void UpdateValues(EffectTextTemplate _template, int _strength)
    {
        // Create and set text + material
        string value = _strength < 1 ? "" : "+" + _strength.ToString();
        string text = _template.m_iconText + " " + value + " " + _template.m_followUpText;
        GetComponent<TextMeshProUGUI>().text = text;
        GetComponent<TextMeshProUGUI>().fontSharedMaterial = _template.m_textMaterial;
    }

    IEnumerator TextAnimation()
    {
        float floatDuration = m_lifeTime * m_lifePercentBeforeStartFadingOut;
        float spinDuration = m_lifeTime - floatDuration;
        float startTime = Time.time;

        // 1. Spawn slightly angled
        float randVal = Random.Range(m_startRotation.minValue, m_startRotation.maxValue);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, randVal));

        // 2. Move up
        while (Time.time - startTime < floatDuration)
        {
            transform.position +=  Vector3.up * (m_floatSpeed * Time.deltaTime);
            yield return null;
        }

        // 3. Spin and fade whlie moving
        startTime = Time.time;
        float rotSpeed = Random.Range(m_rotationSpeed.minValue, m_rotationSpeed.maxValue);
        while (Time.time - startTime < spinDuration)
        {
            // Keep moving up and rotate
            transform.position += Vector3.up * (m_floatSpeed * Time.deltaTime);
            transform.Rotate(new Vector3(0, 0, rotSpeed * Time.deltaTime));

            // Scale
            float newScale = Mathf.Clamp(transform.localScale.x - (m_shrinkRate * Time.deltaTime), 0, 1000);
            transform.localScale = new Vector3(newScale, newScale, newScale);   

            m_TMPro.color = new Color(m_TMPro.color.r, m_TMPro.color.g, m_TMPro.color.b, m_TMPro.color.a - (Time.deltaTime / spinDuration));
            yield return null;
        }

        // Animation done, destroy this thing
        Destroy(gameObject);
        yield return null;
    }
}
