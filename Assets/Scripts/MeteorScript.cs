using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    private ParticleSystem pSystem;
    private ParticleCollisionEvent[] collisionEvents;
    private float soundRadius = 8f;

    // Start is called before the first frame update
    void Start()
    {
        pSystem = this.GetComponent<ParticleSystem>();
        collisionEvents = new ParticleCollisionEvent[8];
        StartCoroutine(SoundRoutine());
    }
 

    public void OnParticleCollision(GameObject other)
    {
        Collider[] collidersSound = Physics.OverlapSphere(this.transform.position, soundRadius);
        foreach (Collider hit in collidersSound)
        {
            if (hit.transform.parent.tag.Equals("Player"))
            {
                SoundPlayer.Instance.Play("ExplosionMeteor");
                break;
            }
        }

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
        // play boom
        Collider[] colliders = Physics.OverlapSphere(explosionPos, 2.5f);

        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.tag.Equals("Player") || hit.gameObject.tag.Equals("Alien"))
            {
                Debug.Log("Explosion touched : " + hit.gameObject.name);
                float dist = Vector3.Distance(hit.transform.position, explosionPos);
                if (dist <= 1f)
                {
                    if (hit.gameObject.tag.Equals("Player"))
                        hit.gameObject.GetComponent<PlayerController>().TakeDamage(DamageSource.Meteor, EventManager.Instance.eventMeteor.damagesCenter);
                    else if (hit.gameObject.tag.Equals("Alien"))
                        hit.gameObject.GetComponent<AlienCharacteristics>().TakeDamage(PlayerID.NotPlayer, EventManager.Instance.eventMeteor.damagesCenter);
                }
                else
                {
                    if (hit.gameObject.tag.Equals("Player"))
                        hit.gameObject.GetComponent<PlayerController>().TakeDamage(DamageSource.Meteor, EventManager.Instance.eventMeteor.damagesBorder);
                    else if (hit.gameObject.tag.Equals("Alien"))
                        hit.gameObject.GetComponent<AlienCharacteristics>().TakeDamage(PlayerID.NotPlayer, EventManager.Instance.eventMeteor.damagesBorder);
                }
            }
        }
    }
    IEnumerator SoundRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        // play sound only if a player can hear it
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, soundRadius);
        foreach (Collider hit in colliders)
        {
            if(hit.transform.parent != null)
            {
                if (hit.transform.parent.tag.Equals("Player"))
                {
                    SoundPlayer.Instance.Play("SwooshMeteor");
                    break;
                }
            }
        }
    }
}
