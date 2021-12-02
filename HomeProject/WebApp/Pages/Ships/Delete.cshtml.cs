using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleShipBrain;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ship = Domain.Ship;

namespace WebApp.Pages.Ships
{
    public class Delete : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public Delete(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Ship Ship { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ship = await _context.Ships.FindAsync(id);

            if (Ship == null)
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

            Ship = await _context.Ships.FindAsync(id);
            List<ConfigShip> confShips = _context.ConfigShips.ToList();
            if (confShips.Any(ship => Ship.ShipId == ship.ShipId))
            {
                return RedirectToPage("/CannotDelete");
            }
            if (Ship != null)
            {
                _context.Ships.Remove(Ship);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Configs/Index");
        }
    }
}