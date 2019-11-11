using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float destroyAfterSeconds = 2f;
    void Start()
    {
        StartCoroutine(DestroyAfterAnimation());
    }

    // Update is called once per frame
    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(this.gameObject);
    }
}
