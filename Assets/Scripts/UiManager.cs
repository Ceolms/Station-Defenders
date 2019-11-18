using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject UILifebar;
    public GameObject UICoreLifebar;
    public GameObject playerNameObject;
    public GameObject minimapObject;

    public GameObject spriteEnergy;
    public GameObject spriteMeteor;
    public GameObject spriteFire;
    public GameObject damageEffectPanel;
    public GameObject gameoverPanel;

    private Image imageOverlayRed;
    private float sizeMaxLife;
    private float sizeMaxLifeCore;
    private bool minimapVisible;
    void Awake()
    {
        sizeMaxLife = UILifebar.transform.localScale.x;
        sizeMaxLifeCore = UICoreLifebar.transform.localScale.x;
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

    public void SetCoreLifebarSize(int percent)
    {
        float scale = sizeMaxLifeCore / 100;

        UICoreLifebar.transform.localScale = new Vector2(scale * percent, UICoreLifebar.transform.localScale.y);
    }

    public void ActiveDamageEffect()
    {
        StartCoroutine(ShowDamagePanel());
    }

    public void GameOverScreen()
    {
        StartCoroutine(ShowGameOverPanel());
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
            spriteObj.SetActive(!spriteObj.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
       
    }
    private IEnumerator ShowDamagePanel()
    {
        damageEffectPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        damageEffectPanel.SetActive(false);
    }

    private IEnumerator ShowGameOverPanel()
    {
        gameoverPanel.SetActive(true);
        Image img = gameoverPanel.GetComponent<Image>();
        while(img.color.a < 1)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.01f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
