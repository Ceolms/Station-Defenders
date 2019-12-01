using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelScoreScript : MonoBehaviour
{
    public PlayerController player;

    int countAlienGreen;
    int countAlienPurple;
    int countAlienRed;
    int countRes;

    public Text textScoreAlienGreen;
    public Text textScoreAlienPurple;
    public Text textScoreAlienRed;
    public Text textScoreRes;

    public Text textAlienGreen;
    public Text textAlienPurple;
    public Text textAlienRed;
    public Text textRes;

    public Text textScoreTotal;

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
        StartCoroutine(PanelRoutine());
    }
    private IEnumerator PanelRoutine()
    {
        textScoreAlienGreen.gameObject.SetActive(true);
        textAlienGreen.gameObject.SetActive(true);
        bool isMax = true;
        player.score = (countAlienGreen * (int)ScoreID.alienGreen) + (countAlienPurple * (int)ScoreID.alienPurple)
           + (countAlienRed * (int)ScoreID.alienBoss) + (countRes * (int)ScoreID.resurrection);
        foreach (PlayerController p in GameManager.Instance.players)
        {
             if (p.score > player.score) isMax = false;   
        }
        int scoreAlienGreen = (int)ScoreID.alienGreen;
        for (int i = 0; i <= countAlienGreen; i++)
        {
            textScoreAlienGreen.text = i + " * " + scoreAlienGreen;
            yield return new WaitForSeconds(0.2f);
        }

        textScoreAlienPurple.gameObject.SetActive(true);
        textAlienPurple.gameObject.SetActive(true);
        int scoreAlienPurple = (int)ScoreID.alienPurple;
        for (int i = 0; i <= countAlienPurple; i++)
        {
            textScoreAlienPurple.text = i + " * " + scoreAlienPurple.ToString();
            yield return new WaitForSeconds(0.2f);
        }

        textScoreAlienRed.gameObject.SetActive(true);
        textAlienRed.gameObject.SetActive(true);
        int scoreAlienRed = (int)ScoreID.alienBoss;
        for (int i = 0; i <= countAlienRed; i++)
        {
            textScoreAlienRed.text = i + " * " + scoreAlienRed.ToString();
            yield return new WaitForSeconds(0.2f);
        }

        textRes.gameObject.SetActive(true);
        textScoreRes.gameObject.SetActive(true);
        int scoreRes = (int)ScoreID.resurrection;
        for (int i = 0; i <= countRes; i++)
        {
            textScoreRes.text = i + " * " + scoreRes.ToString();
            yield return new WaitForSeconds(0.2f);
        } 

        textScoreTotal.gameObject.SetActive(true);
        if (isMax) SoundPlayer.Instance.Play("ScoreCount");
        for (int i = 0; i <= player.score; i++)
        {
            yield return new WaitForSeconds(0.01f);
            textScoreTotal.text = i.ToString();  
        }
        if (isMax)
        {
            SoundPlayer.Instance.Stop("ScoreCount");
        }
    }
}
