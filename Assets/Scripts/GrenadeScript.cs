using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{

    public GameObject explosionPrefab;
    public int damagesCenter = 45;
    public int damagesEdge = 25;
    private bool isInHand = true;
    private float radius = 2.5f;
    private Transform hand;
    private Transform character;
    private Vector3 directionForward;
    private Vector3 directionFUp;
    private int countdown = 3;
    private float throwForce = 600f;

    // Update is called once per frame
    void Update()
    {
        if (isInHand && hand != null) this.transform.position = hand.position;
    }


    public void Throw(Transform h, Transform c)
    {
        hand = h;
        character = c;
        directionForward = c.forward;
        directionFUp = c.up;
        StartCoroutine(GrenadeRoutine());
    }
    private IEnumerator GrenadeRoutine()
    {
        //animation 
        yield return new WaitForSeconds(1.25f);
        isInHand = false;
        // Throw physic
        ConstantForce gravity = this.gameObject.AddComponent<ConstantForce>();
        gravity.force = new Vector3(0.0f, -8.00f, 0.0f);
        this.GetComponent<Rigidbody>().AddForce(((directionForward * 0.4f) + (directionFUp * 0.4f)) * throwForce);
        yield return new WaitForSeconds(0.3f);
        this.transform.gameObject.layer = LayerMask.NameToLayer("Default");
        // blip blip
        Light l = this.GetComponentInChildren<Light>();
        for (int i = 0; i < countdown; i++)
        {
            l.enabled = true;
            SoundPlayer.Instance.Play("blip");
            yield return new WaitForSeconds(0.5f);
            l.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
        //boom
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = this.transform.position;
        SoundPlayer.Instance.Play("ExplosionGrenade");
        Explosion(explosion.transform.position);
        yield return new WaitForSeconds(0.5f);
        Destroy(explosion);
        Destroy(this.gameObject);
    }

    private void Explosion(Vector3 explosionPos)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.tag.Equals("Alien"))
            {
                Debug.Log("Explosion touched : " + hit.gameObject.name);
                float dist = Vector3.Distance(hit.transform.position, explosionPos);
                if (dist <= 1f)
                {
                    hit.gameObject.GetComponent<AlienCharacteristics>().TakeDamage(DamageSource.Grenade, damagesCenter);
                }
                else
                {
                    hit.gameObject.GetComponent<AlienCharacteristics>().TakeDamage(DamageSource.Grenade, damagesEdge);
                }
            }
            else if (hit.transform.parent != null && hit.transform.parent.tag.Equals("Player"))
            {
                Debug.Log("Explosion touched : " + hit.gameObject.name);
                float dist = Vector3.Distance(hit.transform.position, explosionPos);
                if (dist <= 1f)
                {
                    hit.transform.parent.gameObject.GetComponent<PlayerController>().TakeDamage(DamageSource.Grenade, damagesCenter);
                }
                else
                {
                    hit.transform.parent.gameObject.GetComponent<PlayerController>().TakeDamage(DamageSource.Grenade, damagesEdge);
                }
            }
        }
    }
}
