using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<GameObject> listPlayersPrefabs;
    public List<PlayerController> players;
    public List<Pair<PlayerID, int>> listScores;
    // Start is called before the first frame update
    public bool forceSpawnP2;
    public bool forceSpawnP3;
    public bool forceSpawnP4;
    [HideInInspector]  public bool worldPanelVisible;
    [HideInInspector] public bool gameRunning;
    public bool demoMode;
    private bool isPlayingScoreCount;
    void Awake()
    {
        Instance = this;
        players = new List<PlayerController>();
        listScores = new List<Pair<PlayerID, int>>();
        
        InitializePlayers();
        if (forceSpawnP2) ForceSpawn(2);
        if (forceSpawnP3) ForceSpawn(3);
        if (forceSpawnP4) ForceSpawn(4);
        IntializeSplitScreen();
        gameRunning = true;
    }

    private void Update()
    {
        if (gameRunning && !worldPanelVisible)
        {

            bool allPlayersdown = true;
            foreach (PlayerController p in players)
            {
                if (!p.isFainting) allPlayersdown = false;
            }
            if (allPlayersdown || GameObject.Find("Core").GetComponent<Core>().currentHealth <= 0)
            {
                foreach (PlayerController p in players)
                {
                    p.isMoving = false;
                    p.animator.SetBool("isRunning", false);
                }
                gameRunning = false;
                worldPanelVisible = true;
                if (allPlayersdown)
                {
                    players[0].uiManager.GameOverScreen(GameOverType.PlayersDown);
                }
                else
                {
                    players[0].uiManager.GameOverScreen(GameOverType.CoreDestroyed);
                    SoundPlayer.Instance.Play("GameOver");
                }
            }
        }
       if (worldPanelVisible && listScores.Count == players.Count && !isPlayingScoreCount)
        {
            SoundPlayer.Instance.Play("ScoreCount");
            isPlayingScoreCount = true;
        }
        if (worldPanelVisible && isPlayingScoreCount && listScores.Count == 0)
        {
            isPlayingScoreCount = false;
            SoundPlayer.Instance.Stop("ScoreCount");
        }
    }
    /// <summary>
    /// Check the number of controllers connected and spawn a player for each of them .
    /// If no controllers are connected , the P1 is spawn if keyboard and mouse inputs
    /// </summary>
    void InitializePlayers()
    {
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, true);
        }
        if (!demoMode)
        {

            if (PlayerPrefs.GetInt("nbPlayers") > 0)
            {
                Debug.Log("Players : " + PlayerPrefs.GetInt("nbPlayers"));

                for (int i = 0; i < PlayerPrefs.GetInt("nbPlayers"); i++)
                {

                    GameObject playerPrefab = listPlayersPrefabs[i];
                    GameObject player = Instantiate(playerPrefab);

                    string spawnS = "SpawnPosition Player" + (i + 1).ToString();
                    GameObject spawnPosition = GameObject.Find(spawnS);
                    if (spawnPosition != null) player.transform.position = spawnPosition.transform.position;
                    else player.transform.position = new Vector3(i * 4, 0, 0);
                    players.Add(player.GetComponent<PlayerController>());


                    Debug.Log("player" + i + " :  " + PlayerPrefs.GetInt("player" + i));

                    if (PlayerPrefs.GetInt("player" + i) == -1)
                    {
                        Player playerkeyboard = ReInput.players.GetPlayer(i);

                        playerkeyboard.controllers.hasMouse = true;

                        // Load the keyboard and mouse maps into the Player
                        playerkeyboard.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", true);
                        playerkeyboard.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", true);
                        playerkeyboard.controllers.maps.LoadMap(ControllerType.Mouse, 0, "Default", "Default", true);
                        player.GetComponent<PlayerController>().useKeyboard = true;
                    }
                    else
                    {
                        Player playerController = ReInput.players.GetPlayer(i);
                        Joystick j = ReInput.controllers.Joysticks[PlayerPrefs.GetInt("player" + i)];
                        playerController.controllers.AddController(j, true);
                        playerController.isPlaying = true;
                    }
                }
                return;
            }
            else if (ReInput.controllers.Joysticks.Count == 0)
            {
                Debug.Log("Players : ReInput.controllers == 0");

                GameObject player1Prefab = listPlayersPrefabs[0];
                GameObject player1 = Instantiate(player1Prefab);

                GameObject spawnPosition = GameObject.Find("SpawnPosition Player1");
                if (spawnPosition != null) player1.transform.position = spawnPosition.transform.position;
                else player1.transform.position = new Vector3(0, 0, 0);

                player1.GetComponent<PlayerController>().useKeyboard = true;
                players.Add(player1.GetComponent<PlayerController>());
            }
            else
            {
                Debug.Log("auto assign");
                for (int i = 0; i < ReInput.controllers.Joysticks.Count; i++)
                {
                    Player p = ReInput.players.Players[i];
                    Joystick j = ReInput.controllers.Joysticks[i];
                    p.controllers.AddController(j, true);

                    GameObject playerPrefab = listPlayersPrefabs[i];
                    GameObject player = Instantiate(playerPrefab);

                    string spawnS = "SpawnPosition Player" + (i + 1).ToString();
                    GameObject spawnPosition = GameObject.Find(spawnS);
                    if (spawnPosition != null) player.transform.position = spawnPosition.transform.position;
                    else player.transform.position = new Vector3(i * 4, 0, 0);
                    players.Add(player.GetComponent<PlayerController>());
                }
            }
        }
        else
        {
            GameObject player1 = GameObject.Find("Player 1");
            player1.GetComponent<PlayerController>().useKeyboard = true;
            players.Add(player1.GetComponent<PlayerController>());

            Player playerkeyboard = ReInput.players.GetPlayer(0);
            playerkeyboard.controllers.hasMouse = true;
            playerkeyboard.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", true);
            playerkeyboard.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", true);
            playerkeyboard.controllers.maps.LoadMap(ControllerType.Mouse, 0, "Default", "Default", true);
        }
      

    }
    /// <summary>
    /// Initialize camera positions en screen depending on the number of players
    /// </summary>
    void IntializeSplitScreen()
    {
        if (players.Count == 1)
        {
            Camera cp1 = players[0].GetComponentInChildren<Camera>();
            cp1.rect = new Rect(0, 0, 1, 1);

        }
        else if (players.Count == 2)
        {
            Camera cp1 = players[0].GetComponentInChildren<Camera>();
            cp1.rect = new Rect(0, 0, 0.5f, 1);
            Camera cp2 = players[1].GetComponentInChildren<Camera>();
            cp2.rect = new Rect(0.5f, 0, 0.5f, 1);

        }
        else if (players.Count == 3)
        {
            Camera cp1 = players[0].GetComponentInChildren<Camera>();
            cp1.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            Camera cp2 = players[1].GetComponentInChildren<Camera>();
            cp2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            Camera cp3 = players[2].GetComponentInChildren<Camera>();
            cp3.rect = new Rect(0, 0, 0.5f, 0.5f);
        }
        else // 4
        {
            Camera cp1 = players[0].GetComponentInChildren<Camera>();
            cp1.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            Camera cp2 = players[1].GetComponentInChildren<Camera>();
            cp2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            Camera cp3 = players[2].GetComponentInChildren<Camera>();
            cp3.rect = new Rect(0, 0, 0.5f, 0.5f);
            Camera cp4 = players[3].GetComponentInChildren<Camera>();
            cp4.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
    }

    /// <summary>
    /// For debug purpose , add a player without controller.
    /// </summary>
    void ForceSpawn(int id)
    {
        GameObject playerPrefab = listPlayersPrefabs[id - 1];
        GameObject player = Instantiate(playerPrefab);
        GameObject spawnPosition = GameObject.Find("SpawnPosition Player" + (id));
        if (spawnPosition != null) player.transform.position = spawnPosition.transform.position;
        else player.transform.position = new Vector3(id - 1 * 4, 0, 0);
        players.Add(player.GetComponent<PlayerController>());
    }

}
public class Pair<T1, T2>
{
    public T1 First { get; private set; }
    public T2 Second { get; private set; }
    internal Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}

public static class Pair
{
    public static Pair<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Pair<T1, T2>(first, second);
        return tuple;
    }
}
