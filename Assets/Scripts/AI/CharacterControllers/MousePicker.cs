using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum MouseMode
{
    INACTIVE,
    MOVEMENT,
    OVERUI
}

public class MousePicker : MonoBehaviour {

    private MouseMode m_mouseMode;
    public GhostController m_currentGhost;

    private static MousePicker m_instance;
    public LayerMask m_selectionMask;

    private HelpPopupController m_helpPopupController;

    public static MousePicker Instance()
    {
        return m_instance;
    }
    private void Awake()
    {
        m_instance = this;
        m_mouseMode = MouseMode.INACTIVE;
        m_helpPopupController = GetComponent<HelpPopupController>();
    }

    private void Update()
    {
        if (m_mouseMode == MouseMode.MOVEMENT && m_helpPopupController.m_mouseEntered)
            m_mouseMode = MouseMode.OVERUI;

        if (m_mouseMode == MouseMode.OVERUI && !m_helpPopupController.m_mouseEntered)
            m_mouseMode = MouseMode.MOVEMENT;

        if (m_mouseMode == MouseMode.INACTIVE || m_mouseMode == MouseMode.OVERUI)
            return;

        HoverLogic();
        MouseClickLogic();
    }

    void HoverLogic()
    {
        // Shown path will automatically update as player hover over terrain
        // It will not work if they are hovering over different units

        // The ray can only hit players or the ground
        RaycastHit rayHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, 50, m_selectionMask))
        {
            if (m_mouseMode == MouseMode.MOVEMENT)
            {
                // Check node is valid
                Node node = PathRequestManager.Instance().NodeFromWorldPoint(rayHit.point);
                    
                if (node != null)
                {
                    if (!node.Walkable)
                        return;
                    // Check if no punks or ghosts there
                    List<Vector3> tempList = new List<Vector3>();
                    tempList.Add(node.WorldPosition);

                    if (GameMaster.Instance().GetPunksAtLocations(tempList).Count > 0)
                        return;
                    if (GameMaster.Instance().GetGhostsAtLocations(tempList).Count > 0)
                        return;

                    // Player wants to move towards this position
                    m_currentGhost.OnTargetLocation(rayHit.point);
                }
            }
        }
        // else, ray did not hit anything important
    }

    void MouseClickLogic()
    {
        switch (m_mouseMode)
        {
            case MouseMode.MOVEMENT:
                GhostMovementLogic();
                break;
            default:
                Debug.Log("?? What is this mouse mode");
                break;
        }
    }

    void GhostMovementLogic()
    {
        // If [LEFT CLICK] on [GHOST], ghost is now currently selected
        // If [LEFT CLICK] on [PUNK], punk known stats should show up in some UI window
        // If [LEFT CLICK] on [GROUND], currently selected ghost will try move towards it
        // If player presses [RIGHT CLICK], it will reset their turn action choices

        // Default: Left mouse button right now
        if (Input.GetButtonDown("Fire1"))
        {
            // The ray can only hit players or the ground
            RaycastHit rayHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, 50, m_selectionMask))
            {
                if (rayHit.transform.CompareTag("Entity/Ghost"))
                {
                    // Select that ghost
                    GameMaster.Instance().UpdateSelectedGhost(rayHit.transform.gameObject);
                }
                else if (rayHit.transform.CompareTag("Entity/Punk"))
                {
                    // TODO: should update UI on the punk being selected
                }
                else
                {
                    // Player wants to move towards this position
                    m_currentGhost.OnConfirmTargetPosition();
                }
            }
            // else, ray did not hit anything important
        }

        if (Input.GetButtonDown("Fire2"))
        {
            m_currentGhost.ResetAction();
            m_mouseMode = MouseMode.MOVEMENT;
            //m_currentGhost.GetComponent<GhostUi>().OnSelected();
        }
    }

    void GhostAimAbilityLogic()
    {
        m_currentGhost.ConfirmSkillDirection();
    }

    public void FinishAimingAbility()
    {
        // Currently goes to movement mode again
        m_mouseMode = MouseMode.MOVEMENT;
    }

    public void StartPicking(Vector3 _startPosition, GhostController _currentGhost)
    {
        m_currentGhost = _currentGhost;
        m_mouseMode = MouseMode.MOVEMENT;
    }

    public void StopPicking()
    {
        m_mouseMode = MouseMode.INACTIVE;
        m_currentGhost = null;
    }

    public void PausePicking()
    {
        m_mouseMode = MouseMode.INACTIVE;
    }

    public void ResumePicking()
    {
        m_mouseMode = MouseMode.MOVEMENT;
    }
}
