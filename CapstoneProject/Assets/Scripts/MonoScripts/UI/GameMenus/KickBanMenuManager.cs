using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KickBanMenuManager : InGameMenuController
{
    public Text[] PlayerTexts;
    public Button[] PlayerButtons;

    private TeamsController _teamsController;
    private KickBanPlayerMenuManager _kickBanPlayerMenuManager;

    /**************************************************
     * Event Handlers
     **************************************************/

    protected override void Awake()
    {
        base.Awake();
        ToggleKey = KeyCode.K;
        ToggleEnabled = true;
        _teamsController = TeamsController.Instance;
    }

    private void Start()
    {
        enabled = Network.isServer;

        _kickBanPlayerMenuManager = global::KickBanPlayerMenuManager.Instance;
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public override void Show()
    {
        int i = 0;

        // Loop through every player on server and create kick/ban button
        var userModels = _teamsController.GetUserModels();
        for (int j = 0; j < userModels.Count; j++) {
            if (CreateButton(userModels[j], _teamsController.GetUserNetworkPlayers()[j], i) == true) {
                i++;
            }
        }

        // Disable any remaining buttons in the menu
        while (i < 7) {
            PlayerButtons[i].enabled = false;
            i++;
        }
        base.Show();
    }

    private bool CreateButton(UserModel userModel, NetworkPlayer networkPlayer, int index)
    {
        if (networkPlayer == Network.player) return false;

        PlayerButtons[index].enabled = true;
        PlayerButtons[index].onClick.AddListener(() =>
        {
            _kickBanPlayerMenuManager.SetTarget(userModel, networkPlayer);
            _kickBanPlayerMenuManager.Show();
            Hide();
        });
        PlayerTexts[index].text = userModel.UserName;
        return true;
    }

    /**************************************************
     * Button Press Handlers
     **************************************************/

    public void BackPressed()
    {
        Hide();
        global::ServerSettingsMenuManager.Instance.Show();
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static KickBanMenuManager _instance;

    public static KickBanMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<KickBanMenuManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}