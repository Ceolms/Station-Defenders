using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    [HideInInspector]
    public bool isMoving;
    [HideInInspector]
    public bool isFainting;
    private bool isShooting;
    private bool canMove = true;
    private bool isHealing;
    private Transform facingDirection;
    //GameObject vars

    public PlayerInfos infos;
    [HideInInspector] public Camera camera;
    [HideInInspector] public UiManager uiManager;
    private Rigidbody rigidbody;
    public Transform character;
    [HideInInspector]
    public Animator animator;

    public GameObject gun;
    public GameObject sphereMinimap;
    public GameObject prefabBullet;
    public GameObject grenadePrefab;
    public GameObject healParticlePrefab;
    private GameObject healParticle;
    private Transform shootPositon;
    private bool hitCooldown;
    public int score;
    public List<ScoreID> scoreList;

    //camera position
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer((int)id);
        rigidbody = GetComponent<Rigidbody>();
        camera = this.GetComponentInChildren<Camera>();
        infos.lifepoints = infos.maxLifepoints;
        uiManager = camera.transform.GetComponent<UiManager>();
        animator = character.GetComponent<Animator>();
        player.AddInputEventDelegate(OnFireButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Shoot");
        player.AddInputEventDelegate(OnMapButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Map");
        player.AddInputEventDelegate(OnEmoteButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Emote");
        player.AddInputEventDelegate(OnGrenadeButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Grenade");
        player.AddInputEventDelegate(OnHealButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Heal");
        player.AddInputEventDelegate(OnHealButtonUp, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Heal");
        player.AddInputEventDelegate(OnValidateButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Validate");
        uiManager.SetName(id);
        sphereMinimap.SetActive(true);
        scoreList = new List<ScoreID>();
        // StartCoroutine(TestLifeBar());
        uiManager.owner = id;
        //camera position
        offset = camera.transform.position - character.position;
        uiManager.SetGrenadeCount(infos.grenadeCount);

        foreach (Transform child in gun.transform)
        {
            if (child.name.Equals("GunShootPosition"))
            {
                shootPositon = child;
                break;
            }
        }
        foreach (Transform child in this.transform)
        {
            if (child.name.Equals("FacingDirection"))
            {
                facingDirection = child;
                break;
            }
        }
    }


    void Update()
    {
        Debug.DrawRay(facingDirection.position, facingDirection.forward);
        if (GameManager.Instance.gameRunning)
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
            if (isHealing)
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 5.2f);
                foreach (Collider hit in colliders)
                {
                    if (hit.transform.parent != null && hit.transform.parent.tag.Equals("Player"))
                    {

                        PlayerController p = hit.transform.parent.gameObject.GetComponent<PlayerController>();
                        if (p.isFainting)
                        {
                            p.Heal(infos.healPerFrame, id);
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameRunning)
        {
            if (isMoving) rigidbody.velocity = moveVelocity; else rigidbody.velocity = Vector3.zero;
            if (useKeyboard) RotationKeyboard();
            else RotationController();

            if (infos.lifepoints <= 0)
            {
                character.gameObject.layer = LayerMask.NameToLayer("Faint");
            }
            else
            {
                character.gameObject.layer = LayerMask.NameToLayer("Movings");
            }
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

        if (canMove && GameManager.Instance.gameRunning)
        {
            GameObject bullet = Instantiate(prefabBullet, shootPositon.position, Quaternion.identity);
            bullet.transform.forward = character.forward;
            bullet.GetComponent<BulletController>().canMove = true;
            bullet.GetComponent<BulletController>().owner = id;
        }
        else
        {
            Debug.Log("can't move");
        }

    }
    private void OnMapButtonDown(InputActionEventData data)
    {
        if (GameManager.Instance.gameRunning) uiManager.TriggerMinimap();
    }
    private void OnEmoteButtonDown(InputActionEventData data)
    {
        if (canMove && GameManager.Instance.gameRunning)
        {
            animator.SetBool("isRunning", false);
            moveVector = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            StartCoroutine(CanMoveRoutine(1f));
            animator.SetTrigger("isAskingHelp");
        }
    }
    private void OnHealButtonDown(InputActionEventData data)
    {
        if (GameManager.Instance.gameRunning)
        {
            animator.SetBool("isRunning", false);
            moveVector = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            canMove = false;
            isHealing = true;
            animator.SetBool("isHealing", true);
            animator.SetTrigger("beginHeal");
            healParticle = GameObject.Instantiate(healParticlePrefab);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            healParticle.transform.position = pos;
        }
    }
    private void OnHealButtonUp(InputActionEventData data)
    {
        if (GameManager.Instance.gameRunning)
        {
            animator.SetBool("isHealing", false);
            isHealing = false;
            StartCoroutine(CanMoveRoutine(1f));
            Destroy(healParticle);
            healParticle = null;
        }
    }
    private void OnGrenadeButtonDown(InputActionEventData data)
    {
        if (canMove && GameManager.Instance.gameRunning && infos.grenadeCount > 0)
        {
            GameObject grenade = Instantiate(grenadePrefab);
            grenade.GetComponent<GrenadeScript>().Throw(gun.transform.parent, facingDirection);
            grenade.GetComponent<GrenadeScript>().owner = id;
            animator.SetBool("isRunning", false);
            animator.SetTrigger("isThrowingGrenade");
            moveVector = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            StartCoroutine(CanMoveRoutine(1f));
            infos.grenadeCount -= 1;
            uiManager.SetGrenadeCount(infos.grenadeCount);
        }
    }
    private void OnValidateButtonDown(InputActionEventData data)
    {
        if (!GameManager.Instance.gameRunning)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void GetInput()
    {
        if (canMove)
        {
            moveVector.x = player.GetAxis("Move Horizontal");
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
                {
                    character.transform.forward = facingrotation;
                    facingDirection.forward = facingrotation;
                }
            }
            else
            {
                character.transform.localEulerAngles = new Vector3(0f, Mathf.Atan2(h1, v1) * 180 / Mathf.PI, 0f); // this does the actual rotation according to inputs
                facingDirection.localEulerAngles = new Vector3(0f, Mathf.Atan2(h1, v1) * 180 / Mathf.PI, 0f);
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
                facingDirection.LookAt(pointToLook);
            }
        }
    }

    //Gameplay scripts -----------------------
    public void TakeDamage(DamageSource source, int damages)
    {
        float time = 0.3f;
        if (!hitCooldown)
        {
            StartCoroutine(HitCooldownRoutine(time));
            if (infos.lifepoints > 0)
            {
                infos.lifepoints -= damages;
                if (infos.lifepoints < 0) infos.lifepoints = 0;
                uiManager.SetLifebarSize((int)infos.lifepoints);
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

    public void Heal(float amount, PlayerID id)
    {

        this.infos.lifepoints += amount;
        Debug.Log("healing : " + infos.lifepoints);
        uiManager.SetLifebarSize((int)infos.lifepoints);
        if (infos.lifepoints >= infos.maxLifepoints)
        {
            infos.lifepoints = infos.maxLifepoints;
            isFainting = false;
            animator.SetBool("isFainting", false);
            switch (id)
            {
                case (PlayerID.Player1):
                    GameManager.Instance.players[0].scoreList.Add(ScoreID.resurrection);
                    break;
                case (PlayerID.Player2):
                    GameManager.Instance.players[1].scoreList.Add(ScoreID.resurrection);
                    break;
                case (PlayerID.Player3):
                    GameManager.Instance.players[2].scoreList.Add(ScoreID.resurrection);
                    break;
                case (PlayerID.Player4):
                    GameManager.Instance.players[3].scoreList.Add(ScoreID.resurrection);
                    break;
            }
        }
    }
}
[System.Serializable]
public class PlayerInfos : System.Object
{
    public float lifepoints;
    public float maxLifepoints = 100f;
    public int grenadeCount = 100;
    [HideInInspector] public float healPerFrame = 0.2f;
}