using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
public class PanelScoreScript : MonoBehaviour
{
    public PlayerController player;
    private Thread t;
    int countAlienGreen;
    int countAlienPurple;
    int countAlienRed;
    int countRes;
    bool wait;
    float timer;
    bool isStarted;
    public Text textScoreAlienGreen;
    public Text textScoreAlienPurple;
    public Text textScoreAlienRed;
    public Text textScoreRes;

    public Text textAlienGreen;
    public Text textAlienPurple;
    public Text textAlienRed;
    public Text textRes;

    public Text textScoreTotal;

    private void Update()
    {
        if (wait)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                wait = false;
            }
        }
    }
    public void ShowPanel()
    {
        for(int i = 0; i< player.scoreList.Count; i++)
        {
            switch(player.scoreList[i])
            {
                case (ScoreID.alienGreen):
                    countAlienGreen++;
                    break;
                case (ScoreID.alienPurple):
                    countAlienPurple++;
                    break;
                case (ScoreID.alienBoss):
                    countAlienRed++;
                    break;
                case (ScoreID.resurrection):
                    countRes++;
                    break;
            }
        }
        PanelRoutine();
       
       // StartCoroutine(PanelRoutine());
    }

    private void PanelRoutine()
    {
        isStarted = true;
        textScoreAlienGreen.gameObject.SetActive(true);
        textAlienGreen.gameObject.SetActive(true);
        bool isMax = true;
        int scoreAlienGreen = (int)ScoreID.alienGreen;
        int scoreAlienPurple = (int)ScoreID.alienPurple;
        int scoreAlienRed = (int)ScoreID.alienBoss;
        int scoreRes = (int)ScoreID.resurrection;


        player.score = (countAlienGreen * scoreAlienGreen) + (countAlienPurple * scoreAlienPurple)
           + (countAlienRed * scoreAlienRed) + (countRes * scoreRes);
        foreach (PlayerController p in GameManager.Instance.players)
        {
             if (p.score > player.score) isMax = false;   
        }
        for (int i = 0; i <= countAlienGreen; i++)
        {
            textScoreAlienGreen.text = i + " * " + scoreAlienGreen;
           // yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("Green OK");
        textScoreAlienPurple.gameObject.SetActive(true);
        textAlienPurple.gameObject.SetActive(true);

        for (int i = 0; i <= countAlienPurple; i++)
        {
            textScoreAlienPurple.text = i + " * " + scoreAlienPurple.ToString();
        }
        Debug.Log("Purple OK");
        textScoreAlienRed.gameObject.SetActive(true);
        textAlienRed.gameObject.SetActive(true);

        for (int i = 0; i <= countAlienRed; i++)
        {
            textScoreAlienRed.text = i + " * " + scoreAlienRed.ToString();
        }
        Debug.Log("RED OK");
        textRes.gameObject.SetActive(true);
        textScoreRes.gameObject.SetActive(true);

        for (int i = 0; i <= countRes; i++)
        {
            textScoreRes.text = i + " * " + scoreRes.ToString();
        }
        Debug.Log("RES OK");
        textScoreTotal.gameObject.SetActive(true);
        if (isMax) SoundPlayer.Instance.Play("ScoreCount");
        for (int i = 0; i <= player.score; i++)
        {
            // yield return new WaitForSeconds(0.01f);
            textScoreTotal.text = "Score : " + i.ToString();  
        }
        Debug.Log("Score OK");
        if (isMax)
        {
            SoundPlayer.Instance.Stop("ScoreCount");
        }
    }
}
