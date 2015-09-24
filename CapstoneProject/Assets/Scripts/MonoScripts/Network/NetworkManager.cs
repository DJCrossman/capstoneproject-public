using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public List<Guid> BannedUsers;

	private TeamsController _teamsController;
    private TeamMenuManager _teamMenuManager;
    private bool _isConnected = false;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

	private void Awake ()
	{
        MasterServer.ipAddress = ServerData.MasterServerIP;
	    BannedUsers = new List<Guid>();

        // Determine if player is joining(creating) game and join(create) server
        if (ServerData.isHost == true) {
            StartServer(ServerData.serverInfo);
        } else {
            JoinServer(ServerData.hostData, ServerData.password);
        }
	}

    private void Start()
    {
        _teamsController = global::TeamsController.Instance;
        _teamMenuManager = global::TeamMenuManager.Instance;
    }

    void OnConnectedToServer() 
    {
        _isConnected = true;
    }

    private void Update() 
    {
        if (_isConnected == true) {
            _isConnected = false;
            var userData = ServerData.serverInfo.userData;
            
            if (Network.isServer == true) {
                _teamsController.AddNetworkPlayer(Network.player, userData.UserName, userData.UID);
                _teamMenuManager.Show();
            } else {
                networkView.RPC("UserConnected", RPCMode.Server, new object[] {
                    userData.UserName, 
                    userData.UID.ToString()
                });
            }
        }
    }

    void OnServerInitialized() 
    {
        _isConnected = true;
    }

    void OnPlayerDisconnected(NetworkPlayer networkPlayer) {
        _teamsController.RemoveNetworkPlayer(networkPlayer);
        Network.RemoveRPCs(networkPlayer);
        Network.DestroyPlayerObjects(networkPlayer);
    }

    void OnPlayerConnected(NetworkPlayer player) {
        var bannedUsers = new MemoryStream();
        var binaryFormatter = new BinaryFormatter();

        // Get serialized list of team 1 data
        binaryFormatter.Serialize(bannedUsers, BannedUsers);
        string bannedUsersSerialization = Convert.ToBase64String(bannedUsers.GetBuffer());

        networkView.RPC("AmIBanned", player, bannedUsersSerialization);
    }

    void OnDisconnectedFromServer(NetworkDisconnection networkDisconnection)
    {
        if (networkDisconnection == NetworkDisconnection.LostConnection) {
            Application.LoadLevel("MainMenu");
            //TODO popup message
            Debug.Log("You have been kicked from the server.");
        }
        else if (networkDisconnection == NetworkDisconnection.Disconnected) {
            Application.LoadLevel("MainMenu");
            //TODO popup message
            Debug.Log("You have lost your connection.");
        }
    }

    /************************************************************
	 * Control Methods
	 ************************************************************/

	/**
	 * Desc:	Creates a server with the information inside ServerInfo.
	 * Params:	ServerInfo.
	 * Return:	None.
	 * SideFX:	Unity attempts to create a server and register it. 
	 * 			OnServerInitialized() called on success.
	 */
	private void StartServer(ServerInfo serverInfo) {
		Network.incomingPassword = serverInfo.password;
		Network.InitializeServer (serverInfo.connections, serverInfo.port, !Network.HavePublicAddress());
		MasterServer.RegisterHost (ServerData.GameServerID, serverInfo.name, serverInfo.description);
	}

	/**
	 * Desc:	Joins a server with the host data and password.
	 * Params:	Host data and password.
	 * Return:	None.
	 * SideFX:	Unity attempts to join the server with host data and password passed.
	 * 			OnConnectedToServer() called on success.
	 */
	private void JoinServer(HostData hostData, string password) {
		Network.Connect(hostData.ip, hostData.port, password);
	}

    public void Disconnect()
    {
        if (Network.isServer) {
            MasterServer.UnregisterHost();
        }
    }

    /**************************************************
     * RPC Handlers
     **************************************************/

    [RPC]
    private void AmIBanned(string bannedUserSerialization, NetworkMessageInfo networkMessageInfo)
    {
        var bannedUsers = new MemoryStream(Convert.FromBase64String(bannedUserSerialization));
        var binaryFormatter = new BinaryFormatter();

        // Get deserialized list of banned players and disconnect from server if you are one of them
        List<Guid> serversBannedUsers = (List<Guid>)binaryFormatter.Deserialize(bannedUsers);
        foreach (Guid uid in serversBannedUsers)
        {
            if (uid == ServerData.serverInfo.userData.UID) {
                Network.Disconnect();
                Application.LoadLevel("MainMenu");
            }
        }
    }

    [RPC]
    private void UserConnected(string userName, string uid, NetworkMessageInfo networkMessageInfo)
    {
        _teamsController.AddNetworkPlayer(networkMessageInfo.sender, userName, new Guid(uid));
        _teamsController.SendConnectedUsers(networkMessageInfo.sender);
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<NetworkManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }

}
