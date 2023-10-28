using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Authorize]
public class TournamentCreatedModel : PageModel
{
    private readonly AppDbContext _context;

    public TournamentCreatedModel(AppDbContext context)
    {
        _context = context;
    }

    public Competition LatestCompetition { get; set; }

    public int? CompetitionId { get; set; }

    public List<List<(int, int)>> Schedule { get; set; } = new List<List<(int, int)>>();
    private void GenerateSchedule()
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
}

    public void OnGet(int? competitionId = null)
    {
        string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        CompetitionId = competitionId;

        if (CompetitionId.HasValue)
        {
            // Fetch the competition with the given ID for the current user
            LatestCompetition = _context.Competitions
                .FirstOrDefault(c => c.UserId == currentUserId && c.Id == CompetitionId);
        }
        else
        {
            // Fetch the latest competition created by the current user
            LatestCompetition = _context.Competitions
                .Where(c => c.UserId == currentUserId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        // Assuming that the Teams property in the Competition model is a comma-separated string of team names.
        if (LatestCompetition != null)
        {
            // Generate the match schedule
            GenerateSchedule();
        }
    }
    }
    