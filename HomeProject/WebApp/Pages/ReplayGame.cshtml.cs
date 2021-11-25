using System.Collections.Generic;
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
            RePlayId = id;
            var game = await _ctx.Games.FindAsync(id);
            if (game.Status != EGameStatus.Finished)
            {
                return RedirectToPage("./LoadGame");
            }

            var config = game.ConfigId != null
                ? JsonSerializer.Deserialize<GameConfig>((await _ctx.Configs.FindAsync(game.ConfigId)).ConfigStr)
                : new GameConfig();
            Board1 = new BoardSquareState[config!.BoardSizeX, config.BoardSizeY];
            Board2 = new BoardSquareState[config.BoardSizeX, config.BoardSizeY];
            var gameLog = await _ctx.Replays.FindAsync(game.ReplayId);
            var replay = JsonSerializer.Deserialize<List<ReplayTile>>(gameLog.Replays!);
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