using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DetailsModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public Config Config { get; set; }  = default!;
        public List<ConfigShip> ConfigShips { get; set; } = new List<ConfigShip>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            foreach (var ship in _context.ConfigShips)
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
    }
}
