using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages_ConfigShips
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public ConfigShip ConfigShip { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ConfigShip = await _context.ConfigShips
                .Include(c => c.Config)
                .Include(c => c.Ship).FirstOrDefaultAsync(m => m.ConfigShipId == id);

            if (ConfigShip == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ConfigShip = await _context.ConfigShips.FindAsync(id);
            List<Game> games = _context.Games.ToList();
            if (games.Any(game => ConfigShip.ConfigId == game.ConfigId))
            {
                return RedirectToPage("/CannotDelete");
            }
            var shipId = ConfigShip.ConfigId;

            if (ConfigShip != null)
            {
                var savedConf =
                    JsonSerializer.Deserialize<GameConfig>(
                        (await _context.Configs.FindAsync(ConfigShip.ConfigId)).ConfigStr);
                var ship = await _context.Ships.FindAsync(ConfigShip.ShipId);
                var shipConf = new ShipConfig
                {
                    Name = ship.Name,
                    Quantity = ConfigShip.Quantity,
                    ShipSizeX = ship.ShipLength,
                    ShipSizeY = ship.ShipHeight
                };

                foreach (var shipSave in savedConf!.ShipConfigs)
                {
                    if (shipConf.Name != shipSave.Name || shipConf.Quantity != shipSave.Quantity ||
                        shipConf.ShipSizeX != shipSave.ShipSizeX || shipConf.ShipSizeX != shipSave.ShipSizeX) continue;
                    savedConf!.ShipConfigs.Remove(shipSave);
                    break;
                }

                (await _context.Configs.FindAsync(ConfigShip.ConfigId)).ConfigStr = savedConf.ToString();
                _context.ConfigShips.Remove(ConfigShip);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Configs/Edit", new {id = shipId});
        }
    }
}