
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
[Authorize]
public class UserProfileModel : PageModel
    {
        public string? Username { get; private set; }

        public IActionResult OnGet()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/login");
            }

            var nicknameClaim = User.Claims.FirstOrDefault(c => c.Type == "nickname");

            if (nicknameClaim != null)
            {
                Username = nicknameClaim.Value;
            }
            else
            {
                Username = "User";
            }
            return Page();
        }
    }