using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic; 

public class SpawnController : MonoBehaviour 
{
    public Transform[] Team1SpawnPositions;
    public Transform[] Team2SpawnPositions;
	public GameObject CharacterPrefab;

    private TeamsController _teamsController;
    private int _team1SpawnIndex = 0;
    private int _team2SpawnIndex = 0;

	/************************************************************
	 * Event Handlers
	 ************************************************************/
    
	void Awake() 
    {
	    _teamsController = TeamsController.Instance;
	}

    /************************************************************
	 * Control Methods
	 ************************************************************/

	/**
	 * Desc:	Returns where you should spawn a player on a given team.
	 * Params:	None.
	 * Return:	Position where the player should be spawned.
	 * SideFX:	The enumerator moves on to the next position.
	 */
	public Vector3 GetSpawnPosition(Team team) {
	    if (team == Team.Team1) {
	        return Team1SpawnPositions[_team1SpawnIndex++].position;
	    }
        
        if (team == Team.Team2) {
	        return Team2SpawnPositions[_team2SpawnIndex++].position;
	    }

	    return Vector3.zero;
	}

	/**
	 * Desc:	Resets the enumerator so that a spawn location can be retrieved again.
	 * Params:	None.
	 * Return:	None.
	 * SideFX:	Spawn enumerator is reset.
	 */
	private void ResetSpawnIndices() {
	    _team1SpawnIndex = _team2SpawnIndex = 0;
	}

	public void Respawn() {
		if (Network.isServer == false) return;

        networkView.RPC("ReadyPlayersRpc", RPCMode.All);

        // Destroy any existing Player(Clone)s on the scene (for all connected players)
	    var players = FindObjectsOfType<Player>();
        Array.ForEach(players, player => Network.Destroy(player.gameObject));

	    var userModels = _teamsController.GetUserModels();
	    var networkPlayers = _teamsController.GetUserNetworkPlayers();

	    for (int i = 0; i < networkPlayers.Count; i++) {
            // If the player to be spawned is myself (the server) call spawn function directly
            if (networkPlayers[i].guid == Network.player.guid) {
                SpawnSelf(GetSpawnPosition((Team) userModels[i].NextTeam), ServerData.serverInfo.settings.WarmupTime, i);
            }
            else {
                networkView.RPC("SpawnSelf", networkPlayers[i], new object[] {
	                GetSpawnPosition((Team) userModels[i].NextTeam), //where to spawn
	                ServerData.serverInfo.settings.WarmupTime, //how much time until player can move
	                i
	            });
            }
	    }

		ResetSpawnIndices();
	}

	/************************************************************
	 * RPC Handlers
	 ************************************************************/

    [RPC]
    private void ReadyPlayersRpc()
    {
        _teamsController.GetUserModels().ForEach(model =>
        {
            model.CurrentTeam = model.NextTeam;

            if ((Team) model.CurrentTeam != Team.None) {
                model.Dead = false;
            }
        });
    }

	[RPC]
	private void SpawnSelf (Vector3 spawnPosition, int warmupTime, int userIndex)
	{
	    if (spawnPosition == Vector3.zero) return;

		var playerPrefab = ((GameObject) Network.Instantiate(CharacterPrefab, spawnPosition, Quaternion.identity, 0)).transform;
		var player = playerPrefab.GetComponent<Player> ();
	    player.StartActivationTimer(warmupTime);
	    player.PlayerNetwork.SetUserIndex(userIndex);
        _teamsController.UpdateUserModelForAll(ServerData.serverInfo.userData.UID, UserModelField.NetworkIdHash, player.networkView.viewID.GetHashCode());
	}

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static SpawnController _instance;

    public static SpawnController Instance {
        get { return _instance ?? (_instance = FindObjectOfType<SpawnController>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}