using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class DeleteGame : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DeleteGame(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Domain.Game Game { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Game = await _context.Games.FirstOrDefaultAsync(m => m.GameId== id);

            if (Game == null)
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

            Game = await _context.Games.FindAsync(id);

            if (Game != null)
            {
                _context.Games.Remove(Game);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/LoadGame");
        }


    }
}