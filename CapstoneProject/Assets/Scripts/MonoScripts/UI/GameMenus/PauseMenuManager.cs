using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenuManager : InGameMenuController {

    public Button ResumeGame;
    public Button ChooseTeam;
    public Button Options;
    public Button ServerSettings;
    public Button Exit;

    protected override void Awake()
    {
        base.Awake();
        ToggleKey = KeyCode.Escape;
        ToggleEnabled = true;
    }

    public override void Show() {
        base.Show();
        ServerSettings.interactable = Network.isServer;
    }

    /************************************************************
	 * Button Press Handlers
	 ************************************************************/

    public void ChooseTeamPressed()
    {
        global::TeamMenuManager.Instance.Show();
        Hide();
    }

    public void ServerSettingsPressed()
    {
        global::ServerSettingsMenuManager.Instance.Show();
        Hide();
    }

    public void ExitPressed()
    {
        Hide();
        Network.Disconnect();
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static PauseMenuManager _instance;

    public static PauseMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<PauseMenuManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
