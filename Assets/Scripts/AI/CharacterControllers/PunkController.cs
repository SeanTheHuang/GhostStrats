using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunkController : EntityBase
{
    public LayerMask m_targetmask;
    float circleSightRadius = 3;
    float forwardSightAngle = 30;
    float forwardSightRadius = 6;

    public override void OnDeath()
    {
    }
    public override void OnSpawn()
    {
    }
    public override void OnEntityHit()
    {
    }
    public override void OnSelected()
    {
    }
    public override void ChooseAction()
    {
    }

    void Sight()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, circleSightRadius, m_targetmask);
        List<Collider> fTargets = new List<Collider> { };
        if (targets.Length != 0)
        {
            //a target is in the sight range
            if(targets.Length >= 2)
            {//multiple target choose best
                //check health, then check distance
            }

            
        }

        targets = Physics.OverlapSphere(transform.position, forwardSightRadius, m_targetmask);
        for (int i = 0; i < targets.Length; i++) 
        {
            Vector2 v1 = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 v2 = new Vector2(targets[i].transform.position.x, targets[i].transform.position.z);
            if(Vector2.Angle(v1, v2) < forwardSightAngle)
            {
                fTargets.Add(targets[i]);
            }
        }

        if (fTargets.Count != 0)
        {
            //target is forward
        }
        
    }
}
