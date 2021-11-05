using System.Collections.Generic;
using System.Security.Principal;
using BattleShipBrain;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class Game : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        public Game(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public static BsBrain Brain { get; set; } = new BsBrain(new GameConfig());
        
        public BoardSquareState[,]? Board { get; set; }
        
        public int GameId { get; set; }

        public void OnGet(int id, int x, int y)
        {
            GameId = id;
            var savedgame = _ctx.Games.Find(id);
            Brain.RestoreBrainFromJson(savedgame.GameState);
            Brain.PlayerMove(x, y);
            Board = Brain.GetBoard(Brain.Move());
            
            savedgame.GameState = Brain.GetBrainJson(Brain.Move());
            _ctx.SaveChanges();
        }
        
    }
}