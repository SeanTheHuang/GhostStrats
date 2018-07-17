using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    // Use this for initialization
    public Transform m_target;
    public float m_moveSpeed = 2;

    float m_radius = 5;
    float m_angle;
    float mouseDistance = 50;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MouseScreenPos();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("hit");
            Rotate();
        }
    }

    void Rotate()
    {
        m_angle -= 45;
        float x = m_radius * Mathf.Cos(m_angle * Mathf.Deg2Rad);
        float yz = m_radius * Mathf.Sin(m_angle * Mathf.Deg2Rad);

        //transform.eulerAngles += Vector3.up * -45;
        Vector3 oldpos = transform.position;
        transform.position =  new Vector3(x, transform.position.y, yz);


        transform.LookAt(m_target);
        transform.eulerAngles = new Vector3(45, transform.eulerAngles.y, transform.eulerAngles.z);
        if (m_target)
        {

        }
        else
        {

        }
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

        Vector3 fwrd = Vector3.Normalize(new Vector3(transform.forward.x, transform.position.y, transform.forward.z));

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
