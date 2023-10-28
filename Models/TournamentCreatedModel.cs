using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;

[Authorize]
public class TournamentCreatedModel : PageModel
{
    private readonly AppDbContext _context;

    public TournamentCreatedModel(AppDbContext context)
    {
        _context = context;
    }

    public Competition LatestCompetition { get; set; }
    public Dictionary<string, float> TeamPoints { get; set; } = new Dictionary<string, float>();
    public int? CompetitionId { get; set; }
    public Dictionary<int, string> MatchOutcomes { get; set; } = new Dictionary<int, string>();
    public Dictionary<(string, string), int> TeamPairToMatchId { get; set; } = new Dictionary<(string, string), int>();

    public List<List<(int, int)>> Schedule { get; set; } = new List<List<(int, int)>>();

    private void GenerateSchedule(bool saveToDatabase = true)
    {
        var teams = LatestCompetition.Teams?.Split(',').ToList();
        if (teams == null)
        {
            throw new InvalidOperationException("Teams not specified in the competition.");
        }

        var teamCount = teams.Count;
         if (teamCount == 4)
    {
        Schedule.Add(new List<(int, int)> { (1, 2), (3, 4) });
        Schedule.Add(new List<(int, int)> { (1, 3), (2, 4) });
        Schedule.Add(new List<(int, int)> { (1, 4), (2, 3) });
    }

    else if (teamCount == 6)
    {
        Schedule.Add(new List<(int, int)> { (1, 2), (3, 6), (4, 5) });
        Schedule.Add(new List<(int, int)> { (1, 3), (2, 5), (4, 6) });
        Schedule.Add(new List<(int, int)> { (1, 4), (2, 6), (3, 5) });
        Schedule.Add(new List<(int, int)> { (1, 5), (2, 3), (4, 6) });
        Schedule.Add(new List<(int, int)> { (1, 6), (2, 4), (3, 5) });
    }
    else if (teamCount == 8)
    {
        Schedule.Add(new List<(int, int)> { (1, 2), (3, 8), (4, 7), (5, 6) });
        Schedule.Add(new List<(int, int)> { (1, 3), (2, 4), (5, 7), (6, 8) });
        Schedule.Add(new List<(int, int)> { (1, 4), (2, 5), (3, 6), (7, 8) });
        Schedule.Add(new List<(int, int)> { (1, 5), (2, 6), (3, 7), (4, 8) });
        Schedule.Add(new List<(int, int)> { (1, 6), (2, 7), (3, 8), (4, 5) });
        Schedule.Add(new List<(int, int)> { (1, 7), (2, 8), (3, 5), (4, 6) });
        Schedule.Add(new List<(int, int)> { (1, 8), (2, 3), (4, 5), (6, 7) });
    }
    else
    {
        throw new InvalidOperationException("The number of teams provided is not supported by the current match schedule generation logic.");
    }

    if (saveToDatabase)
    {
        SaveMatchesToDatabase();
    }
    }

    private void SaveMatchesToDatabase()
    {
        var teams = LatestCompetition.Teams?.Split(',').ToList();

        foreach (var round in Schedule)
        {
            foreach (var match in round)
            {
                var team1 = teams[match.Item1 - 1];
                var team2 = teams[match.Item2 - 1];
                
                var matchRecord = new Match
                {
                    CompetitionId = LatestCompetition.Id,
                    Team1 = team1,
                    Team2 = team2
                };

                _context.Matches.Add(matchRecord);
            }
        }

        _context.SaveChanges();
    }
    
    public void OnGet(int? competitionId = null)
{
    string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    CompetitionId = competitionId;

    if (CompetitionId.HasValue)
    {
        LatestCompetition = _context.Competitions
            .FirstOrDefault(c => c.UserId == currentUserId && c.Id == CompetitionId);
    }
    else
    {
        LatestCompetition = _context.Competitions
            .Where(c => c.UserId == currentUserId)
            .OrderByDescending(c => c.Id)
            .FirstOrDefault();
    }

    if (LatestCompetition != null)
    {
        // Always generate the schedule for viewing purposes
        GenerateSchedule(false);  // passing false to indicate that we don't want to save it yet

        // Check if matches for this competition already exist in the database
        var existingMatches = _context.Matches
            .Where(m => m.CompetitionId == LatestCompetition.Id)
            .ToList();
        
        foreach (var match in existingMatches)
        {
            TeamPairToMatchId[(match.Team1, match.Team2)] = match.MatchId;
            MatchOutcomes[match.MatchId] = match.Outcome;
        }


        if (!existingMatches.Any())
        {
            // No matches exist for this competition, save them to the database
            SaveMatchesToDatabase();
        }
    }
    TeamPoints = CalculateTeamPoints();
}

public IActionResult OnPostSaveOutcomes(Dictionary<string, string> outcomes, int? CompetitionId)
{
    string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

    // Fetch the LatestCompetition here
   if(!CompetitionId.HasValue) 
{
    ModelState.AddModelError(string.Empty, "CompetitionId is missing.");
    return Page();
}

LatestCompetition = _context.Competitions
        .FirstOrDefault(c => c.UserId == currentUserId && c.Id == CompetitionId);

    if (LatestCompetition == null)
    {
        ModelState.AddModelError(string.Empty, "Competition not found.");
        return Page();
    }

    foreach (var outcome in outcomes)
    {
        // Parsing the team indices from the name attribute of the select element
        var teamIndices = outcome.Key.Split('_').Select(int.Parse).ToArray();
        int team1Index = teamIndices[0];
        int team2Index = teamIndices[1];

        // Retrieve the corresponding match from the database
    
        var team1 = LatestCompetition.Teams.Split(',')[team1Index];
        var team2 = LatestCompetition.Teams.Split(',')[team2Index];

        var match = _context.Matches
            .FirstOrDefault(m => m.CompetitionId == LatestCompetition.Id && m.Team1 == team1 && m.Team2 == team2);

        if (match != null)
        {
            // Set the outcome of the match
            if (outcome.Value == "Draw")
            {
                match.Outcome = "Draw";
            }
            else if (outcome.Value == "Team1Win")
            {
                match.Outcome = "WinTeam1";
            }
            else if (outcome.Value == "Team2Win")
            {
                match.Outcome = "WinTeam2";
            }

            // You can change the status of the match to "Completed" or any other status indicating that the outcome is recorded
            match.Status = "Completed";
        }
    }

    // Save changes to the database
    _context.SaveChanges();

    // Redirect or show a message confirming that the outcomes were saved
    TempData["Message"] = "Match outcomes saved successfully!";
    return RedirectToPage(new { competitionId = CompetitionId });
}
public string GetWinnerByTeams(string team1, string team2)
{
    if (TeamPairToMatchId.ContainsKey((team1, team2)) && MatchOutcomes.ContainsKey(TeamPairToMatchId[(team1, team2)]))
    {
        var outcome = MatchOutcomes[TeamPairToMatchId[(team1, team2)]];
        return outcome switch
        {
            "WinTeam1" => team1,
            "WinTeam2" => team2,
            _ => "Draw"
        };
    }
    return null;
}
public Dictionary<string, float> CalculateTeamPoints()
{
    var teamsList = LatestCompetition.Teams.Split(',').ToList();
    Dictionary<string, float> teamPoints = teamsList.ToDictionary(team => team, team => 0f);

    var matchesForThisCompetition = _context.Matches.Where(m => m.CompetitionId == LatestCompetition.Id).ToList();

    foreach (var match in matchesForThisCompetition)
    {
        switch (match.Outcome)
        {
            case "WinTeam1":
                teamPoints[match.Team1] += LatestCompetition.VictoryPoints;
                teamPoints[match.Team2] += LatestCompetition.DefeatPoints;
                break;
            case "WinTeam2":
                teamPoints[match.Team2] += LatestCompetition.VictoryPoints;
                teamPoints[match.Team1] += LatestCompetition.DefeatPoints;
                break;
            case "Draw":
                teamPoints[match.Team1] += LatestCompetition.DrawPoints;
                teamPoints[match.Team2] += LatestCompetition.DrawPoints;
                break;
        }
    }

    return teamPoints;
}
}