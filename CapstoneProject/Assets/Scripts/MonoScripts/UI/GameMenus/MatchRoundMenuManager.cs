using UnityEngine;
using UnityEngine.UI;

public class MatchRoundMenuManager : MenuController
{
    public Text PromptText;
    public Text NumberText;
    public AudioClip CountDownSound;
    public AudioClip StartSound;

    private int _index = 0;
    private double _countdownTime = 0;
    private bool _counting = false;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

    void FixedUpdate() {
        // If counting is true, add delta time to elapsed time until it is greater than countdown time.
        if (_counting == true) {
            _countdownTime -= Time.fixedDeltaTime;

            if (_countdownTime <= 0) {
                audio.clip = StartSound;
                audio.Play();
                _counting = false;  // stop counting down
                Hide();             // hide the countdown
            } else if (_countdownTime < _index) {
                _index--;
                audio.clip = CountDownSound;
                audio.Play();
                NumberText.text = ((int) _countdownTime + 1).ToString();
            }
        }
    }

    /************************************************************
	 * Control Methods
	 ************************************************************/

    public override void Show() {
        base.Show();
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }

    public void StartCountdown()
    {
        networkView.RPC("StartCountdownRpc", RPCMode.All);
    }

    public void ShowForAll()
    {
        networkView.RPC("ShowRpc", RPCMode.All);
    }

    public void HideForAll()
    {
        networkView.RPC("HideRpc", RPCMode.All);
    }

    /************************************************************
	 * Getters and Setter
	 ************************************************************/

    public void SetPrompt(string message)
    {
        networkView.RPC("SetPromptRpc", RPCMode.All, message);
    }

    public void SetCountdownTime(int countdownTime)
    {
        networkView.RPC("SetCountdownTimeRpc", RPCMode.All, countdownTime);
    }

    /************************************************************
     * RPC Handlers
     ************************************************************/

    [RPC]
    private void SetPromptRpc(string message, NetworkMessageInfo networkMessageInfo)
    {
        PromptText.text = message;
    }

    [RPC]
    private void SetCountdownTimeRpc(int seconds)
    {
        _countdownTime = ((double) seconds);
        _index = seconds;
    }

    [RPC]
    private void StartCountdownRpc()
    {
        Show();
        _counting = true;
    }

    [RPC]
    private void ShowRpc()
    {
        NumberText.text = "";
        Show();
    }

    [RPC]
    private void HideRpc()
    {
        Hide();
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static MatchRoundMenuManager _instance;

    public static MatchRoundMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<MatchRoundMenuManager>()); }
    }
    public static void Reset()
    {
        _instance = null;
    }

}
