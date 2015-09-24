#define DEBUG

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KickBanPlayerMenuManager : InGameMenuController
{
    public Text TitleText;

    private NetworkManager _networkManager;
    private UserModel _targetModel;
    private NetworkPlayer _targetNetworkPlayer;

    /**************************************************
     * Event Handlers
     **************************************************/

    protected override void Awake()
    {
        base.Awake();
        _networkManager = NetworkManager.Instance;
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public override void Show()
    {
        TitleText.text = "KICK/BAN " + _targetModel.UserName.ToUpper();
        base.Show();
    }

    /************************************************************
	 * Button Press Handlers
	 ************************************************************/
    public void KickPressed()
    {
        Network.CloseConnection(_targetNetworkPlayer, true);
        Hide();
    }

    public void BanPressed()
    {
        _networkManager.BannedUsers.Add(_targetModel.Uid);
        KickPressed();
    }

    public void BackPressed()
    {
        global::KickBanMenuManager.Instance.Show();
        Hide();
    }

    /**************************************************
     * Getters and Setters
     **************************************************/

    public void SetTarget(UserModel targetModel, NetworkPlayer targetNetworkPlayer)
    {
        _targetModel = targetModel;
        _targetNetworkPlayer = targetNetworkPlayer;
    }


    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static KickBanPlayerMenuManager _instance;

    public static KickBanPlayerMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<KickBanPlayerMenuManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}