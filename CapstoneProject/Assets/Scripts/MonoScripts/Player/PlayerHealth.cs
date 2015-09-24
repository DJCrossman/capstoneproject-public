using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 100;

    private UserController _userController;
    private TeamsController _teamsController;
    private CameraController _cameraController;
    private HUDController _hudController;

    /************************************************************
     * Event Handlers
     ************************************************************/

    private void Awake()
    {
        enabled = networkView.isMine; //disable component unless it belongs to me
    }

    private void Start()
    {
        _teamsController = global::TeamsController.Instance;
        _cameraController = global::CameraController.Instance;
        _hudController = global::HUDController.Instance;
        _hudController.UpdateHealthHUD(Health);
        _userController = global::UserController.Instance;
        _userController.SetDead(false);
    }

    /************************************************************
     * Control Methods
     ************************************************************/

    // executes when this player is hit
    public void Hit(int damage, Guid killerUid)
    {
        Health -= damage;
        _hudController.UpdateHealthHUD(Health);

        if (Health <= 0) {
            if (_userController.GetPlayer().PlayerNetwork.GetUserModel().Dead == false) {
                PlayerDead(killerUid);
            }
        }
    }

    private void PlayerDead(Guid killerUid)
    {
        var playerNetwork = _userController.GetPlayer().PlayerNetwork;

        _teamsController.UpdateUserModelForAll(killerUid, UserModelField.KillsIncrement);
        _teamsController.UpdateUserModelForAll(killerUid, UserModelField.MoneyIncrement, Health);
        _teamsController.UpdateUserModelForAll(killerUid, UserModelField.PointsIncrement, Health);
        _teamsController.UpdateUserModelForAll(playerNetwork.GetUserModel().Uid, UserModelField.DeathsIncrement);
        _teamsController.UpdateUserModelForAll(playerNetwork.GetUserModel().Uid, UserModelField.Dead, true);
        _cameraController.ChangeCameraView(1, 3);
        _userController.SetDead(true);
        networkView.RPC("KillForAll", RPCMode.All);
    }
}
