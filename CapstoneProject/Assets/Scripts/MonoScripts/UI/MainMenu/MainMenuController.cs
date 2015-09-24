using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MenuController
{
    public Button StoryButton;
    public Button MultiplayerButton;
    public Button SettingsButton;

    protected override void Awake()
    {
        base.Awake();

        global::NetworkManager.Reset();
        global::TeamsController.Reset();
        global::SpawnController.Reset();
        global::UserController.Reset();
        global::MatchController.Reset();
        global::CaptureController.Reset();
        global::CameraController.Reset();
        global::HUDController.Reset();
        global::PauseMenuManager.Reset();
        global::ServerSettingsMenuManager.Reset();
        global::KickBanPlayerMenuManager.Reset();
        global::KickBanMenuManager.Reset();
        global::TeamMenuManager.Reset();
        global::MatchRoundMenuManager.Reset();
        global::GameSummaryMenuManager.Reset();
        global::ChatInputManager.Reset();
        global::ChatManager.Reset();
        global::ElevatorContainer.Reset();
        global::WeaponContainer.Reset();
    }

    public override void Show()
    {
        base.Show();
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
    }

    public override void Hide()
    {
        base.Hide();
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}