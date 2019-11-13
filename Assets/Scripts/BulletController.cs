using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private bool impact;
    private Rigidbody rigidbody;
    public float speed = 60f;
    public int bulletDamages = 10;
    public GameObject prefabImpact;
    private GameObject impactObj;
    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!impact)
        {
            rigidbody.velocity = transform.forward * speed;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag != "Player" && other.transform.tag != "CharacterModel")
        {
            if (!impact)
            {
                impact = true;
                impactObj = Instantiate(prefabImpact, this.transform.position, this.transform.rotation);
                Destroy(this.gameObject);
            }
        }
       
    }
}
