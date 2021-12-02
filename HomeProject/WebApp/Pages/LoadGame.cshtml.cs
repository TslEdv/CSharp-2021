using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
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

        public string LocalGamePath { get; set; } = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" +
                                                    Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar +
                                                    "game.json";
        public string LocalLogPath { get; set; } = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp"+ 
                                                   Path.DirectorySeparatorChar + "GameLog" + 
                                                   Path.DirectorySeparatorChar + "log.json";

        private string LocalConfPath { get; set; } = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" + Path.DirectorySeparatorChar +
                                                     "Configs" + Path.DirectorySeparatorChar + "localgameconf.json";
        public EGameStatus State { get; set; }
        
        public async Task<IActionResult> OnGet()
        {
            var brain = new BsBrain(new GameConfig());
            if (System.IO.File.Exists(LocalGamePath) && System.IO.File.Exists(LocalLogPath) && System.IO.File.Exists(LocalConfPath))
            {
                brain.RestoreBrainFromJson(await System.IO.File.ReadAllTextAsync(LocalGamePath));
                brain.RestoreLog(await System.IO.File.ReadAllTextAsync(LocalLogPath));
            }
            State = brain.GetGameStatus();
            Games = _ctx.Games.ToList();
            return Page();
        }
        
    }
}