using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int greenAlienCount;
        public int purpleAlienCount;
        public int redAlienCount;
        public float rate;
    }

    public GameObject greenAlien;
    public GameObject purpleAlien;
    public GameObject redAlien;

    public List<Wave> waves = new List<Wave>();
    private IEnumerator<Wave> wavesEnum;
    private int index;

    private GameObject[] spawnPoints;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    private void Start()
    {

        if (waves.Count == 0)
        {
            Debug.LogError("No waves referenced");
        }

        spawnPoints = GameObject.FindGameObjectsWithTag("AlienSpawn");

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced");
        }
      
        waveCountdown = timeBetweenWaves;
        wavesEnum = waves.GetEnumerator();
        index = 0;
    }

    private void Update()
    {
        if (state == SpawnState.WAITING)
        {
            // Ckeck if enemies are still alive
            if (!EnemyIsAlive())
            {
                // Begin a new round
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                wavesEnum.MoveNext();
                index++;
                //Debug.Log("Starting Wave : " + index);
                StartCoroutine(SpawnWave(wavesEnum.Current));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }


    void WaveCompleted()
    {
      // Debug.Log("Wave Completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        foreach(PlayerController p in GameManager.Instance.players)
        {
            p.infos.grenadeCount = 3;
            p.uiManager.SetGrenadeCount(p.infos.grenadeCount);
        }
        if (index == waves.Count)
        {
            wavesEnum.Reset();
            index = 0;
         //  TODO WIN
        }
        else
        {
            //TODO next wave text
        }

    }



    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Alien") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        state = SpawnState.SPAWNING;
       // Debug.Log("Start Wave");
       // Debug.Log("GreenAliens:" + wave.greenAlienCount);
        // spawn
        for (int i = 0; i < wave.greenAlienCount; i++)
        {
            
            SpawnEnemy(greenAlien);
            yield return new WaitForSeconds(wave.rate);
        }

        for (int i = 0; i < wave.purpleAlienCount; i++)
        {
            SpawnEnemy(purpleAlien);
            yield return new WaitForSeconds(wave.rate);
        }

        for (int i = 0; i < wave.redAlienCount; i++)
        {
            SpawnEnemy(redAlien);
            yield return new WaitForSeconds(wave.rate);
        }
        //Debug.Log("Spawning Alien");
        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(GameObject enemy)
    {
        //Debug.Log("Spawning enemy : " + enemy.name);
     
        GameObject sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, sp.transform.position, sp.transform.rotation);
    }
}
