using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class CreateCompetitionModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateCompetitionModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Teams { get; set; }

    [BindProperty, Range(0, float.MaxValue)]
    public float VictoryPoints { get; set; }

    [BindProperty, Range(0, float.MaxValue)]
    public float DrawPoints { get; set; }

    [BindProperty, Range(0, float.MaxValue)]
    public float DefeatPoints { get; set; }

    public IActionResult OnPost()
{
    if (!ModelState.IsValid) 
    {
        return Page(); 
    }
    
    Teams = Teams.Replace(";", ",").Replace("\n", ",");
    
    var teamList = Teams.Split(',').Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

    // Check if teams are less than 4 or more than 8.
    if (teamList.Count < 4 || teamList.Count > 8)
    {
        ModelState.AddModelError("", "The number of teams should be between 4 and 8.");
        return Page();
    }

    // If 5 or 7 teams, add one "BYE" to make it even.
    if (teamList.Count == 5 || teamList.Count == 7)
    {
        teamList.Add("BYE");
    }
    
    Teams = string.Join(",", teamList);

    try
    {
        string userId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        
        var competition = new Competition 
        {
            UserId = userId,
            Teams = Teams, 
            VictoryPoints = VictoryPoints,
            DrawPoints = DrawPoints,
            DefeatPoints = DefeatPoints
        };

        _context.Competitions.Add(competition);
        _context.SaveChanges();

        return RedirectToPage("/TournamentCreated"); 
    }
    catch(Exception ex)
    {
        ModelState.AddModelError("", "There was an error saving the data. Please try again.");
        return Page(); 
    }
}


}   