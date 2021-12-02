using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_ConfigShips
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ConfigShip>? ConfigShip { get;set; }

        public async Task OnGetAsync()
        {
            ConfigShip = await _context.ConfigShips
                .Include(c => c.Config)
                .Include(c => c.Ship).ToListAsync();
        }
    }
}
