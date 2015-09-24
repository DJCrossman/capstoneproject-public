using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine.UI;

public class WeaponsMenuManager : InGameMenuController
{
    public GameObject PlayerCurrentWeapon;
    public List<Button> WeaponButtons;

    public Text PlayerNameText;
    public Text PlayerMoneyText;

    private UserController _userController;
    private TeamsController _teamsController;
    private HUDController _hudController;
    private List<WeaponModel> _weaponModels;
    private PlayerNetwork _playerNetwork;
    private WeaponModel _currentWeapon;
    private Weapon _weapon;

    protected override void Awake()
    {
        base.Awake();

        _teamsController = global::TeamsController.Instance;
        _userController = global::UserController.Instance;
        _hudController = global::HUDController.Instance;
        _weaponModels = new List<WeaponModel>(global::WeaponContainer.Instance.WeaponModels);
        _currentWeapon = _weaponModels.First(w => w.Type == WeaponType.Pistol);
        ToggleKey = KeyCode.B;
        ToggleEnabled = true;

    }

    public override void Show()
    {
        if ((_userController.GetPlayer().IsActive == false) && (_userController.GetDead() == false)) {
            _weapon = _userController.GetPlayer().PlayerWeapon;
            _playerNetwork = _userController.GetPlayer().PlayerNetwork;
            UpdateWeaponUi();
            PlayerNameText.text = _playerNetwork.GetUserModel().UserName;
            base.Show();
        }
    }


    public void SelectWeapon(Button button)
    {
        var buttonType = button.GetComponent<WeaponButton>().WeaponType;
        var model = _weaponModels.First(w => w.Type == buttonType);

        if (_playerNetwork.GetUserModel().Money < model.Cost)
        {
            Debug.Log("Not Enough Cash!");
            return;
        }

        // Update actual weapon and money values
        if (_currentWeapon.Type == model.Type) {
            _weapon.UpdateAmmo(_currentWeapon.Ammo);
        } else {
            _currentWeapon = model;
            _weapon.UpdateWeapon(_currentWeapon.Ammo, _currentWeapon.Damage, _currentWeapon.FireRate);
        }

        // Update user model
        _teamsController.UpdateUserModelForAll(_playerNetwork.GetUserModel().Uid, UserModelField.MoneyIncrement, _currentWeapon.Cost * -1);
        _teamsController.UpdateUserModelForAll(ServerData.serverInfo.userData.UID, UserModelField.Weapon, (int) buttonType);

        //Update UI
        UpdateWeaponUi();
        PlayerCurrentWeapon.GetComponentsInChildren<Image>().First(x => x.name == "Image").sprite = _currentWeapon.Sprite;
        PlayerCurrentWeapon.GetComponentsInChildren<Text>().First(x => x.name == "Name").text = _currentWeapon.Name;
        PlayerCurrentWeapon.GetComponentsInChildren<Text>().First(x => x.name == "Ammo").text = _weapon.GetAmmo().ToString();
        _hudController.AmmoText.text = _weapon.GetAmmo().ToString();
    }

    /**************************************************
	 * Helper Methods
	 **************************************************/

    private void UpdateWeaponUi()
    {
        PlayerMoneyText.text = _playerNetwork.GetUserModel().Money.ToString();  
        WeaponButtons.ForEach(b => b.interactable = _playerNetwork.GetUserModel().Money >= _weaponModels.First(m => m.Type == b.GetComponent<WeaponButton>().WeaponType).Cost);
    }
}
