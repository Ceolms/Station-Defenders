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

    private bool ShowHealth;

    // Start is called before the first frame update
    void Start()
    {
        alienMesh.GetComponent<SkinnedMeshRenderer>().material = new Material(material);
        currentHealth = maxHealthPoint;
        if (healthbar != null) healthbar.value = calculateHealth();

      ShowHealth = false;
      DisplayHealthBar();

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

    public void TakeDamage(float time, int damages)
    {
        //ShowHealth = true;
        StartCoroutine(Test());

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

    private IEnumerator Test()
    {
      ShowHealth = true;
      DisplayHealthBar();
      yield return new WaitForSeconds(0.5f);
      ShowHealth = false;
      DisplayHealthBar();
  }

  private void DisplayHealthBar()
  {
    healthbar.gameObject.SetActive(ShowHealth);
  }
}


