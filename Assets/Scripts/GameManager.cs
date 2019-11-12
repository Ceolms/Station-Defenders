using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> listPlayersPrefabs;
    public List<PlayerController> players = new List<PlayerController>();
    // Start is called before the first frame update
    public bool forceSpawnP2;
    public bool forceSpawnP3;
    public bool forceSpawnP4;
    void Awake()
    {
        Instance = this;
        InitializePlayers();
        if (forceSpawnP2) ForceSpawn(2);
        if (forceSpawnP3) ForceSpawn(3);
        if (forceSpawnP4) ForceSpawn(4);

        IntializeSplitScreen();
    }

    /// <summary>
    /// Check the number of controllers connected and spawn a player for each of them .
    /// If no controllers are connected , the P1 is spawn if keyboard and mouse inputs
    /// </summary>
    void InitializePlayers()
    {
        if(ReInput.controllers.Joysticks.Count == 0)
        {
            GameObject player1 = listPlayersPrefabs[0];
            Instantiate(player1);
            GameObject spawnPosition = GameObject.Find("SpawnPosition Player1");
            if (spawnPosition != null) player1.transform.position = spawnPosition.transform.position;
            else player1.transform.position = new Vector3(0, 0, 0);

            player1.GetComponent<PlayerController>().SetKeyboardEnabled(true);
            players.Add(player1.GetComponent<PlayerController>());
        }
        else
        {
            for(int i = 0; i < ReInput.controllers.Joysticks.Count;i++)
            {
                Player p = ReInput.players.Players[i];
                Joystick j = ReInput.controllers.Joysticks[i];
                p.controllers.AddController(j,true);

                GameObject player = listPlayersPrefabs[i];
                Instantiate(player);
                string spawnS = "SpawnPosition Player" + (i + 1).ToString();
                GameObject spawnPosition = GameObject.Find(spawnS);
                if (spawnPosition != null) player.transform.position = spawnPosition.transform.position;
                else player.transform.position = new Vector3(i*4, 0, 0);
                players.Add(player.GetComponent<PlayerController>());
            }

            
        }
    }
    /// <summary>
    /// Initialize camera positions en screen depending on the number of players
    /// </summary>
    void IntializeSplitScreen()
    {
        if(players.Count == 1)
        {
            players[0].camera.rect = new Rect(0, 0, 1, 1);
        }
        else if (players.Count == 2)
        {
            players[0].camera.rect = new Rect(0, 0, 0.5f, 1);
            players[1].camera.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else if (players.Count == 3)
        {
            players[0].camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            players[1].camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            players[2].camera.rect = new Rect(0, 0, 0.5f, 0.5f);
        }
        else // 4
        {
            players[0].camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            players[1].camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            players[2].camera.rect = new Rect(0, 0, 0.5f, 0.5f);
            players[3].camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
    }

    /// <summary>
    /// For debug purpose , add a player without controller.
    /// </summary>
    void ForceSpawn(int id )
    {
        GameObject player = listPlayersPrefabs[id - 1];
        Instantiate(player);
        GameObject spawnPosition = GameObject.Find("SpawnPosition Player" + (id));
        if (spawnPosition != null) player.transform.position = spawnPosition.transform.position;
        else player.transform.position = new Vector3(id-1 * 4, 0, 0);
        players.Add(player.GetComponent<PlayerController>());
    }
 
}
