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
    Transform m_prey, m_oldPrey;
    int m_wallMask;

    Node previousNode;
    List<Vector3> m_pointList, m_realPath;

    int m_movesPerformed;

    bool moving = false;
    private void Awake()
    {
        m_wallMask = (1 << LayerMask.NameToLayer("Wall"));
        m_realPath = new List<Vector3> { };
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("l pressed");
            /*Sight();
            if(m_Targets.Count != 0)
            {
                Debug.Log("targets in sight");
                Debug.Log(m_Targets[0].gameObject.name);
            }*/
            ChooseLocation();
        }
    }

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

    public void ChooseLocation()
    {
        Debug.Log("loc chose");
        Sight();//adds targets
        ChooseTarget();//chooses best target 

        if(m_prey)
        {//FOUND A TARGET MOVE TOWARDS IT
            previousNode = PathRequestManager.Instance().NodeFromWorldPoint(m_prey.position);
            m_pointList = new List<Vector3>();
            m_pointList.Add(m_prey.position);
            PathRequestManager.RequestPath(transform.position, m_prey.position, 1, OnPathFound);
            StartCoroutine(FollowPath());
        }
        else
        {
            //search rooms/patrol
            Debug.Log("else");
        }

    }

    void OnPathFound(Vector3[] _path, bool _pathFound)
    {
        Debug.Log("here");
        if (_pathFound)
        {
            m_realPath.Clear();
            for (int i = 0; i < _path.Length - 1; i++) 
            {
                m_realPath.Add(_path[i]);

            }
            
        }
        else
        {
            //im not sure
            Debug.Log("else ??");
        }
    }

    IEnumerator FollowPath()
    {
        for (int i = 0; i < m_realPath.Count; i++)
        {
            while (true)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, m_realPath[i], m_moveSpeed * Time.deltaTime);

                if(newPos == transform.position)
                {
                    break;//reached point
                }
                else
                {
                    transform.position = newPos;
                }

                yield return null;
            }

            m_movesPerformed++;

            if (m_movesPerformed == m_maxMoves)
            {//end if used up all moves // moved as far as possible
                break;
            }

            m_oldPrey = m_prey;
            Sight();
            ChooseTarget();

            if(m_prey != m_oldPrey)
            {//choose a new path
                PathRequestManager.RequestPath(transform.position, m_prey.position, 1, OnPathFound);
                i = 0;//reset for-loop
            }
        }

        //the attack if possible
        yield return null;
    }

    void Sight()
    {
        m_Targets.Clear();
        Collider[] targets = Physics.OverlapSphere(transform.position, circleSightRadius, m_targetmask);
        if (targets.Length != 0)
        {
            for(int i=0; i < targets.Length;i++)
            {
                if (SightBehindWall(targets[i].transform) == false)
                {
                    m_Targets.Add(targets[i]);
                }
            }
        }
        
        targets = Physics.OverlapSphere(transform.position, forwardSightRadius, m_targetmask);
        for (int i = 0; i < targets.Length; i++)
        {
            Vector2 v1 = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 v2 = new Vector2(targets[i].transform.position.x, targets[i].transform.position.z);
            if (Vector2.Angle(v1, v2) < forwardSightAngle)
            {
                if (SightBehindWall(targets[i].transform) == false)
                {
                    m_Targets.Add(targets[i]);
                }
            }
        }
        //for all tickets set them to visble so all punks can attack
        
    }

    bool SightBehindWall(Transform _t)
    {//checks if there is a wall in the way
        return (Physics.Linecast(transform.position, _t.position, m_wallMask));
    }

    void ChooseTarget()
    {
        m_prey = null;
        if(m_Targets.Count == 0)
        {
            return;
        }

        int lowestHealth = 10000;
        int index = -1;
        for (int i = 0; i < m_Targets.Count; i++) 
        {
            Transform t = m_Targets[i].transform;

            if (Vector3.Distance(transform.position, t.position) < 5)
            {//check if they are in a close enough distance

                if (t.GetComponent<GhostController>())
                {//ghosts
                    if (t.GetComponent<GhostController>().GetCurrentHealth() < lowestHealth)
                    {//check for health
                        lowestHealth = t.GetComponent<GhostController>().GetCurrentHealth();
                        index = i;
                    }
                }
            }
            if(index != -1)
            {
                m_prey = m_Targets[index].transform;
            }
            else
            {
                //none found but have been seen //move as close as possible
            }

            //ghosts
            //check if they are in range, then check lowest health, then go for closest


            //then targets
        }
    }
}