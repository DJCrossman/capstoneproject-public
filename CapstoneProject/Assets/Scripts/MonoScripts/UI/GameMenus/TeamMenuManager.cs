using System.Linq;
using System.Net.Mime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeamMenuManager : InGameMenuController
{

    public Text Team1Text;
    public Text Team2Text;

    private TeamsController _teamsController;

    /**************************************************
     * Event Handlers
     **************************************************/

    protected override void Awake()
    {
        base.Awake();	//must be included in every child of MenuController (due to Unity bug)
        ToggleKey = KeyCode.T;
        ToggleEnabled = true;
        _teamsController = global::TeamsController.Instance;
    }

    /**************************************************
     * Control Methods
     **************************************************/

	public void JoinTeam1() {
		_teamsController.UpdateUserModelForAll(ServerData.serverInfo.userData.UID, UserModelField.NextTeam, Team.Team1);
        _teamsController.UpdateUserModelForAll(ServerData.serverInfo.userData.UID, UserModelField.ColorIndex, _teamsController.GetNumberOfNextTeamMembers(Team.Team1));
	    Hide();
	}

    public void JoinTeam2()
    {
        _teamsController.UpdateUserModelForAll(ServerData.serverInfo.userData.UID, UserModelField.NextTeam, Team.Team2);
        _teamsController.UpdateUserModelForAll(ServerData.serverInfo.userData.UID, UserModelField.ColorIndex, _teamsController.GetNumberOfNextTeamMembers(Team.Team2));
        Hide();
    }

     /**************************************************
     * Singleton Declaration
     **************************************************/

    private static TeamMenuManager _instance;

    public static TeamMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<TeamMenuManager>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
