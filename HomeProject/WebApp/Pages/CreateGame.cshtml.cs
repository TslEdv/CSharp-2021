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
        public static BsBrain Brain { get; set; } = new BsBrain(new GameConfig());
        
        public Config Config { get; private set; } = new Config();

        public int GameId;

        public void OnGet(int id)
        {
            if (id != 0)
            {
                Config = _ctx.Configs.Find(id);
                var savedconf = JsonSerializer.Deserialize<GameConfig>(_ctx.Configs.Find(id).ConfigStr);
                Brain = new BsBrain(savedconf);
            }
            else
            {
                Config.ConfigStr = new GameConfig().ToString();
            }
            var jsonstr = Brain.GetBrainJson(Brain.Move());
            var savegamedb = new Domain.Game()
            {
                GameState = jsonstr
            };
            _ctx.Games.Add(savegamedb);
            _ctx.SaveChanges();
            GameId = savegamedb.GameId;
        }
    }
}