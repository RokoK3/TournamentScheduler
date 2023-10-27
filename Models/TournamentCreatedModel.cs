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

    public void OnGet()
    {
        string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        // Fetch the latest competition created by the current user
        LatestCompetition = _context.Competitions
            .Where(c => c.UserId == currentUserId)
            .OrderByDescending(c => c.Id)
            .FirstOrDefault();
    }
}
