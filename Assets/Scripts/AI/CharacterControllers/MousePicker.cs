using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePicker : MonoBehaviour {

    private bool m_isActive;
    private Action<Vector3> m_callBack;

    private static MousePicker m_instance;
    private LayerMask m_groundMask;

    public static MousePicker Instance()
    {
        return m_instance;
    }
    private void Awake()
    {
        m_instance = this;
        m_isActive = false;
        m_groundMask = LayerMask.NameToLayer("Default");
    }

    private void Update()
    {
        if (!m_isActive)
            return;

        MouseLogic();
    }

    void MouseLogic()
    {
        // If click on pl

        // Default: Left mouse button right now
        if (Input.GetButtonDown("Fire1"))
        {
            // Find out where player pressed, 
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
