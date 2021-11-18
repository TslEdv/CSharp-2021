using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class ConfDelete : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public ConfDelete(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Config Config { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Config = await _context.Configs.FirstOrDefaultAsync(m => m.ConfigId== id);
            List<Domain.Game> games = _context.Games.ToList();
            if (games.Any(game => Config.ConfigId == game.ConfigId))
            {
                return RedirectToPage("/CannotDelete");
            }

            if (Config == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Config = await _context.Configs.FindAsync(id);
            
            

            if (Config != null)
            {
                _context.Configs.Remove(Config);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/NewGame");
        }
    }
}