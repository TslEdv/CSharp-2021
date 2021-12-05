using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class ReplayGame : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        public ReplayGame(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public BoardSquareState[,]? Board1 { get; private set; }
        public BoardSquareState[,]? Board2 { get; private set; }
        
        public int RePlayId { get; set; }
        public int PlacementSkip { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int move)
        {
            var config = new GameConfig();
            var replay = new List<ReplayTile>();
            if (id == 0)
            {
                const string localGamePath = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp\SavedGames\game.json";
                const string localLogPath = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp\GameLog\log.json";
                const string? confFile = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp\Configs\standard.json";
                if (!System.IO.File.Exists(localGamePath) || !System.IO.File.Exists(localGamePath) ||
                    !System.IO.File.Exists(confFile))
                {
                    return RedirectToPage("./LoadGame");
                }
                config = JsonSerializer.Deserialize<GameConfig>(await System.IO.File.ReadAllTextAsync(confFile));
                replay = JsonSerializer.Deserialize<List<ReplayTile>>(await System.IO.File.ReadAllTextAsync(localLogPath));
            }
            else
            {
                RePlayId = id;
                var game = await _ctx.Games.FindAsync(id);
                if (game.Status != EGameStatus.Finished)
                {
                    return RedirectToPage("./LoadGame");
                }

                config = game.ConfigId != null
                    ? JsonSerializer.Deserialize<GameConfig>((await _ctx.Configs.FindAsync(game.ConfigId)).ConfigStr)
                    : new GameConfig();
                var gameLog = await _ctx.Replays.FindAsync(game.ReplayId);
                replay = JsonSerializer.Deserialize<List<ReplayTile>>(gameLog.Replays!);
            }
            Board1 = new BoardSquareState[config!.BoardSizeX, config.BoardSizeY];
            Board2 = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            foreach (var play in replay!.Where(play => play.Placing))
            {
                PlacementSkip++;
            }

            if (move > replay!.Count)
            {
                return RedirectToPage("./ReplayGame", new {id = RePlayId, move = replay.Count});
            }
            for (var i = 0; i < move; i++)
            {
                var tile = replay[i];
                switch (tile.Player)
                {
                    case 0:
                        switch (tile.Placing)
                        {
                            case true:
                                Board1[tile.X, tile.Y] = new BoardSquareState
                                {
                                    IsBomb = tile.IsBomb,
                                    IsShip = tile.IsShip
                                };
                                break;
                            case false:
                                Board2[tile.X, tile.Y] = new BoardSquareState
                                {
                                    IsBomb = tile.IsBomb,
                                    IsShip = tile.IsShip
                                };
                                break;
                        }
                        break;
                    case 1:
                        switch (tile.Placing)
                        {
                            case true:
                                Board2[tile.X, tile.Y] = new BoardSquareState
                                {
                                    IsBomb = tile.IsBomb,
                                    IsShip = tile.IsShip
                                };
                                break;
                            case false:
                                Board1[tile.X, tile.Y] = new BoardSquareState
                                {
                                    IsBomb = tile.IsBomb,
                                    IsShip = tile.IsShip
                                };
                                break;
                        }
                        break;
                }
            }

            return Page();
        }
    }
}