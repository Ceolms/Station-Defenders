using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlienCharacteristics : MonoBehaviour
{
  public float MaxHealthPoint = 100;
  private float currentHealth;

  private Animator alienAnimator;

  public Slider healthbar;

  // Start is called before the first frame update
  void Start()
  {
    currentHealth = MaxHealthPoint;
    healthbar.value = calculateHealth();

    alienAnimator = this.transform.GetChild(0).GetComponent<Animator>();
    alienAnimator.SetTrigger("Smash Attack");
  }

  private float calculateHealth()
  {
    return currentHealth / MaxHealthPoint;
  }

  /*private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag.Equals("projectile"))
    {
      this.currentHealth--;
      if (this.currentHealth <= 0)
      {
        alienAnimator.SetTrigger("Die");
        Destroy(this.gameObject.GetComponent<BoxCollider>());
      }
    }
  }*/

  private IEnumerator DieAfterAnimation(float duree)
  {
    yield return new WaitForSeconds(duree - 0.15f);
    Destroy(this.gameObject);
  }

  public void TakeDamage(int damages)
  {
    if (currentHealth > 0)
    {
      currentHealth -= damages;
      calculateHealth();
      if (currentHealth < 0) currentHealth = 0;
    }
    if (currentHealth == 0) // if lifePoints <= 0
    {
      alienAnimator.SetTrigger("Die");
      Destroy(this.gameObject.GetComponent<BoxCollider>());

      var animController = alienAnimator.runtimeAnimatorController;
      var clip = animController.animationClips.First(a => a.name == "Die");

      StartCoroutine(DieAfterAnimation(clip.length));
    }
    //TODO faire apparaitre la barre de vie des aliens
  }
}
