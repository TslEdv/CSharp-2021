using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages
{
    public class ShipCreate : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public ShipCreate(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Ship Ship { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Ships.Add(Ship);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ConfigShips/Create", new {id = id});
        }
    }
}