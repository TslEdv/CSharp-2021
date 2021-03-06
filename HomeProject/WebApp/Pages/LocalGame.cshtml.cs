using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class LocalGame : PageModel
    {
        private BsBrain? Brain { get; set; } = new(new GameConfig());
        public BoardSquareState[,]? Board { get; private set; }
        public BoardSquareState[,]? FireBoard { get; private set; }
        
        public string Player { get; private set; } = "Player ";
        public List<Ship>? Ships { get; private set; } = new();
        public Ship? CurrentShip;
        public EGameStatus State;
        public int Rotate;
        public bool Start;
        public bool Hit = false;
        public bool Sunk = false;

        public async Task<IActionResult> OnGetAsync(int x, int y, int move, int rotation, int undo, int start)
        {
            string localGamePath = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" +
                                   Path.DirectorySeparatorChar + "SavedGames" + Path.DirectorySeparatorChar +
                                   "game.json";
            string localLogPath = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" +
                                  Path.DirectorySeparatorChar + "GameLog" +
                                  Path.DirectorySeparatorChar + "log.json";
            var confFile = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" + Path.DirectorySeparatorChar +
                           "Configs" + Path.DirectorySeparatorChar + "standard.json";
            if (!System.IO.File.Exists(localGamePath) || !System.IO.File.Exists(localGamePath) ||
                !System.IO.File.Exists(confFile))
            {
                return RedirectToPage("./LoadGame");
            }

            Brain!.RestoreBrainFromJson(await System.IO.File.ReadAllTextAsync(localGamePath));
            Brain.RestoreLog(await System.IO.File.ReadAllTextAsync(localLogPath));
            var savedConf = JsonSerializer.Deserialize<GameConfig>(await System.IO.File.ReadAllTextAsync(confFile));
            State = Brain.GetGameStatus();
            switch (Brain.GetGameStatus())
            {
                case EGameStatus.Finished:
                    Player += Brain.Move() + 1 + " wins";
                    return Page();
                case EGameStatus.Placing:
                    if (undo == 1)
                    {
                        Brain.GoBackAStep();
                        await System.IO.File.WriteAllTextAsync(localGamePath,
                            Brain.GetBrainJson(Brain.Move()));
                        await System.IO.File.WriteAllTextAsync(confFile, savedConf!.ToString());
                        await System.IO.File.WriteAllTextAsync(localLogPath, Brain.GetLogJson());
                        return RedirectToPage("/LocalGame");
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
                                            return RedirectToPage("./LocalGame", new { });
                                        }

                                        break;
                                    case 1:
                                        if (Brain.PlaceShips(x, x + Ships[i].Height - 1, y, y + Ships[i].Length - 1,
                                            Ships[i], savedConf!.EShipTouchRule) == false)
                                        {
                                            return RedirectToPage("./LocalGame", new{ });
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

                                Brain.StartGame();
                                await System.IO.File.WriteAllTextAsync(localGamePath,
                                    Brain.GetBrainJson(Brain.Move()));
                                await System.IO.File.WriteAllTextAsync(confFile, savedConf!.ToString());
                                await System.IO.File.WriteAllTextAsync(localLogPath, Brain.GetLogJson());
                                return RedirectToPage("./LocalGame", new { });
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
                            var countStart = Brain.DidSink();
                            var change = Brain.PlayerMove(x, y);
                            var countEnd = Brain.DidSink();
                            Board = Brain.GetBoard(Brain.Move());
                            FireBoard = Brain.GetFireBoard(Brain.Move());
                            Player += Brain.Move() + 1 + " turn";
                            Ships = Brain.ListShips(Brain.Move());

                            await System.IO.File.WriteAllTextAsync(localGamePath, Brain.GetBrainJson(Brain.Move()));
                            await System.IO.File.WriteAllTextAsync(confFile, savedConf!.ToString());
                            await System.IO.File.WriteAllTextAsync(localLogPath, Brain.GetLogJson());
                            if (change)
                            {
                                return RedirectToPage("./MissMove", new {id = 0});
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

        public async Task<IActionResult> OnPostAsync()
        {
            const string localGamePath = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp\SavedGames\game.json";
            string localLogPath = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" +
                                  Path.DirectorySeparatorChar + "GameLog" +
                                  Path.DirectorySeparatorChar + "log.json";
            var confFile = @"C:\Users\User\Desktop\C#\HomeProject\BattleShipsConsoleApp" + Path.DirectorySeparatorChar +
                           "Configs" + Path.DirectorySeparatorChar + "standard.json";
            if (!System.IO.File.Exists(localGamePath) || !System.IO.File.Exists(localLogPath) ||
                !System.IO.File.Exists(confFile))
            {
                return RedirectToPage("./LoadGame");
            }
            
            Brain!.RestoreBrainFromJson(await System.IO.File.ReadAllTextAsync(localGamePath));
            Brain.GameSurrender();
            await System.IO.File.WriteAllTextAsync(localGamePath, Brain.GetBrainJson(Brain.Move()));
            return RedirectToPage("./LoadGame");
        }
    }
}