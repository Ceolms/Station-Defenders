using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

  public enum SpawnState { SPAWNING, WAITING, COUNTING };

  [System.Serializable]
  public class Wave
  {
    public string name;
    public int greenAlienCount;
    public int purpleAlienCount;
    public int redAlienCount;
    public float rate;
  }

  public Transform greenAlien;
  public Transform purpleAlien;
  public Transform redAlien;

  public List<Wave> waves = new List<Wave>();
  private IEnumerator<Wave> wavesEnum;
  private int index;

  public Transform[] spawnPoints;

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
      if(state != SpawnState.SPAWNING)
      {
        wavesEnum.MoveNext();
        index++;
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
    Debug.Log("Wave Completed!");

    state = SpawnState.COUNTING;
    waveCountdown = timeBetweenWaves;

    if (index == waves.Count)
    {
      wavesEnum.Reset();
      index = 0;
      Debug.Log("All Waves Complete! Looping...");
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
    Debug.Log("Spawning Wave: " + wave.name);
    state = SpawnState.SPAWNING;

    // spawn
    for (int i = 0; i < wave.greenAlienCount; i++)
    {
      SpawnEnemy(greenAlien);
      yield return new WaitForSeconds(1f / wave.rate);
    }

    for (int i = 0; i < wave.purpleAlienCount; i++)
    {
      SpawnEnemy(purpleAlien);
      yield return new WaitForSeconds(1f / wave.rate);
    }

    for (int i = 0; i < wave.redAlienCount; i++)
    {
      SpawnEnemy(redAlien);
      yield return new WaitForSeconds(1f / wave.rate);
    }

    state = SpawnState.WAITING;

    yield break;
  }

  void SpawnEnemy(Transform enemy)
  {
    // Spawn enemy
    Debug.Log("Spawning enemy : " + enemy.name);

    Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
    Instantiate(enemy, sp.position, sp.rotation);
  }
}
