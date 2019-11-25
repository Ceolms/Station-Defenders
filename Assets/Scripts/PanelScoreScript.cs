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
    int scoreTotal;

    int valAG = (int)ScoreID.alienGreen;
    int valAp = (int)ScoreID.alienPurple;
    int valAR = (int)ScoreID.alienBoss;
    int valRes = (int)ScoreID.resurrection;

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
        for(int i = 0; i <= countAlienGreen; i++)
        {
            textScoreAlienGreen.text = i + " * " + ScoreID.alienGreen;
            yield return new WaitForSeconds(0.2f);
        }

        textScoreAlienPurple.gameObject.SetActive(true);
        textAlienPurple.gameObject.SetActive(true);
        for (int i = 0; i <= countAlienPurple; i++)
        {
            textScoreAlienPurple.text = i + " * " + ScoreID.alienPurple;
            yield return new WaitForSeconds(0.2f);
        }

        textScoreAlienRed.gameObject.SetActive(true);
        textAlienRed.gameObject.SetActive(true);
        for (int i = 0; i <= countAlienRed; i++)
        {
            textScoreAlienRed.text = i + " * " + ScoreID.alienBoss;
            yield return new WaitForSeconds(0.2f);
        }

        textRes.gameObject.SetActive(true);
        textScoreRes.gameObject.SetActive(true);
        for (int i = 0; i <= countRes; i++)
        {
            textScoreRes.text = i + " * " + ScoreID.alienBoss;
            yield return new WaitForSeconds(0.2f);
        }

        scoreTotal = (countAlienGreen * (int)ScoreID.alienGreen) + (countAlienPurple * (int)ScoreID.alienPurple) 
            + (countAlienRed * (int)ScoreID.alienBoss) + (countRes * (int)ScoreID.resurrection);

        textScoreTotal.gameObject.SetActive(true);
        for (int i = 0; i <= scoreTotal; i++)
        {
            textScoreTotal.text = i.ToString();
            yield return new WaitForSeconds(0.02f);
        }
    }
}
