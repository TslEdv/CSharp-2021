using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_PizzaTopings
{
    public class DetailsModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DetailsModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public PizzaToping PizzaToping { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PizzaToping = await _context.PizzaTopings
                .Include(p => p.Pizza)
                .Include(p => p.Topping).FirstOrDefaultAsync(m => m.Id == id);

            if (PizzaToping == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
