using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunkHealthBar : MonoBehaviour
{
    Transform m_greenBar;
    Transform m_redBar;

    PunkController m_punkCon;

    int m_hp;
    int m_maxHp;

    private void Awake()
    {
        m_greenBar = transform.GetChild(0);
        m_redBar = transform.GetChild(1);
        m_punkCon = transform.parent.GetComponent<PunkController>();
        m_hp = m_punkCon.GetCurrentHealth();
        m_maxHp = m_punkCon.m_maxHealth;
    }

    private void Update()
    {

        transform.rotation = Quaternion.identity;
        if(m_hp != m_punkCon.GetCurrentHealth())
        {//change health
            if (m_hp < 0)
            {
                m_hp = 0;
            }
            Debug.Log("TIME TO CHANGE THE HELATH");
            m_hp = m_punkCon.GetCurrentHealth();
            float scale = (float)m_hp / (float)m_maxHp;
            Debug.Log("greenbar xscale should be " + scale);
            m_greenBar.localScale = new Vector3(scale, 1, 1);
            Debug.Log("greenbar xscale is " + m_greenBar.localScale.x);

            float xoff = 1 - scale;
            xoff *= 0.5f;
            m_greenBar.localPosition = new Vector3(-xoff, 0, -0.001f);
        }


    }

}
