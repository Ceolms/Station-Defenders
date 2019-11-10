using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject UILifebar;
    public GameObject playerNameObject;
    public GameObject minimapObject;
    private Image imageOverlayRed;
    private float sizeMaxLife;
    private bool minimapVisible;
    void Start()
    {
        sizeMaxLife = UILifebar.transform.localScale.x;
        imageOverlayRed = UILifebar.transform.Find("BarRedOverlay").GetComponent<Image>();

        StartCoroutine(LifeBarTestRoutine());
    }

    public void SetLifebarSize(int percent)
    {
        float scale = sizeMaxLife / 100;

        UILifebar.transform.localScale = new Vector2(scale * percent, UILifebar.transform.localScale.y);
        if(percent < 40)
        {
            imageOverlayRed.color =  new Color(255, 0, 0, 0.5f);
        }
    }

    public void TriggerMinimap()
    {
        minimapVisible = !minimapVisible;
        minimapObject.SetActive(minimapVisible);
    }

    public void SetName(PlayerID id)
    { 
        Text textPlayerName = playerNameObject.GetComponent<Text>();
        switch (id)
        {
            case (PlayerID.Player1):
                textPlayerName.text = "Player 1";
                textPlayerName.color = Color.blue;
                break;
            case (PlayerID.Player2):
                textPlayerName.text = "Player 2";
                textPlayerName.color = Color.red;
                break;
            case (PlayerID.Player3):
                textPlayerName.text = "Player 3";
                textPlayerName.color = Color.green;
                break;
            case (PlayerID.Player4):
                textPlayerName.text = "Player 4";
                textPlayerName.color = Color.yellow;
                break;
        }
    }
    IEnumerator LifeBarTestRoutine()
    {
        for(int i = 100; i >=10; i -=5)
        {
            SetLifebarSize(i);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
