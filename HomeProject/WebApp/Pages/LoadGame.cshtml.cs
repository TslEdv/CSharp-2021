using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class LoadGame : PageModel
    {
        [BindProperty] public int Value { get; set; }
        
        private readonly ApplicationDbContext _ctx;

        public LoadGame(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public List<Domain.Game> Games { get; set; } = default!;
        
        public IActionResult OnGet()
        {
            Games = _ctx.Games.ToList();
            return Page();
        }
        
    }
}