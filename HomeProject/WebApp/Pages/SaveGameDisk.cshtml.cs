using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class SaveGameDisk : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public SaveGameDisk(DAL.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public Domain.Game SaveGame { get; set; } = default!;
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SaveGame = await _context.Games.FindAsync(id);

            if (SaveGame == null)
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

            SaveGame = await _context.Games.FindAsync(id);


            if (SaveGame == null) return RedirectToPage("/LoadGame");
            var datafile =  @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar + "game.json";
            var logfile =  @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ Path.DirectorySeparatorChar + "GameLog" + Path.DirectorySeparatorChar + "log.json";
            var conffile =  @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar + "standard.json";
            await System.IO.File.WriteAllTextAsync(datafile, SaveGame.GameState);
            var log =  await _context.Replays.FindAsync(SaveGame.ReplayId);
            var config = await _context.Configs.FindAsync(SaveGame.ConfigId);
            var savedConf = JsonSerializer.Deserialize<GameConfig>(config.ConfigStr);
            await System.IO.File.WriteAllTextAsync(conffile, savedConf!.ToString());
            await System.IO.File.WriteAllTextAsync(logfile, log.Replays);

            return RedirectToPage("/LoadGame");
        }
    }
}