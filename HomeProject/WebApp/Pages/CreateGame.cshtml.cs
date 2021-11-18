using BattleShipBrain;
using DAL;
using Domain;
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

        private BsBrain Brain { get; set; } = new BsBrain(new GameConfig());
        
        public Config Config { get; private set; } = new Config();

        public int GameId;

        public bool IsRandom { get; private set; } = true;

        public void OnGet(int id)
        {
            var saveGameDb = new Domain.Game();
            if (id != 0)
            {
                Config = _ctx.Configs.Find(id);
                var savedConf = JsonSerializer.Deserialize<GameConfig>(_ctx.Configs.Find(id).ConfigStr);
                if (savedConf is {IsRandom: false})
                {
                    IsRandom = false;
                }
                Brain = new BsBrain(savedConf);
                saveGameDb.ConfigId = id;
                saveGameDb.Config = Config;
            }
            else
            {
                Config.ConfigStr = new GameConfig().ToString();
            }
            var jsonStr = Brain.GetBrainJson(Brain.Move());

            saveGameDb.GameState = jsonStr;
            saveGameDb.Status = Brain.GetGameStatus();
            _ctx.Games.Add(saveGameDb);
            _ctx.SaveChanges();
            GameId = saveGameDb.GameId;
        }
    }
}