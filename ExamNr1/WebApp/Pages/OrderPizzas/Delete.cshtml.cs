using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_OrderPizzas
{
    public class DeleteModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public DeleteModel(WebApp.DAL.AppDbContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderPizza = await _context.OrderPizzas.FindAsync(id);

            if (OrderPizza != null)
            {
                _context.OrderPizzas.Remove(OrderPizza);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
