using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class EditModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public EditModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Config Config { get; set; } = default!;

        public List<ConfigShip> ConfigShips { get; set; } = new List<ConfigShip>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            foreach (var ship in _context.ConfigShips.Include(cs => cs.Ship))
            {
                if (ship.ConfigId == id)
                {
                    ConfigShips.Add(ship);
                }
            }
            
            Config = await _context.Configs
                .Include(c => c.ConfigShips)
                .ThenInclude(cf => cf.Ship)
                .FirstOrDefaultAsync(m => m.ConfigId == id);

            if (Config == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _context.Attach(Config).State = EntityState.Modified;

            List<Game> games = _context.Games.ToList();
            if (games.Any(game => Config.ConfigId == game.ConfigId))
            {
                return RedirectToPage("/CannotDelete");
            }
            var savedConf = JsonSerializer.Deserialize<GameConfig>((await _context.Configs.FindAsync(Config.ConfigId)).ConfigStr);

            savedConf!.Name = Config.ConfigName;
            savedConf.IsRandom = Config.IsRandom;
            savedConf.BoardSizeX = Config.BoardSizeX;
            savedConf.BoardSizeY = Config.BoardSizeY;
            savedConf.EShipTouchRule = Config.TouchRule;
            savedConf.ShipConfigs = new List<ShipConfig>();
            foreach (var ship in _context.ConfigShips.Where(cs=> cs.ConfigId == Config.ConfigId).Include(cs => cs.Ship))
            {
                savedConf.ShipConfigs.Add(new ShipConfig
                {
                    Name = ship.Ship!.Name,
                    Quantity = ship.Quantity,
                    ShipSizeX = ship.Ship.ShipLength,
                    ShipSizeY = ship.Ship.ShipHeight
                });
            }

            Config.ConfigStr = savedConf.ToString();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigExists(Config!.ConfigId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ConfigExists(int id)
        {
            return _context.Configs.Any(e => e.ConfigId == id);
        }
    }
}
