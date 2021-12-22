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
    public class DeleteModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DeleteModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PizzaToping = await _context.PizzaTopings.FindAsync(id);

            if (PizzaToping != null)
            {
                _context.PizzaTopings.Remove(PizzaToping);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
