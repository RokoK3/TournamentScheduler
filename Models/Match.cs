public class Match
{
    public int MatchId { get; set; }
    public int CompetitionId { get; set; }
    public Competition Competition { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public string Status { get; set; } = "Pending";
    public string Outcome { get; set; } // WinTeam1, WinTeam2, Draw
}