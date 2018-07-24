using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// NOTE: This class must be put onto the canvas!!
public class TextEffectController : MonoBehaviour {

    [Header("Prefabs")]
    public Transform m_titleTextPrefab;
    public Transform m_effectsTextPrefab;

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
    }

    public void PlayTitleText(string _titleText)
    {
        Transform newTitle = Instantiate(m_titleTextPrefab, Vector3.zero, m_titleTextPrefab.rotation, transform);
        newTitle.GetComponent<RectTransform>().anchoredPosition3D = m_titleTextPrefab.position; // Properly set position here
        newTitle.GetComponent<TitleText>().InitializeAndStart(_titleText);
    }
}
