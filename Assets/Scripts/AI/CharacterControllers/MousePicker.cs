using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePicker : MonoBehaviour {

    private bool m_isActive;
    private Action<Vector3> m_setTargetCallback;
    private Action m_resetCallback;

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

        MouseLogic();
    }

    void MouseLogic()
    {
        // If [LEFT CLICK] on [GHOST], ghost is now currently selected
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
                else {
                    // Player wants to move towards this position
                    m_setTargetCallback(rayHit.point);
                }
            }
            // else, ray did not hit anything important
        }

        if (Input.GetButtonDown("Fire2"))
            m_resetCallback();
    }

    public void StartPicking(Vector3 _startPosition, Action<Vector3> _setTargetCallback, Action _resetCallback)
    {
        m_setTargetCallback = _setTargetCallback;
        m_resetCallback = _resetCallback;
        m_isActive = true;
    }

    public void StopPicking()
    {
        m_isActive = false;
    }
}
