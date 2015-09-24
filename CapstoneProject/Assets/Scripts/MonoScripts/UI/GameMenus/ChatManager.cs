using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    private const int UsernameCharacterCutoff = 13;     // Number of characters shown of username
    private const int UsernameTextFieldLimit = 65;      // Max length of username text field (13 * 5)
    private const int MessageTextFieldLimit = 400;      // Max length of message text field (80 * 5)

    public Text ChatTeam1UserText;
    public Text ChatTeam2UserText;
    public Text ChatMessageText;
    
    private UserController _userController;

    /**************************************************
     * Event Handlers
     **************************************************/

    private void Awake()
    {
        _userController = UserController.Instance;
    }

    private void Start()
    {
        ChatTeam1UserText.color = ColorConfiguration.Team1.baseColor;
        ChatTeam2UserText.color = ColorConfiguration.Team2.baseColor;
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public void SendChatMessage(string text, MessageType message)
    {
        networkView.RPC("SendReceiveMessage", RPCMode.All, new object[] {
            ServerData.serverInfo.userData.UserName,
            text,
            (int) message,
            ((int) _userController.GetTeam()),
            _userController.GetDead(),
        });
    }

    private void UpdateChat(string username, string text, Team team)
    {
        if (team == Team.Team1) {
            string usernameField = username.Substring(0, Mathf.Min(UsernameCharacterCutoff, username.Length))
                + Environment.NewLine
                + ChatTeam1UserText.text;
            ChatTeam1UserText.text = usernameField.Substring(0, Mathf.Min(usernameField.Length, UsernameTextFieldLimit));
            ChatTeam2UserText.text = Environment.NewLine + ChatTeam2UserText.text;
        } else {
            string usernameField = username.Substring(0, Mathf.Min(UsernameCharacterCutoff, username.Length))
                + Environment.NewLine
                + ChatTeam2UserText.text;
            ChatTeam2UserText.text = usernameField.Substring(0, Mathf.Min(usernameField.Length, UsernameTextFieldLimit));
            ChatTeam1UserText.text = Environment.NewLine + ChatTeam1UserText.text;
        }

        string messageField = text + Environment.NewLine + ChatMessageText.text;
        ChatMessageText.text = messageField.Substring(0, Mathf.Min(messageField.Length, MessageTextFieldLimit));
    }
    
    /**************************************************
     * RPC Handlers
     **************************************************/

    [RPC]
    private void SendReceiveMessage(string username, string text, int type, int currentTeam, bool isDead)
    {
        var messageType = ((MessageType) type);
        var team = ((Team) currentTeam);

        if (isDead == true) {
            if ((_userController.GetDead() == true) && ((messageType == MessageType.Public) || ((messageType == MessageType.Team) && (team == _userController.GetTeam())))) {
                UpdateChat(username, text, team);
            }
        } else {
            if ((messageType == MessageType.Public) || ((messageType == MessageType.Team) && (team == _userController.GetTeam()))) {
                UpdateChat(username, text, team);
            }
        }
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static ChatManager _instance;

    public static ChatManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<ChatManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
