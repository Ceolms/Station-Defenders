using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienController : MonoBehaviour
{

    public enum Priority
    {
        CORE,
        PLAYERS,
    }

    //Constants
    private static float CORE_PRIORITY = 0.7f;
    private static float PLAYERS_PRIORITY = 0.3f;

    //Agent
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Vector3 currentPosition;

    //Targets
    private GameObject bestTarget;
    private Vector3 oldPosCore;
    private Vector3 newPosCore;
    private bool targetIsPlayer;
    private GameObject[] players;
    private GameObject core;
    private NavMeshHit navHit;


    //States
    private bool isMoving;
    private bool isAttacking;
    private bool isDead;
    private bool hasCollision;
    private bool isPlayingDeadAnim;
    private bool changeTarget;
    private GameObject pCollision;
    public Priority priority;

    //Animator
    private Animator anim;
    [HideInInspector]
    public bool hit;

    //Characteristics
    AlienCharacteristics ac;

    // Start is called before the first frame update
    void Start()
    {

        //Get navMeshAgent
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();

        //Get characteristics
        ac = GetComponent<AlienCharacteristics>();

        //Set agent speed
        agent.speed = ac.speed;
        agent.acceleration = ac.speed;
        isMoving = true;

        //Get animator from child
        anim = GetComponentInChildren<Animator>();

        //Get all players
        players = GameObject.FindGameObjectsWithTag("Player");
        //Get core
        core = GameObject.FindGameObjectWithTag("Core");

        oldPosCore = new Vector3(0, 0, 0);

        //Priority
        float rdm = Random.Range(0f, 1f);

        if (rdm <= CORE_PRIORITY)
        {
            priority = Priority.CORE;
        }
        else
        {
            priority = Priority.PLAYERS;
        }


    }

    void FixedUpdate()
    {
        if (agent.enabled) GetClosestEnemy();
        oldPosCore = navHit.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (ac.currentHealth <= 0)
        {
            isMoving = false;
            isDead = true;
        }



        if (agent.enabled && bestTarget != null && !isDead)
        {


            if (targetIsPlayer)
            {
                agent.SetDestination(bestTarget.transform.position);
            }
            else
            {

                NavMesh.SamplePosition(bestTarget.transform.position, out navHit, 3.0f, NavMesh.AllAreas);

                if (oldPosCore != navHit.position)
                {
                    agent.SetDestination(navHit.position);
                }


            }



        }


        if (!isDead)
        {
            if (targetIsPlayer)
            {
                if (agent.enabled && bestTarget != null && Vector3.Distance(transform.position, bestTarget.transform.position) <= agent.stoppingDistance)
                {
                    isMoving = false;
                    agent.enabled = false;
                    obstacle.enabled = true;
                }

                if (!agent.enabled && Vector3.Distance(transform.position, bestTarget.transform.position) > agent.stoppingDistance)
                {
                    StartCoroutine(SwitchObstacleAgent());
                    isMoving = true;
                }
            }
            else
            {
                if (agent.enabled && bestTarget != null && Vector3.Distance(transform.position, navHit.position) <= 1)
                {
                    isMoving = false;
                    agent.enabled = false;
                    obstacle.enabled = true;
                }

                if (!agent.enabled && Vector3.Distance(transform.position, navHit.position) > 1)
                {
                    StartCoroutine(SwitchObstacleAgent());
                    isMoving = true;
                }
            }

        }
        else
        {
            if (agent.enabled)
            {
                agent.velocity = Vector3.zero;
                agent.speed = 0;
                agent.isStopped = true;
                agent.enabled = false;
                obstacle.enabled = false;
            }

            if (!isPlayingDeadAnim)
            {
                isPlayingDeadAnim = true;
                anim.SetTrigger("Die");
            }
        }


        // Moving
        if (isMoving)
        {
            anim.SetBool("Walk Forward", true);
        }
        else if (!isMoving && !isDead)
        {
            anim.SetBool("Walk Forward", false);

            if (agent.enabled) agent.velocity = Vector3.zero;

        }

        //Attacking
        if (targetIsPlayer && Vector3.Distance(transform.position, bestTarget.transform.position) <= agent.stoppingDistance)
        {
            AttackPlayer();
        }

        if (!targetIsPlayer && Vector3.Distance(transform.position, navHit.position) <= 1)
        {
            AttackCore();
        }




    }

    IEnumerator SwitchObstacleAgent()
    {
        obstacle.enabled = false;
        yield return new WaitForSeconds(0.5f);
        agent.enabled = true;
    }

    private void GetClosestEnemy()
    {
        if (priority.Equals(Priority.CORE))
        {
            bestTarget = core;
            agent.stoppingDistance = 1f;
            targetIsPlayer = false;

        }

        else if (priority.Equals(Priority.PLAYERS))
        {
            currentPosition = transform.position;

            bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;

            Vector3 directionToTarget;
            float dSqrToTarget;

            bool AllPlayersFaint = true;
            bool UnreachablePlayers = true;


            //Nearest player
            foreach (GameObject player in players)
            {
                //If player is not faint
                if (player.GetComponent<PlayerController>().infos.lifepoints > 0)
                {
                    NavMeshPath path = new NavMeshPath();
                    agent.CalculatePath(player.transform.position, path);

                    if (path.status != NavMeshPathStatus.PathComplete)
                    {
                        continue;
                    }
                    else
                    {
                        UnreachablePlayers = false;
                    }

                    AllPlayersFaint = false;
                    directionToTarget = player.transform.position - currentPosition;
                    dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;

                        bestTarget = player;

                    }
                }


            }
            targetIsPlayer = true;
            agent.stoppingDistance = 2;

            //Core
            //Check if all players are faint
            if (AllPlayersFaint || UnreachablePlayers)
            {
                bestTarget = core;
                agent.stoppingDistance = 1f;
                targetIsPlayer = false;
            }
        }

    }

    public void AttackPlayer()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            StartCoroutine(animAttackDelay());

        }

        //Sync damage and animation
        if (hit && isAttacking)
        {
            bestTarget.GetComponent<PlayerController>().TakeDamage(DamageSource.Alien, ac.damages);
            hit = false;
        }


    }

    public void AttackCore()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            StartCoroutine(animAttackDelay());

            // Function - Attack the core 
            // *** //
            core.GetComponent<Core>().TakeDamage(ac.damages);


        }


    }

    IEnumerator animAttackDelay()
    {
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(1.5f);
        isAttacking = false;

    }




}