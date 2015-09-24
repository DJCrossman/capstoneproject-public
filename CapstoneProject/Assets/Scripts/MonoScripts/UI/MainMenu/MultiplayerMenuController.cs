using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;

public class MultiplayerMenuController : MenuController
{
    private NetworkServices _networkServices;
    public CanvasGroup LoginMenu;
    public CanvasGroup CreateAccountMenu;
    public Button StartServerButton;
    public Button JoinServerButton;
    public Button SettingsButton;
    public Button BackButton;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();   // Unity Bug: must be included in every child of MenuController
        _networkServices = new NetworkServices();
    }

    public override void Show()
    {
        base.Show();
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        if (!_networkServices.isUserLoggedIn()) {
            // Checks if there is a Local account
            DisplayLoginMenu();
        } else {
            // Sets the GUI Username placeholder
            InitializeUserNameGUI();
        }
    }

    public override void Hide()
    {
        base.Hide();
        CanvasGroup.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_networkServices == null) {
            Application.LoadLevel("MainMenu");
            throw new Exception("Network Services had downs");
        }
        _networkServices.UpdateHostData();
        JoinServerButton.interactable = !_networkServices.isHostDataEmpty();
    }

    public void Login()
    {
        var controller = LoginMenu.GetComponent<LoginMenuController>();
        _networkServices.LogIn(controller.Email.text, controller.Password.text, controller.RemMyPassword.isOn);
        if (_networkServices.isUserLoggedIn()) {
            controller.Hide();
            CanvasGroup.blocksRaycasts = true;
            InitializeUserNameGUI();
        }
    }

    public void CreateAccount()
    {
        var loginController = LoginMenu.GetComponent<LoginMenuController>();
        var accountController = CreateAccountMenu.GetComponent<CreateAccountMenuController>();
        _networkServices.CreateAccount(accountController.UserName.text, accountController.Email.text, accountController.Password.text);
        accountController.Hide();
        loginController.Show();
    }

    public void LogOut()
    {
        _networkServices.LogOut();
    }

    /**************************************************
     * Button Methods
     **************************************************/

    public void StartServer()
    {
        _networkServices.StartServer();
    }

    public void JoinServer()
    {
        _networkServices.JoinServer();
    }

    public void RequestHostData()
    {
        _networkServices.RefreshHostList();
    }

    /**************************************************
     * GUI Methods
     **************************************************/

    private void InitializeUserNameGUI()
    {
        List<Text> components = new List<Text>(this.gameObject.GetComponentsInChildren<Text>());
        var placeholder = components.Find(c => c.name == "UsernamePlaceholder");
        placeholder.text = _networkServices.GetUserName();
    }

    public void SetUserNameFromGUI()
    {
        List<Text> components = new List<Text>(this.gameObject.GetComponentsInChildren<Text>());

        var text = components.Find(c => c.name == "Text");
        var placeholder = components.Find(c => c.name == "UsernamePlaceholder");

        _networkServices.SetUserName(text.text);
        placeholder.text = text.text;
        text.text = "";
    }

    private void DisplayLoginMenu()
    {
        CanvasGroup.blocksRaycasts = false;
        var menu = LoginMenu.GetComponent<LoginMenuController>();
        menu.Show();
    }

    public override void OnGUI() { }
}
