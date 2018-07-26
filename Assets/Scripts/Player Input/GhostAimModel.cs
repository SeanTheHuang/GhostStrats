using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[SelectionBase]
public class GhostAimModel : MonoBehaviour {

    public TextMeshPro m_text;
    MeshRenderer[] m_renderers;
    Vector3 m_textLocalPosition;
    public bool m_locked;

    private void Awake()
    {
        m_renderers = GetComponentsInChildren<MeshRenderer>();

        // If text exists, throw them out of parent
        if (m_text)
        {
            m_textLocalPosition = m_text.transform.localPosition; // Remember to store position for later
            m_text.transform.SetParent(null);
        }
    }

    private void Start()
    {
        HideAimModel();
    }

    public void ResetAimModel()
    {
        m_locked = false;

        if (m_text)
            m_text.enabled = false;
    }

    public void HideAimModel()
    {
        ToggleRenderers(false);
        if (m_text)
            m_text.enabled = false;
    }

    public void ShowAimModel()
    { ToggleRenderers(true);}

    public void SetText(string _text)
    {
        if (!m_text)
        {
            Debug.LogWarning("Text on top of ghost was not set as the TMPro object is not linked.");
            return;
        }

        m_text.text = _text;
        m_text.enabled = true;
        m_text.transform.position = transform.position + m_textLocalPosition;
    }

    void ToggleRenderers(bool _shown)
    {
        foreach (MeshRenderer mrend in m_renderers)
            mrend.enabled = _shown;
    }

}
