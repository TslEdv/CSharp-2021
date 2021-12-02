using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages_ConfigShips
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DetailsModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public ConfigShip ConfigShip { get; set; }  = default!;

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
    }
}
