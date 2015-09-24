using UnityEngine;

public class InGameMenuController : MenuController
{
    private ChatInputManager _chatInputManager;

    protected override void Awake()
    {
        _chatInputManager = ChatInputManager.Instance;
        base.Awake();
    }
    public override void Show()
    {
        if (_chatInputManager.GetActive() == false) {
            base.Show(); 
        }
    }
}
