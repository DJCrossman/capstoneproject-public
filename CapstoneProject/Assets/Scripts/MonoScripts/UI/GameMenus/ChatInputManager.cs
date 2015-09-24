using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MessageType
{
    Public,
    Team
};

public class ChatInputManager : MenuController
{
    private const KeyCode PrimaryToggle = KeyCode.Y;
    private const KeyCode AlternateToggle = KeyCode.U;

    public Text Placeholder;
    public Text Text;

    private ChatManager _chatManager;
    private MessageType _messageType;
    private InputField _inputField;
    private bool _isFirstCharacter = true;

    /**************************************************
     * Event Handlers
     **************************************************/

    protected override void Awake()
    {
        base.Awake();
        ToggleEnabled = false;
        _chatManager = ChatManager.Instance;
        _inputField = GetComponent<InputField>();
        _inputField.onEndEdit.AddListener(SendChatMessage);
        _inputField.onValueChange.AddListener(ValueChanged);
    }

    public override void OnGUI()
    {
        if ((Input.GetKeyDown(PrimaryToggle)) && (GetActive() == false)) {
            Show();
            _messageType = MessageType.Public;
            Placeholder.text = "Type message...";
        } else if ((Input.GetKeyDown(AlternateToggle)) && (GetActive() == false)) {
            Show();
            _messageType = MessageType.Team;
            Placeholder.text = "Type message to team...";
        }
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public override void Show()
    {
        base.Show();
        EventSystem.current.SetSelectedGameObject(gameObject, null);
        _inputField.OnPointerClick(new PointerEventData(EventSystem.current));
        _inputField.text = "";
        _isFirstCharacter = true;
    }

    private void SendChatMessage(string text)
    {
        if (text != "") {
            _chatManager.SendChatMessage(text, _messageType);
        }

        Hide();
    }

    private void ValueChanged(string text)
    {
        if (_isFirstCharacter == true) {
            _inputField.text = "";
            _isFirstCharacter = false;
        }
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static ChatInputManager _instance;

    public static ChatInputManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<ChatInputManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
