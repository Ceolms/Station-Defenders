using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private bool routineRandom;
    private float r = 0;
    GameObject[] lights;
    private bool lightEventActive;
    private float lightdecreaseSpeed = 0.05f;

    public Texture gunNormal;
    public Texture gunDark;

    // Start is called before the first frame update
    void Start()
    {
        if (lights == null)
            lights = GameObject.FindGameObjectsWithTag("Light");
    }

    // Update is called once per frame
    void Update()
    {

        if(!routineRandom)
        {
            StartCoroutine(TestRandom());
        }
        if(r > 3f && !lightEventActive)
        {
            LightEvent();
        }


        if (lightEventActive)
        {
            foreach (GameObject l in lights)
            {
                Light light = l.GetComponent<Light>();
                light.intensity = light.intensity - lightdecreaseSpeed;
            }
            if (lights[0].GetComponent<Light>().intensity < 0.2f) { }
        }
    }


    private IEnumerator TestRandom()
    {
        routineRandom = true;
        yield return new WaitForSeconds(10f);
        r = Random.Range(0f, 10.0f);
        Debug.Log(r);
        routineRandom = false;
    }
    private void LightEvent()
    {
        lightEventActive = true;
        StartCoroutine(LightBackActive());
    }
    private IEnumerator LightBackActive()
    {
        yield return new WaitForSeconds(30f);
        foreach (GameObject l in lights)
        {
            Light light = l.GetComponent<Light>();
            light.intensity = 1.33f;
        }
    }
}
