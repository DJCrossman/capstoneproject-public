using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameSummaryMenuManager : MenuController
{
    public PlayerStatsUIModel[] Team1PlayerStats, Team2PlayerStats;
    public Text Team1Wins, Team2Wins;

    private MatchController _matchController;
    private TeamsController _teamsController;

    /**************************************************
     * Event Handlers
     **************************************************/

    // Use this for initialization
    protected override void Awake() 
    {
        _matchController = global::MatchController.Instance;
        _teamsController = global::TeamsController.Instance;

        base.Awake();
        ToggleKey = KeyCode.Tab;
        ToggleEnabled = true;
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public override void Show() 
    {
        int team1Index = 0, team2Index = 0;

        List<UserModel> userModels = _teamsController.GetUserModels();
        ClearPlayerUi(Team1PlayerStats);
        ClearPlayerUi(Team2PlayerStats);

        userModels.ForEach(delegate(UserModel userModel) {
            Team team = (Team) userModel.CurrentTeam;

            if (team == Team.Team1) {
                UpdatePlayerUi(userModel, Team1PlayerStats[team1Index++]);
            } else if (team == Team.Team2) {
                UpdatePlayerUi(userModel, Team2PlayerStats[team2Index++]);
            }
        });


        MatchModel matchModel = _matchController.GetMatchModel();

        if (matchModel != null) {
            Team1Wins.text = matchModel.GetTeamWins(Team.Team1).ToString();
            Team2Wins.text = matchModel.GetTeamWins(Team.Team2).ToString();
        } else {
            Team1Wins.text = "0";
            Team2Wins.text = "0";
        }

        base.Show();
    }

    /**************************************************
     * Helper Methods
     **************************************************/

    private void UpdatePlayerUi(UserModel userModel, PlayerStatsUIModel uiModel) 
    {
        uiModel.PingText.text = userModel.Ping.ToString();
        uiModel.IsDeadText.text = userModel.Dead == true ? "X" : "";
        uiModel.NameText.text = userModel.UserName;
        uiModel.KillText.text = userModel.Kills.ToString();
        uiModel.DeathsText.text = userModel.Deaths.ToString();
        uiModel.MoneyText.text = userModel.Money.ToString();
        uiModel.PointsText.text = userModel.Points.ToString();
    }

    private void ClearPlayerUi(PlayerStatsUIModel[] uiModels) 
    {
        Array.ForEach(uiModels, uiModel => {
            uiModel.PingText.text = "";
            uiModel.IsDeadText.text = "";
            uiModel.NameText.text = "";
            uiModel.KillText.text = "";
            uiModel.DeathsText.text = "";
            uiModel.MoneyText.text = "";
            uiModel.PointsText.text = "";
        });
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static GameSummaryMenuManager _instance;

    public static GameSummaryMenuManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<GameSummaryMenuManager>()); }
    }
    public static void Reset()
    {
        _instance = null;
    }
}
