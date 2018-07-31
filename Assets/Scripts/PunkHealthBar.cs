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
            float scale;
            if (m_hp < 0)
            {
                m_hp = 0;
                scale = 0;
            }
            else
            {
                m_hp = m_punkCon.GetCurrentHealth();
                 scale = (float)m_hp / (float)m_maxHp;

            }
            scale = Mathf.Clamp01(scale);
            m_greenBar.localScale = new Vector3(scale, 1, 1);
            
            float xoff = 1 - scale;
            xoff *= 0.5f;
            m_greenBar.localPosition = new Vector3(-xoff, 0, -0.001f);
        }


    }

}
