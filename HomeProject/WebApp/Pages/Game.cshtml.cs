using System.Collections.Generic;
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

        [BindProperty]
        public Domain.Game CurrentGame { get; set; } = default!;
        public Game(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private static BsBrain Brain { get; set; } = new BsBrain(new GameConfig());
        
        public BoardSquareState[,]? Board { get; private set; }
        public BoardSquareState[,]? FireBoard { get; private set; }
        
        public int GameId { get; private set; }
        public string Player { get; private set; } = "Player ";
        public List<Ship>? Ships { get; private set; } = new List<Ship>();

        public bool State = false;
        public async Task<IActionResult> OnGetAsync(int id, int x, int y, int move)
        {
            GameId = id;
            CurrentGame = await _ctx.Games.FindAsync(id);
            Brain.RestoreBrainFromJson(CurrentGame.GameState);
            if (Brain.GameFinish())
            {
                State = true;
                Player += Brain.Move() + 1 + " wins";
                return Page();
            }
            else
            {
                switch (move)
                {
                    case 0:
                        Board = Brain.GetBoard(Brain.Move());
                        FireBoard = Brain.GetFireBoard(Brain.Move());
                        Player += Brain.Move() + 1 + " turn";
                        Ships = Brain.ListShips(Brain.Move());
                        break;
                    case 1:
                        GameId = id;
                        CurrentGame = await _ctx.Games.FindAsync(id);
                        Brain.RestoreBrainFromJson(CurrentGame.GameState);
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