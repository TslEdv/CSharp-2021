using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_ExtraToppings
{
    public class EditModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public EditModel(WebApp.DAL.AppDbContext context)
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
           ViewData["OrderPizzaId"] = new SelectList(_context.OrderPizzas, "Id", "Id");
           ViewData["ToppingId"] = new SelectList(_context.Toppings, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ExtraTopping).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraToppingExists(ExtraTopping.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ExtraToppingExists(int id)
        {
            return _context.ExtraToppings.Any(e => e.Id == id);
        }
    }
}
