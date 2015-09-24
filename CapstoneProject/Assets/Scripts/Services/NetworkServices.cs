using System;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class NetworkServices
{

    private HostData[] _hostData;
    private bool _refreshing = false;
    private UserServices _userService;
    private string _localUserPath;

    public NetworkServices()
    {
        MasterServer.ipAddress = ServerData.MasterServerIP;
        _localUserPath = Application.dataPath + "/userProfile.dat";
        // Sets the Match Settings
        ServerData.serverInfo.settings = MatchSettings.Instance;
        _userService = new UserServices(_localUserPath);
        RefreshHostList();
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public void RefreshHostList()
    {
        MasterServer.ClearHostList();
        MasterServer.RequestHostList(Debug.isDebugBuild ? ServerData.DevelopmentServerID : ServerData.GameServerID);
        _refreshing = true;
    }

    public void StartServer()
    {
        // Loads User Profile
        ServerData.serverInfo.userData = _userService.GetUserData();

        _refreshing = false;
        ServerData.serverInfo.connections = 8;
        ServerData.serverInfo.port = 25001;
        ServerData.serverInfo.password = "LolCat5!";
        ServerData.serverInfo.name = "Test Server";
        ServerData.serverInfo.description = "This is a test server";
        ServerData.isHost = true;

        Application.LoadLevel("Apollo");
    }

    public void ConnectToServer()
    {
        var isMatchFound = false;

        if (_hostData != null)
        {
            foreach (var current in _hostData)
            {
                var players = current.connectedPlayers;
                if (players <= 8)
                {
                    isMatchFound = true;
                    ServerData.hostData = current;
                    ServerData.password = "LolCat5!";
                    ServerData.isHost = false;
                    Application.LoadLevel("Apollo");
                    break;
                }
            }

            if (isMatchFound == false)
            {
                StartServer();
            }
        }
        else
        {
            Debug.Log("No servers to join");
        }
    }

    public void JoinServer()
    {
        // Loads User Profile
        ServerData.serverInfo.userData = _userService.GetUserData();
        ConnectToServer();
    }

    public bool isUserLoggedIn()
    {
        return _userService.GetUserData() != null;
    }

    public void UpdateHostData()
    {
        if (_refreshing)
        {
            if (MasterServer.PollHostList().Length > 0)
            {
                _hostData = MasterServer.PollHostList();
                _refreshing = false;
            }
        }
    }

    public void LogIn(string email, string password, bool remMyPassword)
    {
        // Prepend the salt to the password and hash it with a standard cryptographic hash function such as SHA256.
        _userService.VerifyLoginInformation(email, CreateSalt() + _userService.Delimiter + CreateHash(password), remMyPassword);
    }

    public void CreateAccount(string userName, string email, string password)
    {
        _userService.CreateAccount(userName, email, CreateSalt() + _userService.Delimiter + CreateHash(password));
        Debug.Log("Created User Successfully");
    }

    public void LogOut()
    {
        _userService.LogOut();
        Debug.Log("User Logged out Successfully");
        ServerData.serverInfo.userData = null;
    }

    private string CreateSalt()
    {
        // Generate a long random salt using a CSPRNG
        RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
        byte[] salt = new byte[_userService.SaltBytes];
        csprng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }

    private string CreateHash(string password)
    {
        var hash = SHA256.Create();
        byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(data);
    }

    /**************************************************
	 * Getters and Setters
	 **************************************************/

    public void SetUserName(string name)
    {
        _userService.SetUserName(name);
        ServerData.serverInfo.userData = _userService.GetUserData();
    }

    // TODO: Verify if function is needed
    //void OnMasterServerEvent(MasterServerEvent mse)
    //{
    //    switch (mse)
    //    {
    //        case MasterServerEvent.RegistrationSucceeded:
    //            Debug.Log("Server registered");
    //            break;
    //    }
    //}

    public string GetUserName()
    {
        return _userService.GetUserName();
    }

    public bool isHostDataEmpty()
    {
        if (_hostData != null)
            return _hostData.Length == 0;
        return true;
    }
}
