using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightFlicker : MonoBehaviour {

    Image light;
    bool m_on = false, m_start = false;
    float m_startTime, m_flicktime;
    float wait = 0.2f;

	// Use this for initialization
	void Start () {
        light = GetComponent<Image>();
        m_on = false;
        m_startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float ran = Random.Range(0, 100);
        if(Time.time - m_startTime < 4.0f)
        {
            if (ran > Random.Range(99, 100))
            {
                m_on = !m_on;
            }
        }
        else
        {
            /*if (m_on == false)
            {
                m_on = true;
            }*/

            if(ran > 98 && m_flicktime == 0)
            {
                Debug.Log("flicktime");
                m_flicktime = Time.time;
                 wait = Random.Range(0.9f, 2.35f);
            }
            else if (m_flicktime != 0)
            {
                if(Time.time - m_flicktime < wait)
                {
                    if (ran > Random.Range(50, 100))
                    {
                        m_on = !m_on;
                        
                    }
                }
                else
                {
                    m_flicktime = 0;
                    m_on = true;
                }
            }
            else if (m_flicktime == 0)
            {
                m_on = true;
            }
        }
        light.enabled = m_on;
    }

    IEnumerator Flicker()
    {
        
        yield return null;
    }
}
