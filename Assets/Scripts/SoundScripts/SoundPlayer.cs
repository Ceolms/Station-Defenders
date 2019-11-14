using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    public Sound[] soundList;


    public Sound[] musicList;


    public AudioSource sourceMusique;

    [HideInInspector]
    public float multiplier = 1f;
    [HideInInspector]
    public float musicMultiplier = 1f;

    void Awake()
    {
        Instance = this;

        multiplier = PlayerPrefs.GetFloat("volume", 1f);
        musicMultiplier = PlayerPrefs.GetFloat("volumeMusic", 1f);

        UpdateVolume();
        UpdateMusicVolume();

    }

    public void Play(string name)
    {
        Sound s = System.Array.Find(soundList, sound => sound.name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase));
        if (s != null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(this.transform);
            AudioSource aSource = go.AddComponent<AudioSource>();
            go.name = "Sound : " + s.name;
            s.source = aSource;
            s.source.loop = s.loop;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.Play();
            StartCoroutine(DestroySource(go));
        }
        else  Debug.Log("Sound " + name + " doesn't exist");
    }

    public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(musicList, sound => sound.name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase));
        if (s != null)
        {
            s.source = sourceMusique;
            s.source.loop = s.loop;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
        }
        else Debug.Log("Sound " + name + " doesn't exist");
    }

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("volume", multiplier);                 // set in the preferences
        AudioListener.volume = multiplier;
    }

    public void UpdateMusicVolume()
    {
        PlayerPrefs.SetFloat("volumeMusic", musicMultiplier);       // set in the preferences
    }

    private IEnumerator DestroySource(GameObject go)
    {
        yield return new WaitForSeconds(10);
        Destroy(go);
    }
}
