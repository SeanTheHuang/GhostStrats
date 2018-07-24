using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunkController : EntityBase
{
    public LayerMask m_targetmask;
    public float circleSightRadius = 1.5f;
    public float forwardSightAngle = 30;
    public float forwardSightRadius = 4;
    List<Collider> m_Targets = new List<Collider> { };
    Transform m_prey, m_oldPrey;
    int m_wallMask;

    //Node previousNode;
    List<Vector3> m_pointList, m_realPath;
    public PunkHiveMind m_hiveMind;

    int m_movesPerformed;

    Vector3 m_roomToExplore;
    Vector3 m_lastRoomExplored;

    public int m_attackRange = 0;
    public int m_attackDamage = 0;
    [HideInInspector]
    public bool m_finishedMoving = false;

    Vector3 v3debug;//used when cant find a path

    PunkStates m_state;
    int m_pathIndex = 0;

    private void Awake()
    {
        m_wallMask = (1 << LayerMask.NameToLayer("Wall"));
        m_realPath = new List<Vector3> { };
        m_roomToExplore = m_hiveMind.m_HouseLocations[Random.Range(0,m_hiveMind.m_HouseLocations.Count)].position;
        //m_roomToExplore = m_hiveMind.m_HouseLocations[0].position;
        m_state = PunkStates.IDLE;
        m_currentHealth = m_maxHealth;

    }
    private void Update()
    {
        switch (m_state)
        {
            case PunkStates.IDLE:
                {//dont move
                    break;
                }
            case PunkStates.MOVING:
                {
                    MovetoNode();
                    break;
                }
            case PunkStates.ATTACK:
                {
                    Attack();
                    break;
                }
            case PunkStates.DEAD:
                {
                    //do nothing
                    break;
                }
            default:
                Debug.Log("ERROR IN  PUNK STATES");
                break;
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
        m_currentHealth -= _damage;
        if(m_currentHealth == 0 && m_state != PunkStates.DEAD)
        {
            m_hiveMind.RemovePunk(transform);
            GameMaster.Instance().RemovePunk(this);
            m_state = PunkStates.DEAD;
            Destroy(gameObject);
        }
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
        m_pathIndex = 0;
        if (transform.position == m_roomToExplore)
        {
            ChooseNewRoom();

        }
    }

    void ActionPhase()
    {
        ChooseLocation();
    }

    void OnEndOfTurn()
    {

    }


    public void ChooseLocation()
    {
        Sight();//adds targets
        ChooseTarget();//chooses best target 

        Vector3 wheretogo = transform.position;//assigning value so unity not angry
        if(m_prey)
        {//FOUND A TARGET MOVE TOWARDS IT
            wheretogo = m_prey.position;
            //Debug.Log("going for target");
        }
        else
        {
            if(m_hiveMind.m_Noises.Count != 0)
            {
                float distance = 10000;
                for(int i = 0; i < m_hiveMind.m_Noises.Count; i++)
                {
                    float d = Vector3.Distance(transform.position, m_hiveMind.m_Noises[i].m_center.position);
                    if (d < distance)
                    {
                        wheretogo = m_hiveMind.m_Noises[i].m_center.position;
                    }
                }

                wheretogo = RandPosAroundWall(wheretogo);//randomise the sound pos a bit
            }
            else if (transform.position != m_roomToExplore)
            {
                //Debug.Log("hasnt arrived: roomtoEx=" + m_roomToExplore.position);
                wheretogo = m_roomToExplore;
            }
            else if (transform.position == m_roomToExplore)
            {
                //Debug.Log("reached destination");
                ChooseNewRoom();
                wheretogo = m_roomToExplore;
            }

        }

        PathRequestManager.RequestPath(transform.position, wheretogo, 1, OnPathFound);
        v3debug = wheretogo;

        //StartCoroutine(FollowPath());
        m_state = PunkStates.MOVING;
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

            for(int i=0; i< m_realPath.Count; i++)// truncs path
            {
                if (i >= m_maxMoves) 
                {
                    m_realPath.RemoveAt(i);
                    i--;
                }
            }

            for(int i = m_realPath.Count -1; i > -1; i--)//removes 
            {
                for (int j = 0; j < m_hiveMind.m_PunkLocations.Count; j++) 
                {
                    if(m_hiveMind.m_PunkLocations[j].position == m_realPath[i])
                    {
                        m_realPath.RemoveAt(i);
                        break;
                    }
                }
            }

        }
        else
        {
            //im not sure
            Debug.Log("Path not found : " + v3debug);

        }
    }

    void MovetoNode()
    {
        if(m_realPath.Count == 0)
        {
            m_state = PunkStates.ATTACK;
            return;
        }
        else if (m_realPath.Count > 0 && m_pathIndex == 0) 
        {
            Vector3 dir = m_realPath[0] - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }


        Vector3 newPos = Vector3.MoveTowards(transform.position, m_realPath[m_pathIndex], m_moveSpeed * Time.deltaTime);

        if (transform.position == m_realPath[m_pathIndex])
        {
            if (GameMaster.Instance().PunkHitOverwatch(this))
            {
                EndTurn();
                return;
            }

            if (m_pathIndex + 1 < m_realPath.Count)
            {
                Vector3 dir = m_realPath[m_pathIndex + 1] - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
            }

            m_pathIndex++;
            if(m_pathIndex == m_realPath.Count)
            {
                //end it?
                m_state = PunkStates.ATTACK;
                //change state
            }
            else
            {
                m_oldPrey = m_prey;
                Sight();
                ChooseTarget();

                if (m_prey != m_oldPrey)
                {
                    PathRequestManager.RequestPath(transform.position, m_prey.position, 1, OnPathFound);
                    m_pathIndex = 0;
                }
                //rotate check new path + sight
            }
        }
        else
        {
            transform.position = newPos;//moves it
        }

    }

    void Attack()
    {
        if (m_prey)
        {
            Vector3 dir = m_prey.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);

            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                new Vector2(m_prey.position.x, m_prey.position.z)) <= m_attackRange)
            {
                //Attack
                //face them

                if (m_attackRange > 1)
                {
                    if (SightBehindWall(m_prey))
                    {
                        return;
                    }
                }
                m_prey.GetComponent<EntityBase>().OnEntityHit(m_attackDamage);
                //Debug.Log("attacked entity");
            }
        }
        EndTurn();
    }


    void EndTurn()
    {
        m_state = PunkStates.IDLE;
        m_finishedMoving = true;
    }
   /* IEnumerator FollowPath()
    {
        if(m_realPath.Count > 0)
        {//rotation
            Vector3 dir = m_realPath[0] - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        for (int i = 0; i < m_realPath.Count; i++)
        {
            while (true)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, m_realPath[i], m_moveSpeed * Time.deltaTime);

                if(newPos == transform.position)
                {
                    if (i + 1 < m_realPath.Count)
                    {
                        Vector3 dir = m_realPath[i + 1] - transform.position;
                        dir.y = 0;
                        transform.rotation = Quaternion.LookRotation(dir);
                    }

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
    }*/


    void Sight()
    {
        m_Targets.Clear();
        Collider[] targets = Physics.OverlapSphere(transform.position, circleSightRadius, m_targetmask);
        if (targets.Length != 0)
        {
            for (int i = 0; i < targets.Length; i++) 
            {
                if (targets[i].GetComponent<GhostController>())
                {
                    if (targets[i].GetComponent<GhostController>().GhostIsAlive == false
                        || targets[i].GetComponent<GhostController>().m_OutofSight == true)//check for out of sight
                    {
                        continue;
                    }
                }
                if (SightBehindWall(targets[i].transform) == false)
                {
                    m_Targets.Add(targets[i]);
                }
            }
        }
        
        targets = Physics.OverlapSphere(transform.position, forwardSightRadius, m_targetmask);
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].GetComponent<GhostController>())
            {
                if (targets[i].GetComponent<GhostController>().GhostIsAlive == false
                    || targets[i].GetComponent<GhostController>().m_OutofSight == true)
                {
                    continue;
                }
            }
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
       /* foreach (Collider t in m_Targets)
        {
            //Debug.Log("target seen");
            if (t.transform.GetComponent<GhostController>())
            {
                t.transform.GetComponent<GhostController>().SeenbyPunk();
            }
        }*/

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
                    if(t.GetComponent<GhostHole>().GetCurrentHealth() < lowestHealth)
                    {
                        lowestHealth = t.GetComponent<GhostHole>().GetCurrentHealth();
                        index = i;
                    }
                }
                else
                {
                    Debug.Log("BUG: PUNK Targeting not ghost or hole");
                }
                
            }

            if(index != -1)
            {
                m_prey = m_Targets[index].transform;
                //Debug.Log("foundtarget");
            }
        }
    }

    void ChooseNewRoom()
    {
        //dont use this for the first room
        m_lastRoomExplored = m_roomToExplore;
        while (true)
        {
            m_roomToExplore = m_hiveMind.m_HouseLocations[Random.Range(0, m_hiveMind.m_HouseLocations.Count)].position;
            if(m_roomToExplore != m_lastRoomExplored)
            {
                break;
            }
        }
        Vector3 pos = m_roomToExplore;
        pos.x += Random.Range(-1, 2);
        pos.z += Random.Range(-1, 2);
        m_roomToExplore = pos;
    }

    Vector3 RandPosAroundWall(Vector3 _v)
    {
        while (true)
        {
            Vector3 vcopy = _v;
            Vector2 v2 = Random.insideUnitCircle * 4;
            Vector3 v3 = new Vector3(Mathf.RoundToInt(v2.x), 0, Mathf.RoundToInt(v2.y));
            vcopy += v3;

            if (PathRequestManager.Instance().PositionIsWalkable(vcopy))
            {
                return vcopy;
            }
        }
    }
}