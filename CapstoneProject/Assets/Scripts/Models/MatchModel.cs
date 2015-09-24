using UnityEngine;
using System.Collections;

public class MatchModel
{

    private int _team1Wins = 0;
    private int _team2Wins = 0;
    private int _draws = 0;

    /************************************************************
   * Control Methods
   ************************************************************/

    public void WinningTeam(Team team)
    {
        if (team == Team.Team1) {
            _team1Wins++;
        } else if (team == Team.Team2) {
            _team2Wins++;
        } else {
            _draws++;
        }
    }

    public void UpdateMatchModel(int team1Wins, int team2Wins, int draws)
    {
        _team1Wins += team1Wins;
        _team2Wins += team2Wins;
        _draws += draws;
    }

    /************************************************************
     * Getters and Setters
     ************************************************************/

    public int GetTeamWins(Team team)
    {
        if (team == Team.Team1) {
            return _team1Wins;
        } else if (team == Team.Team2) {
            return _team2Wins;
        }

        return 0;
    }

    public int GetDraws()
    {
        return _draws;
    }
}
