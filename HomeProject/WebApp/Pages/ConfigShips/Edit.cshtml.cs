using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_ConfigShips
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public ConfigShip ConfigShip { get; set; } = default!;
        public IList<Ship> Ships { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ships = _context.Ships.ToList();
            ConfigShip = await _context.ConfigShips
                .Include(c => c.Config)
                .Include(c => c.Ship).FirstOrDefaultAsync(m => m.ConfigShipId == id);

            if (ConfigShip == null)
            {
                return NotFound();
            }
            ViewData["ShipId"] = new SelectList(_context.Ships, "ShipId", "Name");
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            _context.Attach(ConfigShip).State = EntityState.Modified;
            
            List<Game> games = _context.Games.ToList();
            if (games.Any(game => ConfigShip.ConfigId == game.ConfigId))
            {
                return RedirectToPage("/CannotDelete");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigShipExists(ConfigShip.ConfigShipId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Configs/Edit", new {id = ConfigShip.ConfigId});
        }

        private bool ConfigShipExists(int id)
        {
            return _context.ConfigShips.Any(e => e.ConfigShipId == id);
        }
    }
}
