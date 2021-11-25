using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApp.Pages
{
    public class CreateGame : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        public CreateGame(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Config Config { get; private set; } = new Config();

        public int GameId;

        public bool IsRandom { get; private set; } = true;

        public async Task<IActionResult> OnGet(int id)
        {
            var brain = new BsBrain(new GameConfig());
            var saveGameDb = new Domain.Game();
            if (id != 0)
            {
                Config = await _ctx.Configs.FindAsync(id);
                var savedConf = JsonSerializer.Deserialize<GameConfig>((await _ctx.Configs.FindAsync(id)).ConfigStr);
                if (savedConf!.TestConf() == false)
                {
                    return RedirectToPage("./CannotCreate");
                }
                if (savedConf is {IsRandom: false})
                {
                    IsRandom = false;
                }
                brain = new BsBrain(savedConf);
                saveGameDb.ConfigId = id;
                saveGameDb.Config = Config;
            }
            else
            {
                Config.ConfigStr = new GameConfig().ToString();
            }
            var jsonStr = brain.GetBrainJson(brain.Move());

            saveGameDb.GameState = jsonStr;
            saveGameDb.Status = brain.GetGameStatus();
            saveGameDb.Replay = new Replay
            {
                Replays = brain.GetLogJson(),
            };
            _ctx.Games.Add(saveGameDb);
            await _ctx.SaveChangesAsync();
            GameId = saveGameDb.GameId;
            return Page();
        }
    }
}