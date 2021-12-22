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
    public class DeleteModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DeleteModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Topping = await _context.Toppings.FindAsync(id);

            if (Topping != null)
            {
                foreach (var extra in _context.ExtraToppings)
                {
                    if (extra.ToppingId == id)
                    {
                        return RedirectToPage("./Index");
                    }
                }
                foreach (var top in _context.PizzaTopings)
                {
                    if (top.ToppingId == id)
                    {
                        return RedirectToPage("./Index");
                    }
                }
                _context.Toppings.Remove(Topping);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
