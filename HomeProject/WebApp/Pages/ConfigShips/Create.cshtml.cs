using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BattleShipBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using Ship = Domain.Ship;

namespace WebApp.Pages_ConfigShips
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public ConfigShip ConfigShip { get; set; } = default!;

        public IList<Ship> Ships { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            ConfigShip = new ConfigShip
            {
                ConfigId = id,
                Config = _context.Configs.Find(id)
            };

            ViewData["ConfigId"] = new SelectList(_context.Configs, "ConfigId", "ConfigId");
            ViewData["ShipId"] = new SelectList(_context.Ships, "ShipId", "Name");
            Ships = _context.Ships.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _context.ConfigShips.Add(ConfigShip);

            var savedConf =
                JsonSerializer.Deserialize<GameConfig>(
                    (await _context.Configs.FindAsync(ConfigShip.ConfigId)).ConfigStr);
            var ship = await _context.Ships.FindAsync(ConfigShip.ShipId);
            savedConf!.ShipConfigs.Add(new ShipConfig
            {
                Name = ship.Name,
                Quantity = ConfigShip.Quantity,
                ShipSizeX = ship.ShipLength,
                ShipSizeY = ship.ShipHeight
            });

            (await _context.Configs.FindAsync(ConfigShip.ConfigId)).ConfigStr = savedConf.ToString();

            await _context.SaveChangesAsync();

            return RedirectToPage("/Configs/Edit", new{id = ConfigShip.ConfigId});
        }
    }
}