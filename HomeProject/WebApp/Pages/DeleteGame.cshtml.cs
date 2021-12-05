using System.IO;
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
        public Domain.Replay GameReplay { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id == 0)
            {
                Game = new Domain.Game
                {
                    GameId = 0
                };
            }
            else
            {
                Game = await _context.Games.FirstOrDefaultAsync(m => m.GameId== id);
            }
            

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
            if (id == 0)
            {
                System.IO.File.Delete(@"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" +
                            Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar +
                            "game.json"!);
                System.IO.File.Delete(@"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ 
                            Path.DirectorySeparatorChar + "GameLog" + 
                            Path.DirectorySeparatorChar + "log.json");
            }
            else
            {
                Game = await _context.Games.FindAsync(id);
                GameReplay = await _context.Replays.FindAsync(Game.ReplayId);

                if (Game != null)
                {
                    _context.Replays.Remove(GameReplay);
                    _context.Games.Remove(Game);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("/LoadGame");
        }


    }
}