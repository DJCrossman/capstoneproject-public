using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerSettingsMenuManager :InGameMenuController
{
    public Button StartMatch;
    public Button RoundsPerMatchUp;
    public Button RoundsPerMatchDown;
    public Button EndOfRoundUp;
    public Button EndOfRoundDown;
    public Button WarmupTimeUp;
    public Button WarmupTimeDown;
    public Button RoundTimeUp;
    public Button RoundTimeDown;
    public Button FriendlyFire;
    public Button KickPlayer;
    public Button BanPlayer;
    public Button Back;
    public Text RoundsPerMatchText;
    public Text EndOfRoundText;
    public Text WarmupTimeText;
    public Text RoundTimeText;
    public Text FriendlyFireText;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

    protected override void Awake()
    {
        base.Awake();
        ToggleKey = KeyCode.S;
        ToggleEnabled = true;
    }

    public void Start()
    {
        UpdateEndOfRoundText();
        UpdateRoundsPerMatchText();
        UpdateWarmupTimeText();
        UpdateRoundTimeText();
        UpdateFriendlyFireText(ServerData.serverInfo.settings.FriendlyFire);
    }

    /************************************************************
	 * Control Methods
	 ************************************************************/

    public override void Show()
    {
        if (Network.isServer == true) base.Show();
    }

    private void UpdateRoundsPerMatchText()
    {
        RoundsPerMatchText.text = "ROUNDS/MATCH = " + ServerData.serverInfo.settings.RoundsInMatch;
    }

    private void UpdateEndOfRoundText()
    {
        EndOfRoundText.text = "END OF ROUND = " + ((int) (ServerData.serverInfo.settings.EndOfRoundTime)) + "s";
    }

    private void UpdateWarmupTimeText()
    {
        WarmupTimeText.text = "WARMUP TIME = " + ((int) (ServerData.serverInfo.settings.WarmupTime)) + "s";
    }

    private void UpdateRoundTimeText() {
        int minutes = ((int) (ServerData.serverInfo.settings.RoundTime/60));
        int seconds = ((int) (ServerData.serverInfo.settings.RoundTime - (60*minutes)));
        string filler = seconds > 10 ? ":" : ":0";

        RoundTimeText.text = "ROUND TIME = " + minutes + filler + seconds + "m";
    }

    private void UpdateFriendlyFireText(bool friendlyFire)
    {
        if (friendlyFire == true) {
            FriendlyFireText.text = "FRIENDLY FIRE IS ON";
        } else {
            FriendlyFireText.text = "FRIENDLY FIRE IS OFF";
        }
    }

    /************************************************************
	 * Button Press Handlers
	 ************************************************************/

    public void RoundsPerMatchIncrease()
    {
        ServerData.serverInfo.settings.RoundsInMatch += 2;
        networkView.RPC("SetRoundsPerMatchRpc", RPCMode.Others, ServerData.serverInfo.settings.RoundsInMatch);
        networkView.RPC("SetRoundsPerMatchRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.RoundsInMatch);
        UpdateRoundsPerMatchText();
    }

    public void RoundsPerMatchDecrease()
    {
        if (ServerData.serverInfo.settings.RoundsInMatch <= 1) return;
        ServerData.serverInfo.settings.RoundsInMatch -= 2;
        networkView.RPC("SetRoundsPerMatchRpc", RPCMode.Others, ServerData.serverInfo.settings.RoundsInMatch);
        networkView.RPC("SetRoundsPerMatchRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.RoundsInMatch);
        UpdateRoundsPerMatchText();
    }

    public void EndOfRoundIncrease()
    {
        ServerData.serverInfo.settings.EndOfRoundTime += 1;
        networkView.RPC("SetEndOfRoundTimeRpc", RPCMode.Others, ServerData.serverInfo.settings.EndOfRoundTime);
        networkView.RPC("SetEndOfRoundTimeRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.EndOfRoundTime);
        UpdateEndOfRoundText();
    }

    public void EndOfRoundDecrease()
    {
        if (ServerData.serverInfo.settings.EndOfRoundTime <= 0) return;
        ServerData.serverInfo.settings.EndOfRoundTime -= 1;
        networkView.RPC("SetEndOfRoundTimeRpc", RPCMode.Others, ServerData.serverInfo.settings.EndOfRoundTime);
        networkView.RPC("SetEndOfRoundTimeRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.EndOfRoundTime);
        UpdateEndOfRoundText();
    }

    public void WarmupTimeIncrease()
    {
        ServerData.serverInfo.settings.WarmupTime += 1;
        networkView.RPC("SetWarmupTimeRpc", RPCMode.Others, ServerData.serverInfo.settings.WarmupTime);
        networkView.RPC("SetWarmupTimeRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.WarmupTime);
        UpdateWarmupTimeText();
    }

    public void WarmupTimeDecrease()
    {
        if (ServerData.serverInfo.settings.WarmupTime <= 0) return;
        ServerData.serverInfo.settings.WarmupTime -= 1;
        networkView.RPC("SetWarmupTimeRpc", RPCMode.Others, ServerData.serverInfo.settings.WarmupTime);
        networkView.RPC("SetWarmupTimeRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.WarmupTime);
        UpdateWarmupTimeText();
    }

    public void RoundTimeIncrease()
    {
        ServerData.serverInfo.settings.RoundTime += 30;
        networkView.RPC("SetRoundTimeRpc", RPCMode.Others, ServerData.serverInfo.settings.RoundTime);
        networkView.RPC("SetRoundTimeRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.RoundTime);
        UpdateRoundTimeText();
    }

    public void RoundTimeDecrease() {
        if (ServerData.serverInfo.settings.RoundTime <= 30) return;
        ServerData.serverInfo.settings.RoundTime -= 30;
        networkView.RPC("SetRoundTimeRpc", RPCMode.Others, (float) ServerData.serverInfo.settings.RoundTime);
        networkView.RPC("SetRoundTimeRpc", RPCMode.AllBuffered, (float) ServerData.serverInfo.settings.RoundTime);
        UpdateRoundTimeText();
    }

    public void FriendlyFireToggle()
    {
        ServerData.serverInfo.settings.FriendlyFire = !ServerData.serverInfo.settings.FriendlyFire;
        networkView.RPC("SetFriendlyFireRpc", RPCMode.Others, ServerData.serverInfo.settings.FriendlyFire);
        networkView.RPC("SetFriendlyFireRpc", RPCMode.AllBuffered, ServerData.serverInfo.settings.FriendlyFire);
        UpdateFriendlyFireText(ServerData.serverInfo.settings.FriendlyFire);
    }

    public void KickBanPressed()
    {
        global::KickBanMenuManager.Instance.Show();
        Hide();
    }

    public void BackPressed()
    {
        Hide();
        global::PauseMenuManager.Instance.Show();
    }

    /**************************************************
     * RPC Handlers
     **************************************************/

    [RPC]
    private void SetRoundsPerMatchRpc(int roundsPerMatch)
    {
        ServerData.serverInfo.settings.RoundsInMatch = roundsPerMatch;
    }

    [RPC]
    private void SetEndOfRoundTimeRpc(int endOfRoundTime)
    {
        ServerData.serverInfo.settings.EndOfRoundTime = endOfRoundTime;
    }

    [RPC]
    private void SetWarmupTimeRpc(int warmupTime)
    {
        ServerData.serverInfo.settings.WarmupTime = warmupTime;
    }

    [RPC]
    private void SetRoundTimeRpc(float roundTime)
    {
        ServerData.serverInfo.settings.RoundTime = (double) roundTime;
    }

    [RPC]
    private void SetFriendlyFireRpc(bool friendlyFire)
    {
        ServerData.serverInfo.settings.FriendlyFire = friendlyFire;
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static ServerSettingsMenuManager _instance;

    public static ServerSettingsMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<ServerSettingsMenuManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}