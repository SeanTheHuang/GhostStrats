using System.Collections;
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

    [Header("Camera Limits")]
    public Vector2 m_lowerLimits;
    public Vector2 m_upperLimits;
    public float m_zoomAmount = 4;

    [Header("Others")]
    public float m_lerpValue = 8;
    public float m_freeMoveLerpVal = 35;
    public float m_cameraPanSpeed = 3;
    public float m_zoomSpeed = 4;
    public float mouseDistFromEdgeOfScreen = 20;
    public Vector3 m_cameraOffset;
    
    public bool m_lockCameraMovement = false;

    Vector3 m_currentOffset;
    float m_cameraBaseHeight;
    Quaternion m_initialRotation;
    CameraState m_cameraState;

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

    #region CAMERA_MOVE_LOGIC

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
        if (m_lockCameraMovement)
        {
            Vector3 v3 = m_initialTargetPosition + m_cameraOffset;
            transform.position = Vector3.Lerp(transform.position, v3, m_freeMoveLerpVal * Time.deltaTime);
            return;
        }

        // Move towards current target position, update target position as mouse touches the screen edges
        UpdateTargetPosition();
        Vector3 targetPos = m_targetPosition + m_currentOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, m_lerpValue * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_initialRotation, m_lerpValue * Time.deltaTime);
        ClampCameraPosition();

        // Added function: Press [F1] to reset to original target
        if (Input.GetKeyDown(KeyCode.F1))
        {
            m_targetPosition = m_initialTargetPosition;
            m_currentOffset = m_cameraOffset;
        }

        // Added function, scroll wheel = zoom in and out
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll < 0)
            m_currentOffset.y = Mathf.Clamp(m_currentOffset.y + m_zoomSpeed * Time.deltaTime, m_cameraOffset.y - m_zoomAmount, m_cameraOffset.y + m_zoomAmount);
        else if (scroll > 0)
            m_currentOffset.y = Mathf.Clamp(m_currentOffset.y - m_zoomSpeed * Time.deltaTime, m_cameraOffset.y - m_zoomAmount, m_cameraOffset.y + m_zoomAmount);
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

        // Ensure target position is within clamp range
        m_targetPosition.x = Mathf.Clamp(m_targetPosition.x, m_lowerLimits.x, m_upperLimits.y);
        m_targetPosition.z = Mathf.Clamp(m_targetPosition.z, m_lowerLimits.y, m_upperLimits.y);
    }

    void ClampCameraPosition()
    {
        float xVal = Mathf.Clamp(transform.position.x, m_lowerLimits.x, m_upperLimits.x);
        float zVal = Mathf.Clamp(transform.position.z, m_lowerLimits.y, m_upperLimits.y);

        transform.position = new Vector3(xVal, transform.position.y, zVal);
    }

    #endregion

    #region SET_CAMERA_STATE

    public void SetOverviewMode()
    { m_cameraState = CameraState.ZOOMED_OUT; }

    public void SetFreeMode(Vector3 _initialTarget)
    {
        m_currentOffset = m_cameraOffset;
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
