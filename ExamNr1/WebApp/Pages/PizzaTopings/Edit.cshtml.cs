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

namespace WebApp.Pages_PizzaTopings
{
    public class EditModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public EditModel(WebApp.DAL.AppDbContext context)
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
           ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Id");
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

            _context.Attach(PizzaToping).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaTopingExists(PizzaToping.Id))
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

        private bool PizzaTopingExists(int id)
        {
            return _context.PizzaTopings.Any(e => e.Id == id);
        }
    }
}
