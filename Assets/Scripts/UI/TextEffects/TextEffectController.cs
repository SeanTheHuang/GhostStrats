using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TextEffectTypes
{
    GHOST_DAMAGE,
    PUNK_DAMAGE,
    STUNNED
}

[System.Serializable]
public struct TextEffectGroup
{
    public TextEffectTypes m_type;
    public EffectTextTemplate m_template;
}

// NOTE: This class must be put onto the canvas!!
public class TextEffectController : MonoBehaviour {

    [Header("Prefabs")]
    public Transform m_titleTextPrefab;
    public Transform m_effectsTextPrefab;

    [Header("Effect Texts")]
    public TextEffectGroup[] m_textTemplates;

    // Singleton shit
    public static TextEffectController Instance
    { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // TEST SHIT
        // TEST CODE
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayTitleText("HELLO ME SEAN");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayEffectText(Vector3.zero, (TextEffectTypes)Random.Range(0, 3), Random.Range(0, 5));
        }
    }

    EffectTextTemplate GetTemplateFromType(TextEffectTypes _type)
    {
        for (int i = 0; i < m_textTemplates.Length; i++)
        {
            if (m_textTemplates[i].m_type == _type)
                return m_textTemplates[i].m_template;
        }

        return null; // Reach here, couldn't find a template for this type
    }

    public void PlayTitleText(string _titleText)
    {
        Transform newTitle = Instantiate(m_titleTextPrefab, Vector3.zero, m_titleTextPrefab.rotation, transform);
        newTitle.GetComponent<RectTransform>().anchoredPosition3D = m_titleTextPrefab.position; // Properly set position here
        newTitle.GetComponent<TitleText>().InitializeAndStart(_titleText);
    }

    public void PlayEffectText(Vector3 _worldPosition, TextEffectTypes _type, int _strength)
    {
        // Change world position to screen position

        EffectTextTemplate template = GetTemplateFromType(_type);

        if (!template)
        {
            Debug.LogError("ERROR: No text effect template found or type " + _type.ToString());
            return;
        }

        // Get canvas space position
        RectTransform CanvasRect = GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(_worldPosition);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        // Create, set position, and play animation
        Transform effectText = Instantiate(m_effectsTextPrefab, Vector3.zero, Quaternion.identity, transform);
        effectText.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        effectText.GetComponent<EffectTextEffect>().InitializeAndPlay(template, _strength);
    }
}
