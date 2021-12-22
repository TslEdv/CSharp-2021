using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_ExtraToppings
{
    public class DeleteModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DeleteModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ExtraTopping ExtraTopping { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ExtraTopping = await _context.ExtraToppings
                .Include(e => e.OrderPizza)
                .Include(e => e.Topping).FirstOrDefaultAsync(m => m.Id == id);

            if (ExtraTopping == null)
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

            ExtraTopping = await _context.ExtraToppings.FindAsync(id);

            if (ExtraTopping != null)
            {
                _context.ExtraToppings.Remove(ExtraTopping);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
