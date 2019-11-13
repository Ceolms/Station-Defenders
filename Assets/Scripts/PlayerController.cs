using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerID
{
    Player1 = 0,
    Player2 = 1,
    Player3 = 2,
    Player4 = 3
}
public class PlayerController : MonoBehaviour
{
    //inputs vars
    public PlayerID id = PlayerID.Player1;
    private Player player;
    public bool useKeyboard = false;

    //movements vars
    private float moveSpeed = 250f;
    private float faintSpeed = 25f;
   
    private Vector3 moveVector;
    private Vector3 moveVelocity;
    private Quaternion rotation;
    private bool isMoving;
    private bool isFainting;
    private bool isShooting;
    private bool canMove = true;
    //GameObject vars

    public PlayerInfos infos;
    public Camera camera;
    private UiManager uiManager;
    private Rigidbody rigidbody;
    public Transform character;
    private Animator animator;
    public GameObject prefabBullet;
    public GameObject gun;
    public GameObject sphereMinimap;

    void Start()
    {
        player = ReInput.players.GetPlayer((int)id);
        rigidbody = GetComponent<Rigidbody>();
        camera = this.GetComponentInChildren<Camera>();
        uiManager = camera.transform.GetComponent<UiManager>();
        animator = character.GetComponent<Animator>();
        player.AddInputEventDelegate(OnFireButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Shoot");
        player.AddInputEventDelegate(OnMapButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Map");
        player.AddInputEventDelegate(OnEmoteButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Emote");
        uiManager.SetName(id);
        sphereMinimap.SetActive(true);
       // StartCoroutine(TestLifeBar());
    }

   
    void Update()
    {
        GetInput();
        ProcessInput();
        // animations
        if (moveVector.x == 0.0f && moveVector.z == 0.0f)
        {
            isMoving = false;
            animator.SetBool("isRunning", false);
        }
        else
        {
            isMoving = true;
            animator.SetBool("isRunning", true);
        }      
        if(infos.lifepoints <= 0)
        {
            isFainting = true;
            animator.SetBool("isFainting", true);
        }
    }

    //Controls scripts -----------------------
    private void OnFireButtonDown(InputActionEventData data)
    {
        Transform pos = null;

        foreach (Transform child in gun.transform)
        {
            if (child.name.Equals("GunShootPosition"))
            {
                pos = child;
                break;
            }
        }
        if(pos == null )
        {
            return;
        }
        GameObject bullet = Instantiate(prefabBullet, pos.position,Quaternion.identity);
        bullet.transform.forward = character.forward;
        bullet.GetComponent<BulletController>().canMove = true;
    }
    private void OnMapButtonDown(InputActionEventData data)
    {
        uiManager.TriggerMinimap();
    }
    private void OnEmoteButtonDown(InputActionEventData data)
    {
        animator.SetTrigger("isAskingHelp");
    }
  
    private void GetInput()
    {
        if(canMove)
        {
            moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
            moveVector.z = player.GetAxis("Move Vertical");
        }
    }
    private void ProcessInput()
    {
        // Process movement
        if (moveVector.x != 0.0f || moveVector.z != 0.0f)
        {
            if(!isFainting) moveVelocity = moveVector * moveSpeed * Time.deltaTime;
            else moveVelocity = moveVector * faintSpeed * Time.deltaTime;
        }
        if (isMoving) rigidbody.velocity = moveVelocity; else rigidbody.velocity = Vector3.zero;
        if (useKeyboard) RotationKeyboard();
        else RotationController();
    }
    private void RotationController()
    {
        float h1 = player.GetAxis("Look Horizontal"); 
        float v1 = player.GetAxis("Look Vertical");
        if (h1 == 0f && v1 == 0f)
        {
            Vector3 facingrotation = Vector3.Normalize(new Vector3(player.GetAxis("Move Horizontal"), 0f, player.GetAxis("Move Vertical")));
            if (facingrotation != Vector3.zero)//This condition prevents from spamming "Look rotation viewing vector is zero" when not moving.
                character.transform.forward = facingrotation;
        }
        else
        {
            character.transform.localEulerAngles = new Vector3(0f, Mathf.Atan2(h1, v1) * 180 / Mathf.PI, 0f); // this does the actual rotation according to inputs
        }
    }
    private void RotationKeyboard()
    {
        Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.red);
            character.rotation = Quaternion.LookRotation(pointToLook);
            //Debug.Log(character.rotation.y);        
        }
    }

    //Gameplay scripts -----------------------
    public void TakeDamage(int damages)
    {
        Debug.Log("Damages:" + damages);
        if(infos.lifepoints > 0)
        {
            infos.lifepoints -= damages;
            if (infos.lifepoints < 0) infos.lifepoints = 0;
            uiManager.SetLifebarSize(infos.lifepoints);
        }
    }
}
[System.Serializable]
public class PlayerInfos : System.Object
{
    public int lifepoints = 100;
}