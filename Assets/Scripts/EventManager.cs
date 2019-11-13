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

    public EventInfos eventLightBreakdown;
    public EventInfos eventMeteor;
    public EventInfos eventFire;
    private EventLight eventLScript;
    private EventMeteor eventMScript;
    private EventFire eventFScript;

    [Tooltip("in percent up to 100% ( always happen)")]
    [Range(0, 100)]
    public float eventRisk = 0;
    public float waitTime = 1f;
    public float coolDown;

    // Start is called before the first frame update
    void Start()
    {
        eventTypeProba = new List<EventType>();
        Instance = this;
        for (int i = 0; i < eventLightBreakdown.eventTypeRisk; i++) eventTypeProba.Add(EventType.LightBreakdown);
        for (int i = 0; i < eventMeteor.eventTypeRisk; i++) eventTypeProba.Add(EventType.MeteorShower);
        for (int i = 0; i < eventFire.eventTypeRisk; i++) eventTypeProba.Add(EventType.Fire);
        if (this.GetComponent<EventLight>() == null) eventLScript = this.gameObject.AddComponent<EventLight>();
        if (this.GetComponent<EventMeteor>() == null) eventMScript = this.gameObject.AddComponent<EventMeteor>();
        if (this.GetComponent<EventFire>() == null) eventFScript = this.gameObject.AddComponent<EventFire>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!eventActive && canEventHappen && !randomActive)
        {
            StartCoroutine(TestRandom());
        }
    }

    private IEnumerator TestRandom()
    {
        Debug.Log("TestRandom start");
        randomActive = true;
        if(eventTypeProba.Count >0)
        {
            while (!eventActive)
            {
                yield return new WaitForSeconds(waitTime);
                int i = Random.Range(0, 100);
                Debug.Log("TestRandom :" + i);
                if (i < eventRisk) // something happen !
                {
                    int j = Random.Range(0, eventTypeProba.Count);
                    EventType evt = eventTypeProba[j];
                    Debug.Log("EventType:" + evt);
                    switch(evt)
                    {
                        case (EventType.LightBreakdown):
                            eventActive = true;
                            eventLScript.StartEvent();
                            break;
                        case (EventType.MeteorShower):
                            eventActive = true;
                            eventMScript.StartEvent();
                            break;
                        case (EventType.Fire):
                            eventActive = true;
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
    EventInfos infos;

    GameObject[] lights;
    GameObject[] playersLights;
    private float lightdecreaseSpeed = 0.05f;

    public void Start()
    {
        infos = EventManager.Instance.eventLightBreakdown;
        lights = GameObject.FindGameObjectsWithTag("Light");
        playersLights = GameObject.FindGameObjectsWithTag("PlayerLight");
    }

    public void StartEvent()
    {
        infos.eventActive = true;
        StartCoroutine(LightBackActive());
        Debug.Log("LightBreakDown event begin!");
    }
    public void Update()
    {
        if (infos.eventActive)
        {
            foreach (GameObject l in lights)
            {
                Light light = l.GetComponent<Light>();
                light.intensity = light.intensity - lightdecreaseSpeed;

            }
            if(lights[0].GetComponent<Light>().intensity < 0.5f)
            {
                foreach (GameObject l in playersLights)
                {
                    Light light = l.GetComponent<Light>();
                    light.enabled = true;
                }
            }
        }
    }
    private IEnumerator LightBackActive()
    {
        yield return new WaitForSeconds(infos.eventDuration);
        foreach (GameObject l in lights)
        {
            Light light = l.GetComponent<Light>();
            light.intensity = 1.5f;
        }
        EventManager.Instance.eventActive = false;
        infos.eventActive = false;
        Debug.Log("LightBreakDown event finish!");
        foreach (GameObject l in lights)
        {
            Light light = l.GetComponent<Light>();
            light.enabled = false;
        }
    }
}

public class EventMeteor : MonoBehaviour
{
    EventInfos infos;

    public void Start()
    {
        infos = EventManager.Instance.eventMeteor;
    }
    public void StartEvent()
    {
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
public class EventFire : MonoBehaviour
{
    EventInfos infos;

    public void Start()
    {
        infos = EventManager.Instance.eventFire;
    }
    public void StartEvent()
    {
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
public class EventInfos : System.Object
{
    public bool eventActive;
    [Tooltip("in percent according to the others")]
    [Range(0, 100)]
    public int eventTypeRisk = 34;
    public float eventDuration = 30f;
}