using System;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class Game : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        [BindProperty]
        public Domain.Game CurrentGame { get; set; } = default!;
        public Game(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public static BsBrain Brain { get; set; } = new BsBrain(new GameConfig());
        
        public BoardSquareState[,]? Board { get; private set; }
        
        public int GameId { get; private set; }

        public void OnGet(int id, int x, int y, int move)
        {
            switch (move)
            {
                case 0:
                    GameId = id;
                    CurrentGame = _ctx.Games.Find(id);
                    Brain.RestoreBrainFromJson(CurrentGame.GameState);
                    Board = Brain.GetBoard(Brain.Move());
                    break;
                case 1:
                    GameId = id;
                    CurrentGame = _ctx.Games.Find(id);
                    Brain.RestoreBrainFromJson(CurrentGame.GameState);
                    Brain.PlayerMove(x, y);
                    Board = Brain.GetBoard(Brain.Move());

                    CurrentGame.GameState = Brain.GetBrainJson(Brain.Move());
                    _ctx.SaveChanges();
                    break;
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