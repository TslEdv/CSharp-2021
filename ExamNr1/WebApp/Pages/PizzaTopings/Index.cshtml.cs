using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL;
using WebApp.Domain;

namespace WebApp.Pages_PizzaTopings
{
    public class IndexModel : PageModel
    {
        private readonly WebApp.DAL.AppDbContext _context;

        public IndexModel(WebApp.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<PizzaToping> PizzaToping { get;set; } = default!;

        public async Task OnGetAsync()
        {
            PizzaToping = await _context.PizzaTopings
                .Include(p => p.Pizza)
                .Include(p => p.Topping).ToListAsync();
        }
    }
}
