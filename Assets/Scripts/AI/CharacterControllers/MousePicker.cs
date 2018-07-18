using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePicker : MonoBehaviour {

    private bool m_isActive;
    private GhostController m_currentGhost;

    private static MousePicker m_instance;
    public LayerMask m_selectionMask;

    public static MousePicker Instance()
    {
        return m_instance;
    }
    private void Awake()
    {
        m_instance = this;
        m_isActive = false;
    }

    private void Update()
    {
        if (!m_isActive)
            return;

        HoverLogic();
        MouseClickLogic();
    }

    void HoverLogic()
    {
        // Shown path will automatically update as player hover over terrain
        // It will not work if they are hovering over different units

        if (!m_isActive) // Don't do shit if no active
            return;

        // The ray can only hit players or the ground
        RaycastHit rayHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out rayHit, 50, m_selectionMask))
        {
            if (rayHit.transform.CompareTag("Entity/Ghost") || rayHit.transform.CompareTag("Entity/Punk"))
            {
                m_currentGhost.ResetChoosingPathNodes(); // Delete current potential path
            }
            else
            {
                // Player wants to move towards this position
                m_currentGhost.OnTargetLocation(rayHit.point);
            }
        }
        // else, ray did not hit anything important
    }

    void MouseClickLogic()
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

            if (Physics.Raycast(ray, out rayHit, 50, m_selectionMask)){
                if (rayHit.transform.CompareTag("Entity/Ghost")) {
                    // Select that ghost
                    GameMaster.Instance().UpdateSelectedGhost(rayHit.transform.gameObject);
                }
                else if (rayHit.transform.CompareTag("Entity/Punk")) {
                    // TODO: should update UI on the punk being selected
                }
                else {
                    // Player wants to move towards this position
                    m_currentGhost.OnConfirmTargetPosition();
                }
            }
            // else, ray did not hit anything important
        }

        if (Input.GetButtonDown("Fire2"))
            m_currentGhost.ResetPath();
    }

    public void StartPicking(Vector3 _startPosition, GhostController _currentGhost)
    {
        m_currentGhost = _currentGhost;
        m_isActive = true;
    }

    public void StopPicking()
    {
        m_isActive = false;
    }
}
