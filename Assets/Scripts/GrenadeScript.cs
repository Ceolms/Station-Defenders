using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public bool isInHand = true;
    public Transform hand;



    // Update is called once per frame
    void Update()
    {
        if (isInHand) this.transform.position = hand.position;
    }


    public void Throw()
    {

    }
    private IEnumerator Countdown()
    {
        yield return null;
    }
}
