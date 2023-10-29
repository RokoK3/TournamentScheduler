using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Authorize]
public class MyCompetitionsModel : PageModel
{
    private readonly AppDbContext _context;

    public MyCompetitionsModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Competition> UserCompetitions { get; set; }

    public void OnGet()
    {
        string currentUserId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        
        UserCompetitions = _context.Competitions
            .Where(c => c.UserId == currentUserId)
            .ToList();
    }
}
