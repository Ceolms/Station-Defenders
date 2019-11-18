using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Sync animation and damate
    public void AnimAttack()
    {
        GetComponentInParent<AlienController>().hit = true;
    }
}
