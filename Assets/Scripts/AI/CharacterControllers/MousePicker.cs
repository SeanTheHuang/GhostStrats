using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePicker : MonoBehaviour {

    private bool m_isActive;
    private Action<Vector3> m_callBack;

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
        // If click on ghost, ghost is now currently selected
        // If click on ground, currently selected ghost will try move towards it

        // Default: Left mouse button right now
        if (Input.GetButtonDown("Fire1"))
        {
            // The ray can only hit players or the ground
            RaycastHit rayHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, 50, m_selectionMask)){
                if (rayHit.transform.CompareTag("Entity/Ghost")) {
                    // Select that ghost
                    Debug.Log("Currently selecting ghost:" + rayHit.transform.name);
                    GameMaster.Instance().UpdateSelectedGhost(rayHit.transform.gameObject);
                }
                else {
                    // Player wants to move towards this position
                    m_callBack(rayHit.point);
                }
            }
            // else, ray did not hit anything important
        }
    }

    public void StartPicking(Vector3 _startPosition, Action<Vector3> _callBack)
    {
        m_callBack = _callBack;
        m_isActive = true;
    }

    public void StopPicking()
    {
        m_isActive = false;
    }
}
