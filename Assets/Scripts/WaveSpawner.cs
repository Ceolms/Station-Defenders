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
    public Transform enemy;
    public int count;
    public float rate;
  }

  public Wave[] waves;
  private int nextWave = 0;

  public Transform[] spawnPoints;

  public float timeBetweenWaves = 5f;
  public float waveCountdown;

  private float searchCountdown = 1f;

  private SpawnState state = SpawnState.COUNTING;

  private void Start()
  {

    if (waves.Length == 0)
    {
      Debug.LogError("No waves referenced");
    }

    if (spawnPoints.Length == 0)
    {
      Debug.LogError("No spawn points referenced");
    }

    waveCountdown = timeBetweenWaves;
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
        // Start spawning wave
        StartCoroutine(SpawnWave(waves[nextWave]));
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

    if(nextWave+1 > waves.Length - 1)
    {
      nextWave = 0;
      Debug.Log("All Waves Complete! Looping...");
    }
    else
    {
      nextWave++;
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
    for (int i = 0; i < wave.count; i++)
    {
      SpawnEnemy(wave.enemy);
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
