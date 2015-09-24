using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MatchResultData
{

    public Guid MID { get; set; }
    public Guid UID { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public DateTime DateOfMatch { get; set; }

    public MatchResultData()
    {
        this.MID = Guid.NewGuid();
        this.UID = Guid.Empty;
        this.Kills = 0;
        this.Deaths = 0;
        this.Wins = 0;
        this.Losses = 0;
        this.DateOfMatch = DateTime.Now;
    }
}