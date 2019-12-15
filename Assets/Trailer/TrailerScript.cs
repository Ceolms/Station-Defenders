using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TrailerScript : MonoBehaviour
{
    public GameObject character;
    public Animator anim;
    public Camera cam;
    public GameObject rifle;
    private Animator animCam;
    bool followCam;
    bool playerMoveLeft;
    bool playerMoveRight;
    private float moveSpeed = 0.02f;
    int p = 0;
    public List<Transform> points = new List<Transform>();
    NavMeshAgent agent;
    bool movementdone;
    // Start is called before the first frame update
    void Start()
    {
        anim = character.GetComponent<Animator>();
        StartCoroutine(RoutineTrailer());
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.updateRotation = false;
        GotoPoint(p);
        anim.SetBool("walk", true);
        animCam = cam.gameObject.GetComponent<Animator>();
        animCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (followCam)
        {
            cam.transform.position = new Vector3(character.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        }
        if (!agent.pathPending && agent.remainingDistance < 0.03f)
        {
            p += 1;
            GotoPoint(p);
        }
        if(movementdone)
        {
            Quaternion q = new Quaternion();
            q.eulerAngles = new Vector3(0, 90, 0);
            this.transform.rotation = q;
        }
        
    }
    IEnumerator RoutineTrailer()
    {
        yield return new WaitForSeconds(3.4f);
        followCam = true;

        yield return new WaitForSeconds(25.0f);
        followCam = false;
        Debug.Log("cam stop");
        GameObject.Find("SoldierTrailer4").SetActive(false);
        agent.isStopped = true;
        anim.enabled = false;

        yield return new WaitForSeconds(1.6f);
        p += 1;
        agent.isStopped = false;
        GotoPoint(p);
        anim.SetBool("walk", false);
        anim.SetBool("run", true);
        anim.enabled = true;
        agent.speed = 2;

        yield return new WaitForSeconds(0.8f);
        agent.isStopped = true;
        Debug.Log("Stop and look");
        anim.SetTrigger("gun");
        movementdone = true;
        rifle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        animCam.enabled = true;
        animCam.SetBool("animation",true);
    }

    void GotoPoint(int i)
    {
        if (i < points.Count)
        {
            agent.destination = points[i].position;
            this.transform.LookAt(points[i]);
        }
    }
}

