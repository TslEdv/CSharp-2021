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

namespace WebApp.Pages_Orders
{
    public class EditModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public EditModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public List<OrderPizza> OrderPizzas { get; set; } = new();

        [BindProperty]
        public Order Order { get; set; } = default!;

        public float Price = 0;
        public string ExtraToppings = default!;
        public int Id;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Id = (int) id;

            Order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);

            if (Order == null)
            {
                return NotFound();
            }

            foreach (var orderPizza in _context.OrderPizzas
                .Include(z => z.Pizza)
                .Include(z => z.ExtraToppings)
                .ThenInclude(top => top.Topping))
            {
                if (orderPizza.OrderId != id) continue;
                OrderPizzas.Add(orderPizza);
                Price += orderPizza.Price;
                if (orderPizza.ExtraToppings != null)
                {
                    foreach (var topping in orderPizza.ExtraToppings)
                    {
                        ExtraToppings += topping.ToString();
                    }
                }
            }

            Order.Price = Price;
            await _context.SaveChangesAsync();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (action == "Order")
            {
                Order.OrderStatus = OrderStatus.Making;
                _context.Attach(Order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");

            }

            _context.Attach(Order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(Order.Id))
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

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
