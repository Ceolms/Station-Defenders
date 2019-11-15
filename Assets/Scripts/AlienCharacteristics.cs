using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlienCharacteristics : MonoBehaviour
{
    public int lifePoints = 100;
    private Animator alienAnimator;

    // Start is called before the first frame update
    void Start()
    {
        alienAnimator = this.transform.GetChild(0).GetComponent<Animator>();
        alienAnimator.SetTrigger("Smash Attack");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Projectile"))
        {
            TakeDamage(collision.gameObject.GetComponent<BulletController>().bulletDamages);
        }
    }

    private IEnumerator DieAfterAnimation(float duree)
    {
        yield return new WaitForSeconds(duree - 0.15f);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int damages)
    {
        if (lifePoints > 0)
        {
            lifePoints -= damages;
            if (lifePoints < 0) lifePoints = 0;
        }
        else // if lifePoints <= 0
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
