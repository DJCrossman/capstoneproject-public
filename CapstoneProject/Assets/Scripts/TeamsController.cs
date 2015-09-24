using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class TeamsController : MonoBehaviour 
{
    private const double SecondsBetweenPing = 3;

    public Text Team1MemberText;
    public Text Team2MemberText;

    private MatchController _matchController;
    private CaptureController _captureController;
    private SpawnController _spawnController;
    private List<NetworkPlayer> _userNetworkPlayers;
    private List<UserModel> _userModels;
    private UserController _userController;
    private TeamMenuManager _teamMenuManager;
    private WeaponContainer _weaponContainer;
    private double _nextPing;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

    private void Awake() 
    {
        _captureController = global::CaptureController.Instance;
        _spawnController = global::SpawnController.Instance;
        _teamMenuManager = global::TeamMenuManager.Instance;
        _userController = global::UserController.Instance;
        _weaponContainer = global::WeaponContainer.Instance;
    }

    private void Start() {
        if (Network.isServer == true) {
            _matchController = MatchController.Instance;
            _userNetworkPlayers = new List<NetworkPlayer>();
        }

        _userModels = new List<UserModel>();
    }

    private void FixedUpdate()
    {
        if (Network.isServer == true) {
            if (_nextPing <= 0) {
                _nextPing = SecondsBetweenPing;
                for (int i = 0; i < _userModels.Count; i++) {
                    if (_userNetworkPlayers[i] == Network.player) {
                        UpdateUserModelForAll(_userModels[i].Uid, UserModelField.Ping, 0);
                    } else {
                        UpdateUserModelForAll(_userModels[i].Uid, UserModelField.Ping, Network.GetAveragePing(_userNetworkPlayers[i]));
                    }
                }
            } else {
                _nextPing -= Time.fixedDeltaTime;
            }
        }
    }

    /************************************************************
	 * Control Methods
	 ************************************************************/

    /**
	 * Desc:	Remove a player from either team on every client.
	 * Params:	The NetworkPlayer of the player wished to remove.
	 * Return:	None.
	 * SideFX:	RPC to RemovePlayerFromTeam sent to all clients.
	 */

    public void AddNetworkPlayer(NetworkPlayer networkPlayer, string userName, Guid uid) {
        if (Network.isClient) {
            Debug.LogWarning("AddNetworkPlayer function called from client.");
            return;
        }

        // Add player to server dicitionary of connected users
        var userModel = new UserModel() {
            UserName = userName,
            Uid = uid,
            Dead = true,
            ColorIndex = -1,
            CurrentTeam = ((int) Team.None),
            NextTeam = ((int) Team.None)
        };

        int position = _userModels.Count;
        _userNetworkPlayers.Add(networkPlayer);
        _userModels.Add(userModel);

        // Add player to list of connected users on all clients
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, userModel);
        string userModelSerialization = Convert.ToBase64String(memoryStream.GetBuffer());
        networkView.RPC("AddConnectedUserRpc", RPCMode.Others, userModelSerialization);
    }

    /**
	 * Desc:	Remove a player from either team on every client.
	 * Params:	The NetworkPlayer of the player wished to remove.
	 * Return:	None.
	 * SideFX:	RPC to RemovePlayerFromTeam sent to all clients.
	 */

    public void RemoveNetworkPlayer(NetworkPlayer networkPlayer) {
        if (Network.isClient) {
            Debug.LogWarning("RemoveNetworkPlayer function called from client.");
            return;
        }

        Guid uid = new Guid();
        for (int i = 0; i < _userModels.Count; i++) {
            if (_userNetworkPlayers[i] == networkPlayer) {
                uid = _userModels[i].Uid;
                _userModels.RemoveAt(i);
                _userNetworkPlayers.RemoveAt(i);
            }
        }

        // Remove player from list of connected users on all clients
        networkView.RPC("RemoveConnectedUserRpc", RPCMode.Others, uid.ToString());
    }

    /**
	 * Desc:	Removes the player from either team.
	 * Params:	The UID of the player wished to remove.
	 * Return:	None.
	 * SideFX:	Controllers of each team tries to remove player.
	 */

    public void SendConnectedUsers(NetworkPlayer networkPlayer) {
        if (Network.isServer == false) {
            Debug.LogWarning("Client is trying to send connected users information.");
            return;
        }

        var memoryStream = new MemoryStream();
        new BinaryFormatter().Serialize(memoryStream, _userModels);
        string userModelsSerialization = Convert.ToBase64String(memoryStream.GetBuffer());
        networkView.RPC("GetConnectedUsers", networkPlayer, userModelsSerialization);
    }

    public void UpdatePlayerMortality(Guid uid, Team team) 
    {
        if (Network.isServer) {
            _captureController.RemovePlayer(uid);
            if (_userModels.FirstOrDefault(i => ((i.CurrentTeam == ((int) team)) && (i.Dead == false))) == null) {
                _matchController.AllPlayersDead(team);
            }
        }
    }

    public void UpdateUserModelForAll(Guid uid, UserModelField field, System.Object obj = null)
    {
        var fieldParam = (int)field;
        string serialization = "";

        if (obj != null) {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, obj);
            serialization = Convert.ToBase64String(memoryStream.GetBuffer());
        }

        UpdateUserModelProperty(uid, field, obj);

        networkView.RPC("UpdateUserModelRpc", RPCMode.Others, new object[]
        {
            uid.ToString(),
            fieldParam, 
            serialization
        });
    }

    /************************************************************
	 * Helper Methods
	 ************************************************************/

    private void UpdateUserModelProperty(Guid uid, UserModelField userModelField, System.Object obj = null)
    {
        // Loop through every model looking for the correct Guid
        for (int i = 0; i < _userModels.Count; i++) {

            // When Guid found, modify the corresponding field in the model
            if (_userModels[i].Uid == uid) {
                switch (userModelField) {
                    case UserModelField.Ping:
                        _userModels[i].Ping = (int)obj;
                        break;
                    case UserModelField.Weapon:
                        _userModels[i].Weapon = (int)obj;
                        UserModelWeaponChangeListener(i, uid);
                        break;
                    case UserModelField.Kills:
                        _userModels[i].Kills = (int)obj;
                        break;
                    case UserModelField.KillsIncrement:
                        _userModels[i].Kills++;
                        break;
                    case UserModelField.Deaths:
                        _userModels[i].Deaths = (int)obj;
                        break;
                    case UserModelField.DeathsIncrement:
                        _userModels[i].Deaths++;
                        break;
                    case UserModelField.Dead:
                        _userModels[i].Dead = (bool)obj;
                        break;
                    case UserModelField.Money:
                        _userModels[i].Money = (int)obj;
                        break;
                    case UserModelField.MoneyIncrement:
                        _userModels[i].Money += (int)obj;
                        break;
                    case UserModelField.Points:
                        _userModels[i].Points = (int)obj;
                        break;
                    case UserModelField.PointsIncrement:
                        _userModels[i].Points += (int)obj;
                        break;
                    case UserModelField.ColorIndex:
                        _userModels[i].ColorIndex = (int)obj;
                        break;
                    case UserModelField.NextTeam:
                        _userModels[i].NextTeam = (int)obj;
                        UserModelTeamChangeListener(i);
                        break;
                    case UserModelField.CurrentTeam:
                        _userModels[i].CurrentTeam = (int)obj;
                        break;
                    case UserModelField.Username:
                        _userModels[i].UserName = (string)obj;
                        break;
                    case UserModelField.Uid:
                        _userModels[i].Uid = new Guid((string)obj);
                        break;
                    case UserModelField.NetworkIdHash:
                        _userModels[i].NetworkIdHash = (int)obj;
                        break;
                }
                break;
            }
        }
    }

    private void UserModelWeaponChangeListener(int index, Guid uid)
    {
        Sprite sprite = new Sprite();
        var weaponType = (WeaponType) _userModels[index].Weapon;

        foreach (var weaponModel in _weaponContainer.WeaponModels) {
            if (weaponModel.Type == weaponType) {
                sprite = weaponModel.Sprite;
            }
        }

        Array.ForEach(FindObjectsOfType<Player>(), player => {
            if (player.PlayerNetwork.GetUserModel().Uid == uid) {
                player.PlayerWeapon.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        });
    }

    private void UserModelTeamChangeListener(int index)
    {
        UpdateTeamMembersText();   
    }

    private void UpdateTeamMembersText() {
        Team1MemberText.text = Team2MemberText.text = "";

        _userModels.ForEach(userModel => {
            var team = ((Team)userModel.NextTeam);

            if (team == Team.Team1) {
                Team1MemberText.text = userModel.UserName + Environment.NewLine + Team1MemberText.text;
            }
            else if (team == Team.Team2) {
                Team2MemberText.text = userModel.UserName + Environment.NewLine + Team2MemberText.text;
            }
        });
    }

    /************************************************************
	 * Getters and Setters
	 ************************************************************/

    public List<UserModel> GetUserModels()
    {
        return _userModels;
    }

    public List<NetworkPlayer> GetUserNetworkPlayers()
    {
        return _userNetworkPlayers;
    }

    public int GetNumberOfNextTeamMembers(Team team) 
    {
        return GetUserModels().Count(i => i.NextTeam == ((int) team));
    }

    public int GetNumberOfCurrentTeamMembers(Team team)
    {
        return GetUserModels().Count(i => i.CurrentTeam == ((int)team));
    }

    /************************************************************
	 * Remote Procedure Call Handlers
	 ************************************************************/
    
    [RPC]
    private void UpdateUserModelRpc(string uid, int userModelField, string serialization)
    {
        System.Object obj = null;

        // Deseiralize data
        if (serialization != "") {
            var memoryStream = new MemoryStream(Convert.FromBase64String(serialization));
            obj = new BinaryFormatter().Deserialize(memoryStream);
        }

        UpdateUserModelProperty(new Guid(uid), (UserModelField) userModelField, obj);
    }

    /**
	 * Desc:	Removes the player from either team.
	 * Params:	The UID of the player wished to remove.
	 * Return:	None.
	 * SideFX:	Controllers of each team tries to remove player.
	 */

    [RPC]
    private void GetConnectedUsers(string connectedUsersSerialization)
    {
        // Unserialize UserModels data.
        var memoryStream = new MemoryStream(Convert.FromBase64String(connectedUsersSerialization));
        _userModels = ((List<UserModel>) new BinaryFormatter().Deserialize(memoryStream));

        // Set the UserIndex for every alive player
        Array.ForEach<Player>(FindObjectsOfType<Player>(), player => {
            bool deletePlayerObject = true;

            for (int i = 0; i < _userModels.Count; i++) {
                if (_userModels[i].NetworkIdHash == player.networkView.viewID.GetHashCode()) {
                    player.PlayerNetwork.SetUserIndex(i);
                    deletePlayerObject = _userModels[i].Dead;
                    break;
                }
            }

            if (deletePlayerObject) Destroy(player.gameObject);
        });

        UpdateTeamMembersText();
        _teamMenuManager.Show();
    }

    [RPC]
    private void AddConnectedUserRpc(string userModelSerialization) 
    {
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream(Convert.FromBase64String(userModelSerialization));
        var userModel = (UserModel)binaryFormatter.Deserialize(memoryStream);
        _userModels.Add(userModel);
    }

    [RPC]
    private void RemoveConnectedUserRpc(string uid)
    {
        var guid = new Guid(uid);

        for (int i = 0; i < _userModels.Count; i++) {
            if (_userModels[i].Uid == guid) {
                _userModels.RemoveAt(i);
                return;
            }
        }

        Debug.LogWarning("Player with the UID " + uid + " was not found or removed.");
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static TeamsController _instance;

    public static TeamsController Instance {
        get { return _instance ?? (_instance = FindObjectOfType<TeamsController>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}

public enum Team
{
    None, 
    Team1, 
    Team2
}

public enum UserModelField
{
    Uid,
    Dead,
    Username,
    ColorIndex,
    CurrentTeam,
    NextTeam,
    Kills,
    KillsIncrement,
    Deaths,
    DeathsIncrement,
    Ping,
    Points,
    PointsIncrement,
    Money,
	MoneyIncrement,
    NetworkIdHash,
    Weapon
}