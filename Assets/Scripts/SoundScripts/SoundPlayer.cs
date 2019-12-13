using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }
    public bool mainMenu;
    public Sound[] soundList;

    public Sound[] musicList;

    [HideInInspector]
    public float multiplier = 1f;
    [HideInInspector]
    public float musicMultiplier = 1f;

    private bool isPlayingMusic;
    void Awake()
    {
        Instance = this;

        multiplier = PlayerPrefs.GetFloat("volume", 1f);
        musicMultiplier = PlayerPrefs.GetFloat("volumeMusic", 1f);

        UpdateVolume();
        UpdateMusicVolume();
    }
    private void Update()
    {
        
        if(!isPlayingMusic && musicList.Length >0 && !mainMenu)
        {
            int r = Random.Range(0, musicList.Length);
            {
                PlayMusic(musicList[r].name);
            }
        }
    }
    public void Play(string name)
    {
        Sound s = System.Array.Find(soundList, sound => sound.name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase));
        if (s != null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(this.transform);
            AudioSource aSource = go.AddComponent<AudioSource>();
            go.name = s.name;
            aSource.clip = s.clip;
            aSource.loop = s.loop;
            aSource.volume = s.volume;
            aSource.Play();
            if(!aSource.loop)StartCoroutine(DestroySource(go));
        }
        else  Debug.Log("Sound " + name + " doesn't exist");
    }

    public void Stop(string name)
    {
        GameObject go = GameObject.Find(name);
        if(go != null)
        {
            AudioSource s = go.GetComponent<AudioSource>();
            if (s != null) s.Stop();
            Debug.Log("Stopping" + go.name);
            Destroy(go);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(musicList, sound => sound.name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase));
        if (s != null)
        {
            AudioSource so;

            if (this.GetComponent<AudioSource>() == null)
            {
                so = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            }
            else so = this.GetComponent<AudioSource>();

            s.source = so;
            s.source.loop = s.loop;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.Play();
            isPlayingMusic = true;
            if(!s.source.loop)
            {
                StartCoroutine(EndMusic(s.clip.length));
            }
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
    private IEnumerator EndMusic(float t)
    {
        yield return new WaitForSeconds(t + 2f);
        isPlayingMusic = false;
    }
}
