using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum EventType
{
    None,
    LightBreakdown,
    MeteorShower,
    Fire
}
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    private bool canEventHappen = true;
    private bool randomActive;
    private List<EventType> eventTypeProba;
    [HideInInspector] public bool eventActive;

    public EventLightInfos eventLightBreakdown;
    public EventMeteorInfos eventMeteor;
    public EventFireInfos eventFire;

    private EventLight eventLScript;
    private EventMeteor eventMScript;
    private EventFire eventFScript;

    public Material matCircle;

    [HideInInspector]
    public List<Light> lights = new List<Light>();
    [HideInInspector]
    public List<Light> playersLights = new List<Light>();
    [HideInInspector]
    public Color originalColor;
    [HideInInspector]
    public float originalIntensity;


    [Tooltip("in percent up to 100% ( always happen)")]
    [Range(0, 100)]
    public float eventRisk = 0;
    public float timeBetweenCheck = 1f;
    public float coolDown;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        eventTypeProba = new List<EventType>();
        for (int i = 0; i < eventLightBreakdown.eventTypeRisk; i++) eventTypeProba.Add(EventType.LightBreakdown);
        for (int i = 0; i < eventMeteor.eventTypeRisk; i++) eventTypeProba.Add(EventType.MeteorShower);
        for (int i = 0; i < eventFire.eventTypeRisk; i++) eventTypeProba.Add(EventType.Fire);

        GameObject[] gosL = GameObject.FindGameObjectsWithTag("Light");

        foreach (GameObject go in gosL)
        {
            lights.Add(go.GetComponent<Light>());
        }


        if (lights.Count > 0)
        {
            originalColor = lights[0].color;
            originalIntensity = lights[0].intensity;
        }
        gosL = GameObject.FindGameObjectsWithTag("PlayerLight");
        foreach (GameObject go in gosL)
        {
            playersLights.Add(go.GetComponent<Light>());
        }

        if (this.GetComponent<EventLight>() == null) eventLScript = this.gameObject.AddComponent<EventLight>();
        if (this.GetComponent<EventMeteor>() == null) eventMScript = this.gameObject.AddComponent<EventMeteor>();
        if (this.GetComponent<EventFire>() == null) eventFScript = this.gameObject.AddComponent<EventFire>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!eventActive && canEventHappen && !randomActive)
        {
            StartCoroutine(TestRandom());
        }
    }

    private IEnumerator TestRandom()
    {
        randomActive = true;
        if (eventTypeProba.Count > 0)
        {
            while (!eventActive)
            {
                yield return new WaitForSeconds(timeBetweenCheck);
                int i = Random.Range(0, 100);
                //  Debug.Log("TestRandom :" + i);
                if (i < eventRisk) // something happen !
                {
                    int j = Random.Range(0, eventTypeProba.Count);
                    EventType evt = eventTypeProba[j];
                    //   Debug.Log("EventType:" + evt);
                    switch (evt)
                    {
                        case (EventType.LightBreakdown):
                            eventLScript.StartEvent();
                            break;
                        case (EventType.MeteorShower):
                            eventMScript.StartEvent();
                            break;
                        case (EventType.Fire):
                            eventFScript.StartEvent();
                            break;
                    }
                }
            }
        }
        randomActive = false;
        yield return null;
    }
}

public class EventLight : MonoBehaviour
{
    EventLightInfos infos;


    private float lightdecreaseSpeed = 0.05f;

    public void Start()
    {
        infos = EventManager.Instance.eventLightBreakdown;
    }

    public void StartEvent()
    {
        EventManager.Instance.eventActive = true;
        infos.eventActive = true;
        StartCoroutine(LightBackActive());
        SoundPlayer.Instance.Play("WarningLight");
        Debug.Log("LightBreakDown event begin!");
    }
    public void Update()
    {
        if (infos.eventActive)
        {
            foreach (Light l in EventManager.Instance.lights)
            {
                l.intensity = l.intensity - lightdecreaseSpeed;
            }
            if (EventManager.Instance.lights[0].GetComponent<Light>().intensity < 0.5f)
            {
                foreach (Light l in EventManager.Instance.lights)
                {
                    l.enabled = true;
                }
            }
        }
    }
    private IEnumerator LightBackActive()
    {
        yield return new WaitForSeconds(infos.eventDuration);
        foreach (Light l in EventManager.Instance.lights)
        {
            l.intensity = EventManager.Instance.originalIntensity;
        }
        EventManager.Instance.eventActive = false;
        infos.eventActive = false;
        Debug.Log("LightBreakDown event finish!");
        foreach (Light l in EventManager.Instance.lights)
        {
            l.enabled = false;
        }
    }
}

public class EventMeteor : MonoBehaviour
{
    EventMeteorInfos infos;
    GameObject[] listMeteorAreas;
    private bool routineActive;
    public void Start()
    {
        infos = EventManager.Instance.eventMeteor;
        listMeteorAreas = GameObject.FindGameObjectsWithTag("MeteorArea");
    }
    public void StartEvent()
    {
        if (listMeteorAreas.Length > 0)
        {
            EventManager.Instance.eventActive = true;
            infos.eventActive = true;
            StartCoroutine(RedAlertLight());
            SoundPlayer.Instance.Play("RedAlert");
            SoundPlayer.Instance.Play("WarningMeteor");
            Debug.Log("Meteor Event Start!");
        }
        else Debug.Log("No Meteor Area found.");
    }

    public void Update()
    {
        if (infos.eventActive)
        {
            if (!routineActive)
            {
                StartCoroutine(SpawnMeteors());
            }
        }
    }

    private IEnumerator SpawnMeteors()
    {
        routineActive = true;
        for (int i = 0; i < infos.meteorAmount; i++)
        {
            yield return new WaitForSeconds(infos.timeBetweenMeteor);

            int indexArea = Random.Range(0, listMeteorAreas.Length);
            float minX = listMeteorAreas[indexArea].transform.position.x - (listMeteorAreas[indexArea].transform.lossyScale.x / 2);
            float maxX = listMeteorAreas[indexArea].transform.position.x + (listMeteorAreas[indexArea].transform.lossyScale.x / 2);

            float minZ = listMeteorAreas[indexArea].transform.position.z - (listMeteorAreas[indexArea].transform.lossyScale.z / 2);
            float maxZ = listMeteorAreas[indexArea].transform.position.z + (listMeteorAreas[indexArea].transform.lossyScale.z / 2);
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            SpawnMeteor(new Vector3(x, 0, z));
        }
        infos.eventActive = false;
        EventManager.Instance.eventActive = false;
        routineActive = false;
        Debug.Log("Meteor Event End!");
    }

    private void SpawnMeteor(Vector3 position)
    {
        StartCoroutine(SpawnMeteorRoutine(position));
    }
    private IEnumerator SpawnMeteorRoutine(Vector3 position)
    {
        GameObject meteor = Instantiate(infos.meteorPrefab);
        meteor.transform.position = position;
        yield return new WaitForSeconds(4f);
        Destroy(meteor);
    }
    private IEnumerator RedAlertLight()
    {
        float pulseSpeed = 1.9f;
        float minIntensity = 0.2f;
        float maxIntensity = EventManager.Instance.originalIntensity;
        float targetIntensity = minIntensity;
        foreach (Light l in EventManager.Instance.lights)
        {
            l.color = Color.red;
        }
        while (infos.eventActive)
        {
            yield return new WaitForSeconds(0.01f);
            foreach (Light l in EventManager.Instance.lights)
            {
                float intensity = Mathf.MoveTowards(l.intensity, targetIntensity, Time.deltaTime * pulseSpeed);
                if (intensity >= maxIntensity)
                {
                    intensity = maxIntensity;
                    targetIntensity = minIntensity;
                }
                else if (intensity <= minIntensity)
                {
                    intensity = minIntensity;
                    targetIntensity = maxIntensity;
                }
                l.intensity = intensity;
            }
        }
        foreach (Light l in EventManager.Instance.lights)
        {
            l.color = EventManager.Instance.originalColor;
        }
    }
}
public class EventFire : MonoBehaviour
{
    EventFireInfos infos;

    public void Start()
    {
        infos = EventManager.Instance.eventFire;
    }
    public void StartEvent()
    {
        EventManager.Instance.eventActive = true;
        infos.eventActive = true;
    }

    public void Update()
    {
        if (infos.eventActive)
        {
            infos.eventActive = false;
            EventManager.Instance.eventActive = false;
        }
    }
}

[System.Serializable]
public class EventLightInfos : System.Object
{
    public bool eventActive;
    [Tooltip("in percent according to the others")]
    [Range(0, 100)]
    public int eventTypeRisk = 34;
    public float eventDuration = 30f;
}
[System.Serializable]
public class EventMeteorInfos : System.Object
{
    public bool eventActive;
    [Tooltip("in percent according to the others")]
    [Range(0, 100)]
    public int eventTypeRisk = 34;
    [Range(0, 100)]
    public int meteorAmount = 10;
    [Range(0.2f, 5f)]
    public float timeBetweenMeteor = 2f;
    public int damagesCenter = 45;
    public int damagesBorder = 15;
    public GameObject meteorPrefab;
}
[System.Serializable]
public class EventFireInfos : System.Object
{
    public bool eventActive;
    [Tooltip("in percent according to the others")]
    [Range(0, 100)]
    public int eventTypeRisk = 34;
    public float eventDuration = 30f;
}