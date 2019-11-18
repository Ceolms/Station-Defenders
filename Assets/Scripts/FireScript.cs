using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public int damagesPerSeconds;
    //private SphereCollider collider;
    private bool fireActive;
    public Vector3 targetScale;
    public float scaleIncreaseSpeed = 0.2f;
    private float duration;
    // Start is called before the first frame update

    private void Start()
    {
     //   collider = this.GetComponent<SphereCollider>();
        this.transform.localScale = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (this.transform.localScale.x < targetScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleIncreaseSpeed * Time.deltaTime);
        }
    }

    private IEnumerator FireRoutine()
    {
        Color originalColor = Color.white;
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 10f);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Light l = hitColliders[i].GetComponent<Light>();
            if (l != null)
            {
                originalColor = l.color;
                l.color = Color.red;
            }
        }
        foreach (Transform particles in this.transform)
        {
            particles.gameObject.SetActive(true);
            fireActive = true;
        }
        yield return new WaitForSeconds(duration);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Light l = hitColliders[i].GetComponent<Light>();
            if (l != null)
            {
                l.color = originalColor;
            }
        }
    }

    public void StartFire(float duration)
    {
        this.duration = duration;
        StartCoroutine(FireRoutine());
    }

    private void OnTriggerStay(Collider collision)
    {
        
        if (fireActive)
        {
            if (collision.transform.parent.tag.Equals("Player"))
            {
                collision.transform.parent.GetComponent<PlayerController>().TakeDamage(DamageSource.Fire, damagesPerSeconds);
            }
            else if (collision.gameObject.tag.Equals("Alien"))
            {
                collision.transform.GetComponent<AlienCharacteristics>().TakeDamage(DamageSource.Fire, damagesPerSeconds);
            }
        }
    }
}
