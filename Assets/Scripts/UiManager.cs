using Rewired;
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
    public GameObject textGrenade;

    public GameObject worldPanel;
    public GameObject textGameOver;
    public GameObject imageButton;

    public List<GameObject> prefabsScorePanel;

    public Sprite spriteA;
    public Sprite spriteEnter;
    [HideInInspector] public PlayerID owner;
    private Image imageOverlayRed;
    private float sizeMaxLife;
    private float sizeMaxLifeCore;
    private bool minimapVisible;
    private List<PanelScoreScript> listePanelsScores;
    void Awake()
    {
        sizeMaxLife = UILifebar.transform.localScale.x;
        sizeMaxLifeCore = UICoreLifebar.transform.localScale.x;
        imageOverlayRed = UILifebar.transform.Find("BarRedOverlay").GetComponent<Image>();
        listePanelsScores = new List<PanelScoreScript>();
    }

    public void SetLifebarSize(int percent)
    {
        if (imageOverlayRed == null) UILifebar.transform.Find("BarRedOverlay").GetComponent<Image>();
        float scale = sizeMaxLife / 100;

        UILifebar.transform.localScale = new Vector2(scale * percent, UILifebar.transform.localScale.y);
        if (percent < 40)
        {
            imageOverlayRed.color = new Color(255, 0, 0, 0.5f);
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

    public void GameOverScreen(GameOverType t)
    {
        StartCoroutine(ShowGameOverPanel(t));
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

    public void SetGrenadeCount(int n)
    {
        textGrenade.GetComponent<Text>().text = n.ToString();
        if (n > 0) textGrenade.GetComponent<Text>().color = Color.white;
        else textGrenade.GetComponent<Text>().color = Color.red;
    }
    public void ShowWarningSprite(EventType type)
    {
        switch (type)
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

        for (int i = 0; i < 8; i++)
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
    private IEnumerator ShowGameOverPanel(GameOverType t)
    {
        yield return new WaitForSeconds(0f);
        Debug.Log("Game Over");
        worldPanel.SetActive(true);
        textGameOver.SetActive(true);
        Image img = worldPanel.GetComponent<Image>();
        while (img.color.a < 0.3f)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.05f);
            Text textGO = textGameOver.GetComponent<Text>();
            textGO.color = new Color(textGO.color.r, textGO.color.g, textGO.color.b, textGO.color.a + 0.05f);
            yield return new WaitForSeconds(0.1f);
        }
        if (t == GameOverType.PlayersDown) SoundPlayer.Instance.Play("AllCrewDown");
        while (img.color.a < 1)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + 0.05f);
            Text textGO = textGameOver.GetComponent<Text>();
            textGO.color = new Color(textGO.color.r, textGO.color.g, textGO.color.b, textGO.color.a + 0.05f);
            yield return new WaitForSeconds(0.1f);
        }
        
        worldPanel.transform.GetChild(1).gameObject.SetActive(true);
        worldPanel.transform.GetChild(2).gameObject.SetActive(true);

        if (ReInput.controllers.Joysticks.Count == 0) imageButton.GetComponent<Image>().sprite = spriteEnter;
        else imageButton.GetComponent<Image>().sprite = spriteA;
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < GameManager.Instance.players.Count; i++)
        {
            Debug.Log("Spawning Panel" + GameManager.Instance.players.Count);
            GameObject panel = Instantiate(prefabsScorePanel[i]);
            panel.transform.parent = worldPanel.transform.GetChild(2).transform;
            listePanelsScores.Add(panel.GetComponent<PanelScoreScript>());
            listePanelsScores[i].player = GameManager.Instance.players[i];
            Debug.Log("showing screen of : player" + i);
            listePanelsScores[i].ShowPanel();
        }
        if (GameManager.Instance.players.Count == 1)
        {
            listePanelsScores[0].transform.position = GameObject.Find("PositionSolo").transform.position;
        }
        else if (GameManager.Instance.players.Count == 2)
        {
            listePanelsScores[0].transform.position = GameObject.Find("Position2").transform.position;
            listePanelsScores[1].transform.position = GameObject.Find("Position3").transform.position;
        }
        else if (GameManager.Instance.players.Count == 3)
        {
            listePanelsScores[0].transform.position = GameObject.Find("Position1").transform.position;
            listePanelsScores[1].transform.position = GameObject.Find("Position2").transform.position;
            listePanelsScores[2].transform.position = GameObject.Find("Position3").transform.position;
        }
        else
        {
            listePanelsScores[0].transform.position = GameObject.Find("Position1").transform.position;
            listePanelsScores[1].transform.position = GameObject.Find("Position2").transform.position;
            listePanelsScores[2].transform.position = GameObject.Find("Position3").transform.position;
            listePanelsScores[3].transform.position = GameObject.Find("Position4").transform.position;
        }
    }
}
