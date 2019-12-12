using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerScript : MonoBehaviour
{
    public GameObject character;
    public Animator anim;
    public Camera cam;
    bool followCam;
    bool playerMoveLeft;
    bool playerMoveRight;
    private float moveSpeed = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        anim = character.GetComponent<Animator>();
        StartCoroutine(RoutineTrailer());
    }

    // Update is called once per frame
    void Update()
    {
        if (followCam) cam.transform.position = new Vector3(character.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        if(playerMoveRight)
        {
            this.transform.position = new Vector3(transform.position.x + moveSpeed, transform.position.y, transform.position.z);
        }
    }

    IEnumerator RoutineTrailer()
    {
        playerMoveRight = true;
        yield return new WaitForSeconds(4.2f);
        followCam = true;
        yield return new WaitForSeconds(26.5f);
        followCam = false;
        yield return new WaitForSeconds(4.0f);
        playerMoveRight = false;
        yield return new WaitForSeconds(2.0f);
        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(0, 270, 0);
        this.transform.rotation = q;
    }
}
