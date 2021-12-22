using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_Topings
{
    public class DetailsModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DetailsModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public Topping Topping { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Topping = await _context.Toppings.FirstOrDefaultAsync(m => m.Id == id);

            if (Topping == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
