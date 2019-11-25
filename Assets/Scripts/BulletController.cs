using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private bool impact;
    public float speed = 1f;
    public int bulletDamages = 10;
    [HideInInspector] public bool canMove;
    public GameObject prefabImpact;
    private GameObject impactObj;
    [HideInInspector] public PlayerID owner;

    void Update()
    {
        if (!impact && canMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!impact)
        {
            impact = true;
            impactObj = Instantiate(prefabImpact, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
            if(other.gameObject.tag.Equals("Alien"))
            {
                other.transform.GetComponent<AlienCharacteristics>().TakeDamage(owner,bulletDamages);
            }
        }
    }
}
