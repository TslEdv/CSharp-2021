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

namespace WebApp.Pages_Pizzas
{
    public class EditModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public EditModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public List<PizzaToping> PizzaTopings { get; set; } = new();

        [BindProperty]
        public Pizza Pizza { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Pizza = await _context.Pizzas.FirstOrDefaultAsync(m => m.Id == id);

            if (Pizza == null)
            {
                return NotFound();
            }

            foreach (var toping in _context.PizzaTopings.Include(t => t.Topping))
            {
                if (toping.PizzaId == id)
                {
                    PizzaTopings.Add(toping);
                }
            }
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

            _context.Attach(Pizza).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaExists(Pizza.Id))
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

        private bool PizzaExists(int id)
        {
            return _context.Pizzas.Any(e => e.Id == id);
        }
    }
}
