using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpPopupController : MonoBehaviour
{

    public GameObject m_helperPopup;
    private RectTransform m_helperPopupRect;
    private TextMeshProUGUI m_headerText;
    private TextMeshProUGUI m_bodyText;

    public float m_popupDelayTime;

    public bool m_mouseEntered;
    private float m_mouseEnterTime;
    private bool m_popUpEnabled;
    private int m_abilityType; // 0 = move, 1 = Undo, 2 = Attack, 3 = Hide, 4 = Overwatch, 5 = Special
    private int m_ghostSelectedOffset; // As ghosts have different ability. Offset needed in the list when a special is highlighted

    // The title texts for the pop up
    public List<string> m_textHeaders;
    // The body texts for the pop up
    public List<string> m_textBodies;

    static HelpPopupController instance;

    public static HelpPopupController Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        m_helperPopupRect = m_helperPopup.GetComponent<RectTransform>();
        m_headerText = m_helperPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_bodyText = m_helperPopup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // Pop up only appears after the mouse is hovered over the button
        if(!m_popUpEnabled && m_mouseEntered && Time.time > (m_mouseEnterTime + m_popupDelayTime))
        {
            m_helperPopup.SetActive(true);
            updateText();
            m_popUpEnabled = true;
        }

        //If the popup is enabled, make it follow the mouse
        if (m_popUpEnabled)
            m_helperPopup.transform.position = new Vector2(Input.mousePosition.x + m_helperPopupRect.rect.width / 2, Input.mousePosition.y + m_helperPopupRect.rect.height / 2);
    }


    // Update is called once per frame
    public void MouseEnter(int abilityType)
    {
        if (!m_mouseEntered)
        {
            m_mouseEnterTime = Time.time;
            m_mouseEntered = true;
            m_abilityType = abilityType;
        }
    }

    // Update is called once per frame
    public void MouseExit()
    {
        m_mouseEntered = false;        

        if (m_popUpEnabled)
        {
            // Disable the popup
            m_helperPopup.SetActive(false);
            m_popUpEnabled = false;
        }
    }

    public void UpdateGhostSelected(GameObject selectedGhost)
    {
        GhostAbilityBehaviour m_ghostAbilityBehaviour = selectedGhost.GetComponent<GhostAbilityBehaviour>();

        //Debug.Log(selectedGhost.GetComponent<GhostAbilityBehaviour>().m_ghostType);
        if (m_ghostAbilityBehaviour.m_ghostType == GhostType.WALLER)
            m_ghostSelectedOffset = 0;
        else if (m_ghostAbilityBehaviour.m_ghostType == GhostType.DOLLER)
            m_ghostSelectedOffset = 1;
        else if (m_ghostAbilityBehaviour.m_ghostType == GhostType.SCREAMER)
            m_ghostSelectedOffset = 2;
        else if (m_ghostAbilityBehaviour.m_ghostType == GhostType.MONSTER)
            m_ghostSelectedOffset = 3;

        //Update the pop text
        if (m_popUpEnabled)
            updateText();
    }

    private void updateText()
    {
        int abilityType = m_abilityType;
        if (m_abilityType == 5)
            abilityType += m_ghostSelectedOffset;

        m_headerText.text = m_textHeaders[abilityType];
        m_bodyText.text = m_textBodies[abilityType];
    }
}
