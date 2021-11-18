using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task<IActionResult> OnGetAsync(int id, int x, int y, int move, int rotation)
        {
            GameId = id;
            CurrentGame = await _ctx.Games.FindAsync(id);
            Brain!.RestoreBrainFromJson(CurrentGame.GameState);
            State = Brain.GetGameStatus();
            if (Brain.GameFinish())
            {
                CurrentGame.Status = Brain.GetGameStatus();
                CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                await _ctx.SaveChangesAsync();
                return Page();
            }

            switch (Brain.GetGameStatus())
            {
                case EGameStatus.Finished:
                    Player += Brain.Move() + 1 + " wins";
                    return Page();
                case EGameStatus.Placing:
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
                                        Brain.PlaceShips(x, x + Ships[i].Length - 1, y, y + Ships[i].Height - 1, Ships[i]);
                                        break;
                                    case 1:
                                        Brain.PlaceShips(x, x + Ships[i].Height - 1, y, y + Ships[i].Length - 1, Ships[i]);
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
                            Brain.PlayerMove(x, y);
                            Board = Brain.GetBoard(Brain.Move());
                            FireBoard = Brain.GetFireBoard(Brain.Move());
                            Player += Brain.Move() + 1 + " turn";
                            Ships = Brain.ListShips(Brain.Move());

                            CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                            await _ctx.SaveChangesAsync();
                            break;
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

            if (CurrentGame != null)
            {
                _ctx.Games.Remove(CurrentGame);
                await _ctx.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}