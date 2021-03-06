using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApp.Pages
{
    public class Game : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        [BindProperty] public Domain.Game CurrentGame { get; set; } = default!;

        public Game(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private BsBrain? Brain { get; set; } = new(new GameConfig());
        public BoardSquareState[,]? Board { get; private set; }
        public BoardSquareState[,]? FireBoard { get; private set; }

        public int GameId { get; private set; }
        public string Player { get; private set; } = "Player ";
        public List<Ship>? Ships { get; private set; } = new();
        public Ship? CurrentShip;
        public EGameStatus State;
        public int Rotate;
        public bool Start;
        public bool Hit = false;
        public bool Sunk = false;

        public async Task<IActionResult> OnGetAsync(int id, int x, int y, int move, int rotation, int undo, int start)
        {
            GameId = id;
            CurrentGame = await _ctx.Games.FindAsync(id);
            var replay = await _ctx.Replays.FindAsync(CurrentGame.ReplayId);
            Brain!.RestoreLog(replay.Replays!);
            Brain!.RestoreBrainFromJson(CurrentGame.GameState);
            State = Brain.GetGameStatus();
            var countStart = 0;
            var countEnd = 0;
            var savedConf = new GameConfig();
            if (CurrentGame.ConfigId != null)
            {
                savedConf = JsonSerializer.Deserialize<GameConfig>((await _ctx.Configs.FindAsync(CurrentGame.ConfigId)).ConfigStr);
            }
            if (Brain.GameFinish())
            {
                CurrentGame.Status = Brain.GetGameStatus();
                CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                await _ctx.SaveChangesAsync();
            }

            switch (Brain.GetGameStatus())
            {
                case EGameStatus.Finished:
                    Player += Brain.Move() + 1 + " wins";
                    return Page();
                case EGameStatus.Placing:
                    if (undo == 1)
                    {
                        Brain.GoBackAStep();
                        var log = await _ctx.Replays.FindAsync(CurrentGame.ReplayId);
                        log.Replays = Brain.GetLogJson();
                        Brain.StartGame();
                        CurrentGame.Replay!.Replays = Brain.GetLogJson();
                        CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                        CurrentGame.Status = Brain.GetGameStatus();
                        await _ctx.SaveChangesAsync();
                        return RedirectToPage("./Game", new {id = GameId});
                    }
                    Rotate = rotation;
                    switch (move)
                    {
                        case 0:
                            Board = Brain.GetBoard(Brain.Move());
                            FireBoard = Brain.GetFireBoard(Brain.Move());
                            Player += Brain.Move() + 1 + " turn";
                            Ships = Brain.ListShips(Brain.Move());
                            foreach (var ship in Ships.Where(ship => ship.Coordinates.Count == 0))
                            {
                                CurrentShip = ship;
                                break;
                            }

                            break;
                        case 1:
                            Board = Brain.GetBoard(Brain.Move());
                            FireBoard = Brain.GetFireBoard(Brain.Move());
                            Ships = Brain.ListShips(Brain.Move());
                            for (var i = 0; i < Ships.Count; i++)
                            {
                                if (Ships[i].Coordinates.Count != 0) continue;

                                switch (rotation)
                                {
                                    case 0:
                                        if (Brain.PlaceShips(x, x + Ships[i].Length - 1, y, y + Ships[i].Height - 1,
                                            Ships[i], savedConf!.EShipTouchRule) == false)
                                        {
                                            return RedirectToPage("./Game", new {id = GameId});
                                        }
                                        break;
                                    case 1:
                                        if (Brain.PlaceShips(x, x + Ships[i].Height - 1, y, y + Ships[i].Length - 1,
                                            Ships[i], savedConf!.EShipTouchRule) == false)
                                        {
                                            return RedirectToPage("./Game", new {id = GameId});
                                        }
                                        break;
                                }

                                if (i + 1 != Ships.Count)
                                {
                                    CurrentShip = Ships[i + 1];
                                }

                                if (i + 1 == Ships.Count)
                                {
                                    Brain.ChangePlayer();
                                }

                                var log = await _ctx.Replays.FindAsync(CurrentGame.ReplayId);
                                log.Replays = Brain.GetLogJson();
                                Brain.StartGame();
                                CurrentGame.Replay!.Replays = Brain.GetLogJson();
                                CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                                CurrentGame.Status = Brain.GetGameStatus();
                                await _ctx.SaveChangesAsync();
                                return RedirectToPage("./Game", new {id = GameId});
                            }

                            break;
                    }

                    return Page();
                case EGameStatus.Started:
                    switch (move)
                    {
                        case 0:
                            Board = Brain.GetBoard(Brain.Move());
                            FireBoard = Brain.GetFireBoard(Brain.Move());
                            Player += Brain.Move() + 1 + " turn";
                            Ships = Brain.ListShips(Brain.Move());
                            break;
                        case 1:
                            var before = Brain.GetFireBoard(Brain.Move());
                            countStart = Brain.DidSink();
                            var change = Brain.PlayerMove(x, y);
                            countEnd = Brain.DidSink();
                            before[x, y].IsBomb = true;
                            var log = await _ctx.Replays.FindAsync(CurrentGame.ReplayId);
                            log.Replays = Brain.GetLogJson();
                            Board = Brain.GetBoard(Brain.Move());
                            FireBoard = Brain.GetFireBoard(Brain.Move());
                            Player += Brain.Move() + 1 + " turn";
                            Ships = Brain.ListShips(Brain.Move());

                            CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                            await _ctx.SaveChangesAsync();
                            if (Brain.GameFinish())
                            {
                                return RedirectToPage("./Game", new {id = GameId});
                            }
                            if (change)
                            {
                                return RedirectToPage("./MissMove", new {id = GameId});
                            }
                            else
                            {
                                Hit = true;
                                if (countStart < countEnd)
                                {
                                    Sunk = true;
                                }
                            }
                            break;
                            
                    }
                    if (start == 1)
                    {
                        Start = true;
                        Player = "Player " + (Brain.Move() + 1);
                        return Page();
                    }
                    return Page();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CurrentGame = await _ctx.Games.FindAsync(id);
            Brain!.RestoreBrainFromJson(CurrentGame.GameState);

            Brain.GameSurrender();
            CurrentGame.Status = Brain.GetGameStatus();
            CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
            await _ctx.SaveChangesAsync();

            return RedirectToPage("./Game", new {id = id});
        }
    }
}