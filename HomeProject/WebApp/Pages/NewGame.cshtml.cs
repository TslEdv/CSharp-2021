using System.Collections.Generic;
using System.Linq;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class NewGame : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        public NewGame(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public List<Config> Configs = default!;

        public void OnGet()
        { 
            Configs = _ctx.Configs.ToList();
        }
    }
}