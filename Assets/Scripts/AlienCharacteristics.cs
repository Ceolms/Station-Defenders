using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlienCharacteristics : MonoBehaviour
{
    public int healthPoint = 10;
    private Animator alienAnimator;

    // Start is called before the first frame update
    void Start()
    {
      alienAnimator = this.transform.GetChild(0).GetComponent<Animator>();
      alienAnimator.SetTrigger("Smash Attack");
    }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag.Equals("projectile"))
    {
      this.healthPoint--;
      if(this.healthPoint <= 0)
      {
        alienAnimator.SetTrigger("Die");
        Destroy(this.gameObject.GetComponent<BoxCollider>());

        var animController = alienAnimator.runtimeAnimatorController;
        var clip = animController.animationClips.First(a => a.name == "Die");

        StartCoroutine(DieAfterAnimation(clip.length));

      }
    }
  }

  private IEnumerator DieAfterAnimation(float duree)
  {
    yield return new WaitForSeconds(duree - 0.15f);
    Destroy(this.gameObject);
  }

}
