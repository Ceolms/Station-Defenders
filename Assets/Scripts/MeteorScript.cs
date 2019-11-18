using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    private ParticleSystem pSystem;
    private ParticleCollisionEvent[] collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        pSystem = this.GetComponent<ParticleSystem>();
        collisionEvents = new ParticleCollisionEvent[8];
    }

    public void OnParticleCollision(GameObject other)
    {
        int collCount = pSystem.GetSafeCollisionEventSize();

        if (collCount > collisionEvents.Length)
            collisionEvents = new ParticleCollisionEvent[collCount];

        int eventCount = pSystem.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < eventCount; i++)
        {
            Explosions(collisionEvents[i].intersection);
        }
    }

    void Explosions(Vector3 explosionPos)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 2.5f);
        foreach (Collider hit in colliders)
        {
            if(hit.gameObject.tag.Equals("Player") || hit.gameObject.tag.Equals("Alien"))
            {
                Debug.Log("Explosion touched : " + hit.gameObject.name);
                float dist = Vector3.Distance(hit.transform.position, explosionPos);
                if(dist <= 1f)
                {
                    if (hit.gameObject.tag.Equals("Player"))
                        hit.gameObject.GetComponent<PlayerController>().TakeDamage(DamageSource.Meteor,EventManager.Instance.eventMeteor.damagesCenter);
                    else if (hit.gameObject.tag.Equals("Alien"))
                        hit.gameObject.GetComponent<AlienCharacteristics>().TakeDamage(DamageSource.Meteor, EventManager.Instance.eventMeteor.damagesCenter);
                }
                else
                {
                    if (hit.gameObject.tag.Equals("Player"))
                        hit.gameObject.GetComponent<PlayerController>().TakeDamage(DamageSource.Meteor, EventManager.Instance.eventMeteor.damagesBorder);
                    else if (hit.gameObject.tag.Equals("Alien"))
                        hit.gameObject.GetComponent<AlienCharacteristics>().TakeDamage(DamageSource.Meteor, EventManager.Instance.eventMeteor.damagesBorder);
                }
            }
        }
    }
}
