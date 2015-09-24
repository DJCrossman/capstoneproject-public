using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MatchResultsService
{
    private MatchResultDataServices _dataServices;

    public void CreateMatchResult(Guid uid, int kills, int deaths, int wins, int losses)
    {
        var data = new MatchResultData()
        {
            MID = Guid.NewGuid(),
            UID = uid,
            Kills = kills,
            Deaths = deaths,
            Wins = wins,
            Losses = losses,
            DateOfMatch = DateTime.Now
        };
        _dataServices.CreateMatchResult(data);
    }
}