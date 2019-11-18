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
    [HideInInspector] public Camera camera;
    [HideInInspector] public UiManager uiManager;
    private Rigidbody rigidbody;
    public Transform character;
    private Animator animator;

    public GameObject gun;
    public GameObject sphereMinimap;
    public GameObject prefabBullet;
    public GameObject grenadePrefab;
    private bool hitCooldown;
    private int grenadeCount = 300;

    //camera position
    private Vector3 offset;

    // Start is called before the first frame update
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
        player.AddInputEventDelegate(OnGrenadeButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Grenade");
        uiManager.SetName(id);
        sphereMinimap.SetActive(true);
        // StartCoroutine(TestLifeBar());

        //camera position
        offset = camera.transform.position - character.position;
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

        if (infos.lifepoints <= 0)
        {
            isFainting = true;
            animator.SetBool("isFainting", true);
        }
        else
        {
            isFainting = false;
            animator.SetBool("isFainting", false);
        }
    }


    void LateUpdate()
    {
        //Position camera to player
        camera.transform.position = character.transform.position + offset;
    }
    //Controls scripts -----------------------

    void OnFireButtonDown(InputActionEventData data)
    {
        if (canMove)
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
            if (pos == null)
            {
                return;
            }
            GameObject bullet = Instantiate(prefabBullet, pos.position, Quaternion.identity);
            bullet.transform.forward = character.forward;
            bullet.GetComponent<BulletController>().canMove = true;
        }

    }
    private void OnMapButtonDown(InputActionEventData data)
    {
        uiManager.TriggerMinimap();
    }
    private void OnEmoteButtonDown(InputActionEventData data)
    {
        if (canMove)
        {
            animator.SetBool("isRunning", false);
            moveVector = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            StartCoroutine(CanMoveRoutine(1f));
            animator.SetTrigger("isAskingHelp");
        }
    }
    private void OnGrenadeButtonDown(InputActionEventData data)
    {
        if (canMove && grenadeCount > 0)
        {
            GameObject grenade = Instantiate(grenadePrefab);
            grenade.GetComponent<GrenadeScript>().Throw(gun.transform.parent, character);
            animator.SetBool("isRunning", false);
            animator.SetTrigger("isThrowingGrenade");
            moveVector = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            StartCoroutine(CanMoveRoutine(1f));
            grenadeCount -= 1;
        }
    }

    private void GetInput()
    {
        if (canMove)
        {
            moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
            moveVector.z = player.GetAxis("Move Vertical");
        }
    }
    private void ProcessInput()
    {

        if (canMove)
        {
            if (moveVector.x != 0.0f || moveVector.z != 0.0f)
            {
                if (!isFainting) moveVelocity = moveVector * moveSpeed * Time.deltaTime;
                else moveVelocity = moveVector * faintSpeed * Time.deltaTime;
            }
            if (isMoving) rigidbody.velocity = moveVelocity; else rigidbody.velocity = Vector3.zero;
            if (useKeyboard) RotationKeyboard();
            else RotationController();
        }


    }
    private void RotationController()
    {
        if (canMove)
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

    }
    private void RotationKeyboard()
    {
        if (canMove)
        {
            RaycastHit hit;
            var layerMask = 1 << LayerMask.NameToLayer("MouseLayer");
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Vector3 pointToLook = hit.point;
                //Debug.DrawLine(ray.origin, pointToLook, Color.red);
                pointToLook.y = character.transform.position.y;
                character.transform.LookAt(pointToLook);
            }
        }
    }

    //Gameplay scripts -----------------------
    public void TakeDamage(float time, int damages)
    {
        if (!hitCooldown)
        {
            StartCoroutine(HitCooldownRoutine(time));
            if (infos.lifepoints > 0)
            {
                infos.lifepoints -= damages;
                if (infos.lifepoints < 0) infos.lifepoints = 0;
                uiManager.SetLifebarSize(infos.lifepoints);
                uiManager.ActiveDamageEffect();
            }
        }
    }

    private IEnumerator HitCooldownRoutine(float t)
    {
        hitCooldown = true;
        yield return new WaitForSeconds(t);
        hitCooldown = false;
    }
    private IEnumerator CanMoveRoutine(float t)
    {
        canMove = false;
        yield return new WaitForSeconds(t);
        canMove = true;
    }
}
[System.Serializable]
public class PlayerInfos : System.Object
{
    public int lifepoints = 100;
}