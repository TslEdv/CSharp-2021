using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_ExtraToppings
{
    public class IndexModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public IndexModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<ExtraTopping> ExtraTopping { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ExtraTopping = await _context.ExtraToppings
                .Include(e => e.OrderPizza)
                .Include(e => e.Topping).ToListAsync();
        }
    }
}
