using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float destroyAfterSeconds;
    void Start()
    {
        
    }

    // Update is called once per frame
    IEnumerable DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(this.gameObject);
    }
}
