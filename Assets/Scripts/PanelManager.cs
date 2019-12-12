using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine.SceneManagement;

public enum CurrentMenu
{
    MainMenu,
    HowToPlayMenu,
    PlayMenu
}
public class PanelManager : MonoBehaviour
{

    public Animator initiallyOpen;

    private int m_OpenParameterId;
    private Animator m_Open;
    private GameObject m_PreviouslySelected;
    private bool cursorCooldown;
    const string k_OpenTransitionName = "Open";
    const string k_ClosedStateName = "Closed";
    public Button btnStart;
    public Button btnHowToPlay;
    public Button btnQuit;
    public Button btnReturnPlay;
    public Button btnReturnHowto;
    public Button btnPlay;
    public Animator animHowTo;
    public Animator animControls;
    public Animator animMain;
    public Animator animPlay;

    public List<PlayerJoinScript> playersList;
    public CurrentMenu menu;
    public Button selectedBtn;
    //rewired join
    private int maxPlayers = 4;
    private int rewiredPlayerIdCounter = 0;
    public List<int> assignedJoysticks;
    private int playerReady = 0;


    private void Start()
    {
        SoundPlayer.Instance.Play("MenuTheme");
    }
    public void Update()
    {
        if (!cursorCooldown)
        {
            for (int i = 0; i < ReInput.players.playerCount; i++)
            {
                Player player = ReInput.players.GetPlayer(i);
                float yp = player.GetAxis("Move Horizontal");
                if (yp == 0) yp = player.GetAxis("Look Vertical");
                if (yp > 0)
                {
                    SwitchButton(false);
                    Cursor.visible = false;
                    StartCoroutine(Cooldown());
                }
                else if (yp < 0)
                {
                    SwitchButton(true);
                    Cursor.visible = false;
                    StartCoroutine(Cooldown());
                }
            }
            Player p = ReInput.players.GetSystemPlayer();
            float y = p.GetAxis("Move Horizontal");
            if (y == 0) y = p.GetAxis("Look Vertical");
            if (y > 0)
            {
                SwitchButton(false);
                Cursor.visible = false;
                StartCoroutine(Cooldown());
            }
            else if (y < 0)
            {
                SwitchButton(true);
                Cursor.visible = false;
                StartCoroutine(Cooldown());
            }
        }
        ControllerPollingInfo pB = ReInput.controllers.polling.PollAllControllersForFirstButtonDown();
        string sb = pB.elementIdentifierName;
        if (!string.IsNullOrEmpty(sb) && !sb.Contains("Mouse"))
        {
            //Debug.Log(sb);
            if (sb.Equals("A"))
            {
                if (selectedBtn != null) selectedBtn.onClick.Invoke();

                if (selectedBtn == btnHowToPlay)
                {
                    selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Normal");
                    selectedBtn = btnReturnHowto;
                    selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Highlighted");
                }
            }
            if (sb.Equals("B"))
            {
                btnReturnHowto.onClick.Invoke();
                btnReturnPlay.onClick.Invoke();
            }
            if (sb.Equals("Return") && menu == CurrentMenu.PlayMenu)
            {
                int rewiredPlayerId = GetNextGamePlayerId();
                Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
                AssignKeyboardAndMouseToPlayer(rewiredPlayer);

                // Disable KB/Mouse Assignment category in System Player so it doesn't assign through the keyboard/mouse anymore
                ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Assignment");
                ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Mouse, "Assignment");
            }

        }
        if (!string.IsNullOrEmpty(sb) && sb.Contains("Mouse"))
        {
            if (selectedBtn != null) selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Normal");
            Cursor.visible = true;
        }
        if (menu == CurrentMenu.PlayMenu) UpdatePlayerJoin();
    }

    private void UpdatePlayerJoin()
    {
        if (ReInput.players.GetSystemPlayer().GetButtonDown("Join"))
        {
            AssignNextPlayer();
        }
    }
    public void Click(Button btn)
    {
        if (btn == btnHowToPlay)
        {
            animMain.SetBool(m_OpenParameterId, false);
            StartCoroutine(DisableDelay(animMain));

            animHowTo.gameObject.SetActive(true);
            animHowTo.SetBool(m_OpenParameterId, true);

            animControls.gameObject.SetActive(true);
            animControls.SetBool(m_OpenParameterId, true);
            menu = CurrentMenu.HowToPlayMenu;
        }
        else if (btn == btnStart)
        {
            animPlay.gameObject.SetActive(true);
            animMain.SetBool(m_OpenParameterId, false);
            animPlay.SetBool(m_OpenParameterId, true);
            StartCoroutine(DisableDelay(animMain));
            menu = CurrentMenu.PlayMenu;
        }
        else if (btn == btnQuit)
        {
            Quit();
        }
        else if (btn == btnReturnHowto)
        {
            animControls.SetBool(m_OpenParameterId, false);
            animHowTo.SetBool(m_OpenParameterId, false);

            StartCoroutine(DisableDelay(animControls));
            StartCoroutine(DisableDelay(animHowTo));

            animMain.gameObject.SetActive(true);
            animMain.SetBool(m_OpenParameterId, true);
            menu = CurrentMenu.MainMenu;
            // selectedBtn = btnStart;
            // selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Highlighted");

        }
        else if (btn == btnReturnPlay)
        {
            animMain.gameObject.SetActive(true);
            animMain.SetBool(m_OpenParameterId, true);

            animPlay.SetBool(m_OpenParameterId, false);
            StartCoroutine(DisableDelay(animPlay));
            menu = CurrentMenu.MainMenu;
            btnStart.animator.SetTrigger("Normal");
            btnHowToPlay.animator.SetTrigger("Normal");
            btnQuit.animator.SetTrigger("Normal");
            //selectedBtn = btnStart;
            //selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Highlighted");
            assignedJoysticks = new List<int>();
            foreach (var j in ReInput.controllers.Joysticks)
            {
                ReInput.players.GetSystemPlayer().controllers.AddController(j, true);
            }
            foreach (PlayerJoinScript pjs in playersList)
            {
                pjs.Quit();
            }
            playerReady = 0;
        }
        else if (btn == btnPlay)
        {
            if (assignedJoysticks.Count > 0)
            {
                PlayerPrefs.SetInt("nbPlayers", assignedJoysticks.Count);
                SceneManager.LoadScene("MasterScene");
            }
            else
            {
                PlayerPrefs.SetInt("nbPlayers", 0);
                SceneManager.LoadScene("MasterScene");
            }
        }
    }

    private void SwitchButton(bool down)
    {
        if (menu == CurrentMenu.MainMenu)
        {
            if (selectedBtn == null)
            {
                selectedBtn = btnStart;
                selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Highlighted");
                return;
            }
            selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Normal");
            if (selectedBtn == btnStart)
            {
                if (down) selectedBtn = btnHowToPlay;
                else selectedBtn = btnQuit;
            }
            else if (selectedBtn == btnHowToPlay)
            {
                if (down) selectedBtn = btnQuit;
                else selectedBtn = btnStart;
            }
            else if (selectedBtn == btnQuit)
            {
                if (down) selectedBtn = btnStart;
                else selectedBtn = btnHowToPlay;
            }
        }
        else if (menu == CurrentMenu.HowToPlayMenu)
        {
            selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Normal");
            selectedBtn = btnReturnHowto;
        }
        else if (menu == CurrentMenu.PlayMenu)
        {
            if (selectedBtn == btnStart)
            {
                btnStart.gameObject.GetComponent<Animator>().SetTrigger("Normal");
                selectedBtn = btnPlay;
            }
            else if (selectedBtn == btnPlay)
            {
                btnPlay.gameObject.GetComponent<Animator>().SetTrigger("Normal");
                selectedBtn = btnReturnPlay;
            }
            else if (selectedBtn == btnReturnPlay)
            {
                btnReturnPlay.gameObject.GetComponent<Animator>().SetTrigger("Normal");
                selectedBtn = btnPlay;

            }
        }
        selectedBtn.gameObject.GetComponent<Animator>().SetTrigger("Highlighted");
    }
    public void OnEnable()
    {
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, true);
        }
        m_OpenParameterId = Animator.StringToHash(k_OpenTransitionName);

        if (initiallyOpen == null)
            return;
        OpenPanel(initiallyOpen);
        if (ReInput.controllers.Joysticks.Count >= 0)
        {
            Cursor.visible = false;
        }
        menu = CurrentMenu.MainMenu;
        assignedJoysticks = new List<int>();
        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    public void OpenPanel(Animator anim)
    {
        if (m_Open == anim)
        {
            return;
        }
        anim.gameObject.SetActive(true);
        var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

        anim.transform.SetAsLastSibling();

        CloseCurrent();

        m_PreviouslySelected = newPreviouslySelected;

        m_Open = anim;
        m_Open.SetBool(m_OpenParameterId, true);

        GameObject go = FindFirstEnabledSelectable(anim.gameObject);

        SetSelected(go);
    }

    static GameObject FindFirstEnabledSelectable(GameObject gameObject)
    {
        GameObject go = null;
        var selectables = gameObject.GetComponentsInChildren<Selectable>(true);
        foreach (var selectable in selectables)
        {
            if (selectable.IsActive() && selectable.IsInteractable())
            {
                go = selectable.gameObject;
                break;
            }
        }
        return go;
    }

    public void CloseCurrent()
    {
        if (m_Open == null)
            return;
        m_Open.SetBool(m_OpenParameterId, false);
        SetSelected(m_PreviouslySelected);
        StartCoroutine(DisablePanelDeleyed(m_Open));
        m_Open = null;
    }

    IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            if (!anim.IsInTransition(0))
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(k_ClosedStateName);

            wantToClose = !anim.GetBool(m_OpenParameterId);

            yield return new WaitForEndOfFrame();
        }

        if (wantToClose)
            anim.gameObject.SetActive(false);
    }
    IEnumerator DisableDelay(Animator anim)
    {
        yield return new WaitForSeconds(0.6f);
        anim.gameObject.SetActive(false);
    }

    private void SetSelected(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(go);
    }

    private IEnumerator Cooldown()
    {
        cursorCooldown = true;
        yield return new WaitForSeconds(0.2f);
        cursorCooldown = false;
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
    }

    //rewired functions

    void AssignNextPlayer()
    {
        if (rewiredPlayerIdCounter >= maxPlayers)
        {
            Debug.Log("Max player limit already reached!");
            return;
        }

        // Get the next Rewired Player Id
        int rewiredPlayerId = GetNextGamePlayerId();

        // Get the Rewired Player
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Determine which Controller was used to generate the JoinGame Action
        Player systemPlayer = ReInput.players.GetSystemPlayer();
        var inputSources = systemPlayer.GetCurrentInputSources("Join");

        foreach (var source in inputSources)
        {
            if (source.controllerType == ControllerType.Keyboard || source.controllerType == ControllerType.Mouse)
            { // Assigning keyboard/mouse

                // Assign KB/Mouse to the Player
                AssignKeyboardAndMouseToPlayer(rewiredPlayer);

                // Disable KB/Mouse Assignment category in System Player so it doesn't assign through the keyboard/mouse anymore
                ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Assignment");
                ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Mouse, "Assignment");
                break;

            }
            else if (source.controllerType == ControllerType.Joystick)
            { // assigning a joystick

                // Assign the joystick to the Player. This will also un-assign it from System Player
                AssignJoystickToPlayer(rewiredPlayer, source.controller as Joystick);
                break;

            }
            else
            { // Custom Controller
                throw new System.NotImplementedException();
            }
        }

        // Enable UI map so Player can start controlling the UI
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI");
    }
    private void AssignKeyboardAndMouseToPlayer(Player player)
    {
        // Assign mouse to Player
        player.controllers.hasMouse = true;

        // Load the keyboard and mouse maps into the Player
        player.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "UI", "Default", true);
        player.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default", true);
        player.controllers.maps.LoadMap(ControllerType.Mouse, 0, "Default", "Default", true);

        // Exclude this Player from Joystick auto-assignment because it is the KB/Mouse Player now
        player.controllers.excludeFromControllerAutoAssignment = true;
        Join();
        Debug.Log("Assigned Keyboard/Mouse to Player " + player.name);
    }

    private void AssignJoystickToPlayer(Player player, Joystick joystick)
    {
        // Assign the joystick to the Player, removing it from System Player
        player.controllers.AddController(joystick, true);

        // Mark this joystick as assigned so we don't give it to the System Player again
        assignedJoysticks.Add(joystick.id);
        Join();
        Debug.Log("Assigned " + joystick.name + " to Player " + player.name);
    }

    private int GetNextGamePlayerId()
    {
        return rewiredPlayerIdCounter++;
    }
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;

        // Check if this Joystick has already been assigned. If so, just let Auto-Assign do its job.
        if (assignedJoysticks.Contains(args.controllerId)) return;

        // Joystick hasn't ever been assigned before. Make sure it's assigned to the System Player until it's been explicitly assigned
        ReInput.players.GetSystemPlayer().controllers.AddController(
            args.controllerType,
            args.controllerId,
            true // remove any auto-assignments that might have happened
        );
    }
    private void Join()
    {
        playerReady += 1;
        Debug.Log("playerReady : " + playerReady);
        Debug.Log("playerList : " + playersList.Count);
        if (playerReady < playersList.Count)
        {
            playersList[playerReady - 1].Join();
        }
    }
}


