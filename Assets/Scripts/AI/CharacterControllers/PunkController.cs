using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunkController : EntityBase
{
    public LayerMask m_targetmask;
    float circleSightRadius = 3;
    float forwardSightAngle = 30;
    float forwardSightRadius = 6;
    List<Collider> m_Targets = new List<Collider> { };

    public override void OnDeath()
    {
    }
    public override void OnSpawn()
    {
    }
    public override void OnEntityHit()
    {
    }
    public override void SelectingWhereToMove()
    {
    }
    public override void ChooseAction()
    {
    }

    void Sight()
    {
        m_Targets.Clear();
        Collider[] targets = Physics.OverlapSphere(transform.position, circleSightRadius, m_targetmask);
        if (targets.Length != 0)
        {
            for(int i=0; i < targets.Length;i++)
            {
                m_Targets.Add(targets[i]);
            }
        }
        //need to raycast for walls
        targets = Physics.OverlapSphere(transform.position, forwardSightRadius, m_targetmask);
        for (int i = 0; i < targets.Length; i++)
        {
            Vector2 v1 = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 v2 = new Vector2(targets[i].transform.position.x, targets[i].transform.position.z);
            if (Vector2.Angle(v1, v2) < forwardSightAngle)
            {
                m_Targets.Add(targets[i]);
            }
        }
    }

    void ChooseTarget()
    {
        if(m_Targets.Count == 0)
        {
            return;
        }

        int lowestHealth = 10000;
        for (int i = 0; i < m_Targets.Count; i++) 
        {
            Transform t = m_Targets[i].transform;
            

            if(t.GetComponent<GhostController>())
            {//ghosts
                if (t.GetComponent<GhostController>().GetCurrentHealth() < lowestHealth)
                {//check for health
                    lowestHealth = t.GetComponent<GhostController>().GetCurrentHealth();
                }
            }
            //ghosts
            //check if they are in range, then check lowest health, then go for closest

            
            //then targets
        }
    }
}