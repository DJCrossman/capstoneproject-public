using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class CreateAccountMenuController : MenuController
{
    public InputField UserName;
    public InputField Email;
    public InputField Password;
    public InputField VerifyPassword;
    public Button CreateAccountButton;
    public Button CancelButton;

    protected override void Awake()
    {
        base.Awake();   // Bug: must be included in every child of MenuController (due to Unity bug)
        CreateAccountButton.interactable = false;
    }

    public void EnableLoginButton()
    {
        if (!UserName || !Email || !Password || !VerifyPassword) return;
        var notEmpty = UserName.text != "" && Email.text != "" && Password.text != "" && VerifyPassword.text != "";
        var validPassword = Password.text == VerifyPassword.text;
        CreateAccountButton.interactable = notEmpty && validPassword;
        if (!validPassword && notEmpty)
            Debug.Log("Passwords are not the same.");
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
        UserName.text = "";
        Email.text = "";
        Password.text = "";
        VerifyPassword.text = "";
    }

    public override void OnGUI() {}
}
