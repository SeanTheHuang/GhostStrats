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
    public PunkHiveMind m_hiveMind;

    int m_movesPerformed;

    Vector3 m_roomToExplore;
    Vector3 m_lastRoomExplored;

    public int m_attackRange = 0;
    public int m_attackDamage = 0;
    [HideInInspector]
    public bool m_finishedMoving = false;

    Vector3 v3debug;
    private void Awake()
    {
        m_wallMask = (1 << LayerMask.NameToLayer("Wall"));
        m_realPath = new List<Vector3> { };
        m_roomToExplore = m_hiveMind.m_HouseLocations[Random.Range(0,m_hiveMind.m_HouseLocations.Count)];
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
            //DoTurn();
        }
    }

    public override void OnDeath()
    {
    }
    public override void OnSpawn()
    {
    }
    public override void OnEntityHit(int _damage)
    {
        Debug.Log(transform.name + " has been hit for " + _damage.ToString() + " damage.");
    }
    public override void OnSelected()
    {
    }
    public override void ChooseAction()
    {
        
    }

    public void DoTurn()
    {
        m_finishedMoving = false;
        OnStartOfTurn();
        

        ActionPhase();
        OnEndOfTurn();
        //Debug.Log("PunkTurnEnd");
    }


     void OnStartOfTurn()
    {
        m_movesPerformed = 0;
        if(transform == m_roomToExplore)
        {
            ChooseNewRoom();
            
        }
    }

     void ActionPhase()
    {
        ChooseLocation();
        if(m_prey)
        {
            
            if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2 (m_prey.position.x,m_prey.position.z)) <= m_attackRange)
            {
                //Attack
                //face them
                m_prey.GetComponent<EntityBase>().OnEntityHit(m_attackDamage);
                Debug.Log("attacked entity");
            }
        }
    }

     void OnEndOfTurn()
    {
        
    }


    public void ChooseLocation()
    {
        //Debug.Log("loc chose");
        Sight();//adds targets
        ChooseTarget();//chooses best target 

        Transform wheretogo = transform;//assigning value so unity not angry
        if(m_prey)
        {//FOUND A TARGET MOVE TOWARDS IT
            wheretogo = m_prey;
            Debug.Log("going for target");
        }
        else
        {
            //search rooms/patrol
            //if seen a ghost recently, head to the closest room patrol spot
            //else cycle between room patrols looking for things.

            //attentions - noise, last seen ghost, other people 
            if(m_hiveMind.m_Noises.Count != 0)
            {
                float distance = 10000;
                for(int i = 0; i < m_hiveMind.m_Noises.Count; i++)
                {
                    float d = Vector3.Distance(transform.position, m_hiveMind.m_Noises[i].m_center.position);
                    if (d < distance)
                    {
                        wheretogo = m_hiveMind.m_Noises[i].m_center;
                    }
                }

                wheretogo.position = new Vector3(wheretogo.position.x + Random.Range(-4, 4),
                    wheretogo.position.y,
                    wheretogo.position.z + Random.Range(-4, 4));//randomise the sound pos a bit
            }
            else if (transform.position != m_roomToExplore.position)
            {
                //Debug.Log("hasnt arrived: roomtoEx=" + m_roomToExplore.position);
                wheretogo = m_roomToExplore;
            }
            else if (transform.position == m_roomToExplore.position)
            {
                Debug.Log("reached destination");
                ChooseNewRoom();
                wheretogo = m_roomToExplore;
            }

        }

        previousNode = PathRequestManager.Instance().NodeFromWorldPoint(wheretogo.position);
        PathRequestManager.RequestPath(transform.position, wheretogo.position, 1, OnPathFound);
        v3debug = wheretogo.position;

        StartCoroutine(FollowPath());
    }


    void OnPathFound(Vector3[] _path, bool _pathFound)
    {
        if (_pathFound)
        {
            m_realPath.Clear();
            if (m_prey)
            {
                for (int i = 0; i < _path.Length - 1; i++)
                {
                    m_realPath.Add(_path[i]);
                }
            }
            else
            {
                for (int i = 0; i < _path.Length; i++)
                {
                    m_realPath.Add(_path[i]);
                }
            }
        }
        else
        {
            //im not sure
            Debug.Log("Path not found : " + v3debug);

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
        m_finishedMoving = true;
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
        //for all targets set them to visble so all punks can attack
        foreach (Collider t in m_Targets)
        {
            if (t.transform.GetComponent<GhostController>())
            {
                t.transform.GetComponent<GhostController>().SeenbyPunk();
            }

            
        }

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
                else if(t.GetComponent<GhostHole>())
                {//spawners
                    index = i;
                    break;
                }
                
            }

            if(index != -1)
            {
                m_prey = m_Targets[index].transform;
                Debug.Log("foundtarget");
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

    void ChooseNewRoom()
    {
        //dont use this for the first room
        m_lastRoomExplored = m_roomToExplore;
        while (true)
        {
            m_roomToExplore = m_hiveMind.m_HouseLocations[Random.Range(0, m_hiveMind.m_HouseLocations.Count)];
            if(m_roomToExplore != m_lastRoomExplored)
            {
                break;
            }
        }
        Vector3 pos = m_roomToExplore.position;
        pos.x += Random.Range(-1, 2);
        pos.z += Random.Range(-1, 2);
        m_roomToExplore.position = pos;
    }
}