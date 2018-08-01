using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour {

    public float m_maxMoveAmount = 50;
    public float m_newTargetTime = 0.3f;
    public float m_lerpVal = 10;
    Vector3 m_startPos, m_targetPos;
    Vector3 m_startScale;
    float m_lastTime;

    private void Awake()
    {
        m_targetPos = m_startPos = transform.position;
        m_startScale = transform.localScale;
    }

    private void Update()
    {
        if (Time.time - m_lastTime > m_newTargetTime)
        {
            m_lastTime = Time.time;
            m_targetPos = new Vector3(Random.Range(-m_maxMoveAmount, m_maxMoveAmount), Random.Range(-m_maxMoveAmount, m_maxMoveAmount), 0) + m_startPos;
        }

        transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_lerpVal);
        float val = (Mathf.Sin(Time.time * 0.1f) + 1) * 0.1f;
        transform.localScale = m_startScale + new Vector3(val, val, val);
    }
}
