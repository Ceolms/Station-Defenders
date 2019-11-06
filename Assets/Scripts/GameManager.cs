using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> listPlayersPrefabs;
    private List<PlayerController> players = new List<PlayerController>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Debug.Log(listPlayersPrefabs.Count);
        InitializePlayers();
        ForceSpawn(2);
        IntializeSplitScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializePlayers()
    {
        if(ReInput.controllers.Joysticks.Count == 0)
        {
            GameObject player1 = listPlayersPrefabs[0];
            Instantiate(player1);
            GameObject spawnPosition = GameObject.Find("SpawnPosition Player1");
            if (spawnPosition != null) player1.transform.position = spawnPosition.transform.position;
            else player1.transform.position = new Vector3(0, 0, 0);
            player1.GetComponent<PlayerController>().useKeyboard = true;
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
                GameObject spawnPosition = GameObject.Find("SpawnPosition Player"+(i+1));
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
    /// For debug purpose , add a player without controller. i = playerId -1
    /// </summary>
    void ForceSpawn(int i )
    {
        GameObject player = listPlayersPrefabs[i];
        Instantiate(player);
        GameObject spawnPosition = GameObject.Find("SpawnPosition Player" + (i+1));
        if (spawnPosition != null) player.transform.position = spawnPosition.transform.position;
        else player.transform.position = new Vector3(i * 4, 0, 0);
        players.Add(player.GetComponent<PlayerController>());
    }
}
