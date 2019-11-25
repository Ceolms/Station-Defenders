using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlienCharacteristics : MonoBehaviour
{

    public float maxHealthPoint = 100;
    public int damages = 10;
    public int speed = 1;
    private float currentHealth;
    private bool hitCooldown;
    private Animator alienAnimator;
    private Quaternion initRotation;

    public Slider healthbar;
    public GameObject alienMesh;
    public Material material;
    public ScoreID scoreType;
    private bool ShowHealth;

    // Start is called before the first frame update
    void Start()
    {
        alienMesh.GetComponent<SkinnedMeshRenderer>().material = new Material(material);
        currentHealth = maxHealthPoint;
        if (healthbar != null) healthbar.value = calculateHealth();

      ShowHealth = false;
      healthbar.gameObject.SetActive(ShowHealth);

      initRotation = healthbar.transform.rotation;
      

        alienAnimator = this.transform.GetChild(0).GetComponent<Animator>();
    }

  private void LateUpdate()
  {
    healthbar.transform.rotation = initRotation;
  }

  private float calculateHealth()
    {
        return currentHealth / maxHealthPoint;
    }

    private IEnumerator DieAfterAnimation(float duree)
    {
        yield return new WaitForSeconds(duree - 0.15f);
        Destroy(this.gameObject);
    }

    public void TakeDamage(PlayerID id, int damages)
    {
        //ShowHealth = true;
        StartCoroutine(ShowHealthRoutine());

        float time = 0.1f;

        if (!hitCooldown)
        {
            StartCoroutine(HitCooldownRoutine(time));
            if (currentHealth > 0)
            {
                currentHealth -= damages;
                if (healthbar != null) healthbar.value = calculateHealth();
                if (currentHealth < 0) currentHealth = 0;
                StartCoroutine(HitEffectRoutine());
            }
            if (currentHealth == 0) // if lifePoints <= 0
            {
                switch(id)
                {
                    case (PlayerID.Player1):
                        GameManager.Instance.players[0].scoreList.Add(scoreType);
                        break;
                    case (PlayerID.Player2):
                        GameManager.Instance.players[1].scoreList.Add(scoreType);
                        break;
                    case (PlayerID.Player3):
                        GameManager.Instance.players[2].scoreList.Add(scoreType);
                        break;
                    case (PlayerID.Player4):
                        GameManager.Instance.players[3].scoreList.Add(scoreType);
                        break;
                }

                alienAnimator.SetTrigger("Die");
                Destroy(this.gameObject.GetComponent<BoxCollider>());
                var animController = alienAnimator.runtimeAnimatorController;
                var clip = animController.animationClips.First(a => a.name == "Die");
                StartCoroutine(DieAfterAnimation(clip.length));
            }
        }
    }
    private IEnumerator HitCooldownRoutine(float t)
    {
        hitCooldown = true;
        yield return new WaitForSeconds(t);
        hitCooldown = false;
    }
    private IEnumerator HitEffectRoutine()
    {
        alienMesh.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        alienMesh.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
    }

    private IEnumerator ShowHealthRoutine()
    {
      ShowHealth = true;
        healthbar.gameObject.SetActive(ShowHealth);
        yield return new WaitForSeconds(0.5f);
      ShowHealth = false;
        healthbar.gameObject.SetActive(ShowHealth);
    }
}


