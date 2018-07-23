﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    enum CameraState
    {
        FREE,
        ZOOMED_OUT,
        FOLLOW
    }

    public static CameraControl Instance
    { get; private set; }


    // Use this for initialization
    [Header("Transform targets")]
    public Transform m_overviewShotTransform;
    public Transform m_targetTransform;
    private Vector3 m_targetPosition;
    private Vector3 m_initialTargetPosition;

    [Header("Others")]
    public float m_lerpValue = 50;
    public float m_cameraPanSpeed = 3;
    public float mouseDistFromEdgeOfScreen = 20;
    public Vector3 m_cameraOffset;

    Quaternion m_initialRotation;

    CameraState m_cameraState;
    float m_radius = 5;
    float m_angle = 0;

    private void Awake()
    {
        Instance = this;
        m_cameraState = CameraState.ZOOMED_OUT;
        m_initialRotation = transform.rotation;

        // Ensure can move overview transform easily
        m_overviewShotTransform.SetParent(null);
    }

    private void Update()
    {
        switch (m_cameraState)
        {
            case CameraState.ZOOMED_OUT:
                ZoomOutLogic();
                break;
            case CameraState.FOLLOW:
                FollowLogic();
                break;
            case CameraState.FREE:
                FreeCameraLogic();
                break;
            default:
                break;
        }

        // TEST KEYS FOR CAMERA
        if (Input.GetKeyDown(KeyCode.B))
            m_cameraState = CameraState.ZOOMED_OUT;
        else if (Input.GetKeyDown(KeyCode.N))
            m_cameraState = CameraState.FOLLOW;
        else if (Input.GetKeyDown(KeyCode.M))
            m_cameraState = CameraState.FREE;
    }

    #region CAMERA_LOGIC

    void ZoomOutLogic()
    {
        if (!m_overviewShotTransform)
            return;

        // Move towards overview position and rotation
        transform.position = Vector3.Lerp(transform.position, m_overviewShotTransform.position, m_lerpValue * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_overviewShotTransform.rotation, m_lerpValue * Time.deltaTime);
    }

    void FollowLogic()
    {
        if (!m_targetTransform) // Nothing to follow
            return;

        // Move to follow target, keep original rotation
        Vector3 targetPos = m_targetTransform.position + m_cameraOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, m_lerpValue * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_initialRotation, m_lerpValue * Time.deltaTime);
    }

    void FreeCameraLogic()
    {
        // Move towards current target position, update target position as mouse touches the screen edges
        UpdateTargetPosition();

        Vector3 targetPos = m_targetPosition + m_cameraOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, m_lerpValue * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_initialRotation, m_lerpValue * Time.deltaTime);

        // Added function: Press [F1] to reset to original target
        if (Input.GetKeyDown(KeyCode.F1))
            m_targetPosition = m_initialTargetPosition;
    }

    void UpdateTargetPosition()
    {
        float mousePosY = Input.mousePosition.y;
        float mousePosX = Input.mousePosition.x;

        // X dir pan
        if (mousePosX <= mouseDistFromEdgeOfScreen)
            m_targetPosition.x -= m_cameraPanSpeed * Time.deltaTime;
        else if (mousePosX >= Screen.width - mouseDistFromEdgeOfScreen)
            m_targetPosition.x += m_cameraPanSpeed * Time.deltaTime;

        // Y dir pan
        if (mousePosY <= mouseDistFromEdgeOfScreen)
            m_targetPosition.z -= m_cameraPanSpeed * Time.deltaTime;
        else if (mousePosY >= Screen.height - mouseDistFromEdgeOfScreen)
            m_targetPosition.z += m_cameraPanSpeed * Time.deltaTime;
    }

    #endregion

    #region SET_CAMERA_STATE

    public void SetOverviewMode()
    { m_cameraState = CameraState.ZOOMED_OUT; }

    public void SetFreeMode(Vector3 _initialTarget)
    {
        m_targetPosition = m_initialTargetPosition = _initialTarget;
        m_cameraState = CameraState.FREE;
    }

    public void SetFollowMode(Transform _followTarget)
    {
        m_targetTransform = _followTarget;
        m_cameraState = CameraState.FOLLOW;
    }

    #endregion

    // Update is called once per frame
    //void Update()
    //{
    //    MouseScreenPos();
    //    //if(Input.GetKey(KeyCode.Space))
    //    //{
    //    //    Debug.Log("hit");
    //    //    Rotate();
    //    //}
    //    if (!m_target)
    //        return;
    //    if(m_target.position != m_targetLastPos)
    //    {
    //        Focus();
    //        m_targetLastPos = m_target.position;
    //    }
    //}

    //void Rotate()
    //{
    //    m_angle -= 20 * Time.deltaTime;
    //    if (m_angle > 360)
    //    {
    //        m_angle -= 360;
    //    }
    //    else if (m_angle < 0)
    //    {
    //        m_angle += 360;
    //    }
    //    Focus();
    //}

    //void Focus()
    //{

    //    float x = m_radius * Mathf.Cos(m_angle * Mathf.Deg2Rad);
    //    float yz = m_radius * Mathf.Sin(m_angle * Mathf.Deg2Rad);

    //    x += m_target.position.x;
    //    yz += m_target.position.z;
    //    //Debug.Log("x: " + x);
    //    //Debug.Log("yz :" + yz);

    //    /* Vector3 center = transform.position + (transform.forward * m_radius);
    //     x += center.x;
    //     yz += center.z;*/

    //    transform.position = new Vector3(x, transform.position.y, yz);


    //    // transform.LookAt(center);
    //    transform.LookAt(m_target);

    //    transform.eulerAngles = new Vector3(45, transform.eulerAngles.y, transform.eulerAngles.z);
    //}
}
