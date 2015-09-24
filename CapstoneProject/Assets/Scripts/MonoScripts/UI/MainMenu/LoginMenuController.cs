using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoginMenuController : MenuController
{
    public InputField Email;
    public InputField Password;
    public Toggle RemMyPassword;
    public Button LoginButton;
    public Button CancelButton;
    public Button CreateAccountButton;

    protected override void Awake ()
    {
        base.Awake();   // Bug: must be included in every child of MenuController (due to Unity bug)
        LoginButton.interactable = false;
	}

    public void EnableLoginButton()
    {
        if(Email && Password)
            LoginButton.interactable = (Email.text != "" && Password.text != "");
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

    public void ClearForm()
    {
        Email.text = "";
        Password.text = "";
        LoginButton.interactable = (Email.text != "" && Password.text != "");
    }

    public override void OnGUI() { }
}
