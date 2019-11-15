using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject UILifebar;
    public GameObject playerNameObject;
    public GameObject minimapObject;

    public GameObject spriteEnergy;
    public GameObject spriteMeteor;
    public GameObject spriteFire;

    private Image imageOverlayRed;
    private float sizeMaxLife;
    private bool minimapVisible;
    void Awake()
    {
        sizeMaxLife = UILifebar.transform.localScale.x;
        imageOverlayRed = UILifebar.transform.Find("BarRedOverlay").GetComponent<Image>();
    }

    public void SetLifebarSize(int percent)
    {
        if(imageOverlayRed == null) UILifebar.transform.Find("BarRedOverlay").GetComponent<Image>();
        float scale = sizeMaxLife / 100;

        UILifebar.transform.localScale = new Vector2(scale * percent, UILifebar.transform.localScale.y);
        if(percent < 40)
        {
            imageOverlayRed.color =  new Color(255, 0, 0, 0.5f);
        }
        else
        {
            imageOverlayRed.color = new Color(255, 0, 0, 0.0f);
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
    public void ShowWarningSprite(EventType type)
    {
        switch(type)
        {
            case EventType.LightBreakdown:
                StartCoroutine(ShowWarningSpriteRoutine(spriteEnergy));
                break;
            case EventType.MeteorShower:
                StartCoroutine(ShowWarningSpriteRoutine(spriteMeteor));
                break;
            case EventType.Fire:
                StartCoroutine(ShowWarningSpriteRoutine(spriteFire));
                break;
        }
    }
    private IEnumerator ShowWarningSpriteRoutine(GameObject spriteObj)
    {

        for(int i = 0; i<  8; i++)
        {
            spriteMeteor.SetActive(!spriteMeteor.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
       
    }
}
