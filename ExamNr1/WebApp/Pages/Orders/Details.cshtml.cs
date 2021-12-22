using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_Orders
{
    public class DetailsModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DetailsModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; } = default!;
        public List<OrderPizza> OrderPizzas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);

            foreach (var orderPizza in _context.OrderPizzas
                .Include(z => z.Pizza)
                .Include(z => z.ExtraToppings)
                .ThenInclude(top => top.Topping))
            {
                if (orderPizza.OrderId != id) continue;
                OrderPizzas.Add(orderPizza);
            }
            ;
            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
