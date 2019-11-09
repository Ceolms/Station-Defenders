using System.Collections;
using System.Collections.Generic;
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
      Debug.Log(this.healthPoint);
      if(this.healthPoint == 0)
      {
        alienAnimator.SetTrigger("Die");
        Destroy(this);
      }
    }
  }

  // Update is called once per frame
  void Update()
    {
        
    }
}
