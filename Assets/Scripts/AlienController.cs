using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienController : MonoBehaviour
{

    //Agent
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Vector3 currentPosition;

    //Targets
    private GameObject bestTarget;
    private bool targetIsPlayer;
    private GameObject[] players;
    private GameObject core;


    //States
    private bool isMoving;
    private bool isAttacking;

    //Animator
    private Animator anim;
    [HideInInspector]
    public bool hit;
    private int damages;

    // Start is called before the first frame update
    void Start()
    {
        //Get navMeshAgent
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();

        //Get animator from child
        anim = GetComponentInChildren<Animator>();

        //Get all players
        players = GameObject.FindGameObjectsWithTag("Player");
        //Get core
        core = GameObject.FindGameObjectWithTag("Core");
        damages = this.GetComponent<AlienCharacteristics>().damages;
    }

    void FixedUpdate()
    {
        GetClosestEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gameRunning)
        {
            if (bestTarget != null) agent.SetDestination(bestTarget.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = false;
            }
            else
            {
                isMoving = true;
            }

            if (isMoving)
            {
                anim.SetBool("Walk Forward", true);

            }
            else
            {
                anim.SetBool("Walk Forward", false);
                agent.velocity = Vector3.zero;
                if (targetIsPlayer)
                {
                    AttackPlayer();
                }
                else
                {
                    AttackCore();
                }
            }
        }
    }

    private void GetClosestEnemy()
    {
        currentPosition = transform.position;

        bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        Vector3 directionToTarget;
        float dSqrToTarget;

        //Nearest player
        foreach (GameObject player in players)
        {
            //If player is not faint
            if(player.GetComponent<PlayerController>().infos.lifepoints > 0)
            {
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
        //Check if core is nearest than players
        directionToTarget = core.transform.position - currentPosition;
        dSqrToTarget = directionToTarget.sqrMagnitude;
        if(dSqrToTarget < closestDistanceSqr)
        {
            bestTarget = core;
            agent.stoppingDistance = 3;
            targetIsPlayer = false;
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
            bestTarget.GetComponent<PlayerController>().TakeDamage(DamageSource.Alien,damages);
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
            core.GetComponent<Core>().TakeDamage(damages);
        }
    }

    IEnumerator animAttackDelay()
    {
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(1.5f);
        isAttacking = false;

    }
    

    
}
