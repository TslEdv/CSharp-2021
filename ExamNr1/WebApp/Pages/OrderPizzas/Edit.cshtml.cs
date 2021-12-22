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

namespace WebApp.Pages_OrderPizzas
{
    public class EditModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public EditModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public OrderPizza OrderPizza { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderPizza = await _context.OrderPizzas
                .Include(o => o.Order)
                .Include(o => o.Pizza).FirstOrDefaultAsync(m => m.Id == id);

            if (OrderPizza == null)
            {
                return NotFound();
            }
           ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
           ViewData["PizzaId"] = new SelectList(_context.Pizzas, "Id", "Id");
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

            _context.Attach(OrderPizza).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderPizzaExists(OrderPizza.Id))
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

        private bool OrderPizzaExists(int id)
        {
            return _context.OrderPizzas.Any(e => e.Id == id);
        }
    }
}
