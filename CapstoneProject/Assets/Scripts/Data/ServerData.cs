using UnityEngine;
using System.Collections;

public static class ServerData {

    public const string GameServerID = "IRleoa2pMnv2mVyuWCVm8UmxKZ9Cj1p8XrcB2lLHJZ84HpLXL38LGnb043";
    public const string DevelopmentServerID = "IRleoa2pMnv2mVyuWCVm8UmxKZ9Cj1p8XrcB2lLHJZ84HpLXL38LGndevb043";
    public const string MasterServerIP = "23.96.178.94";

    // info needed to connect to server
    public static HostData hostData;
    public static string password;

    // info needed to create server
    public static ServerInfo serverInfo;

	public static bool isHost;
}

public struct ServerInfo
{
	public UserData userData;
	public MatchSettings settings;
    public int connections;
    public int port;
    public string name;
    public string description;
    public string password;
}
