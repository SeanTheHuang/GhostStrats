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
    public Transform m_overviewShot;
    public Transform m_target;
    private Vector3 m_targetLastPos;
    public float m_moveSpeed = 2;

    float m_radius = 5;
    float m_angle = 0;
    float mouseDistance = 50;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Focus();
    }
       
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

    void MoveTowardsFunction()
    {
        
    }

    void Rotate()
    {
        m_angle -= 20 * Time.deltaTime;
        if (m_angle > 360)
        {
            m_angle -= 360;
        }
        else if (m_angle < 0)
        {
            m_angle += 360;
        }
        Focus();
    }

    void Focus()
    {
        return;
        float x = m_radius * Mathf.Cos(m_angle * Mathf.Deg2Rad);
        float yz = m_radius * Mathf.Sin(m_angle * Mathf.Deg2Rad);

        x += m_target.position.x;
        yz += m_target.position.z;
       //Debug.Log("x: " + x);
       //Debug.Log("yz :" + yz);
        
       /* Vector3 center = transform.position + (transform.forward * m_radius);
        x += center.x;
        yz += center.z;*/

        transform.position = new Vector3(x, transform.position.y, yz);


       // transform.LookAt(center);
        transform.LookAt(m_target);

        transform.eulerAngles = new Vector3(45, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void MouseScreenPos()
    {
        float mousePosy = Input.mousePosition.y;
        float mousePosx = Input.mousePosition.x;

        if (mousePosy < 0 || mousePosy > Screen.height
            || mousePosx < 0 || mousePosx > Screen.width) 
        {
            return;
        }

        float movspd = m_moveSpeed * Time.deltaTime;

       // Vector3 fwrd = Vector3.Normalize(new Vector3(transform.forward.x, transform.position.y, transform.forward.z));

        Vector3 dir = transform.forward;
        dir.y = 0;
        dir.Normalize();

        if (mousePosy < mouseDistance)
        {
            transform.position += -dir * movspd;
        }
        else if ( Screen.height - mousePosy < mouseDistance )
        {  
            transform.position += dir * movspd;
        }

        if(mousePosx < mouseDistance)
        {
            transform.position += -transform.right * movspd;
        }
        else if (Screen.width - mousePosx < mouseDistance)
        {
            transform.position += transform.right * movspd;
        }
    }
}
