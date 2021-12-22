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
    public class IndexModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public IndexModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<OrderPizza> OrderPizza { get;set; } = default!;

        public async Task OnGetAsync()
        {
            OrderPizza = await _context.OrderPizzas
                .Include(o => o.Order)
                .Include(o => o.Pizza).ToListAsync();
        }
    }
}
